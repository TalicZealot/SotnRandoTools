using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Models;

namespace SotnRandoTools.Coop
{
	internal sealed class CoopNetworking
	{
		private readonly ICoopViewModel coopViewModel;
		private const int PingInterval = 500;
		private const int ClientTimeout = 2500;
		private const int TickInterval = 16;
		private const int MaxRetries = 5;
		private TcpListener server;
		private IPEndPoint ipEndPoint;
		private CancellationTokenSource tryConnectCancellationSource;
		private CancellationTokenSource dataReceiverTokenSource;
		private CancellationTokenSource acceptClientsTokenSource;
		private CancellationTokenSource pingTokenSource;
		private CancellationTokenSource sendTokenSource;
		private readonly ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();
		private TcpClient? client = null;
		private NetworkStream? stream = null;
		private DateTime lastPong = DateTime.UtcNow;
		private bool authenticated;
		private bool started = false;
		private bool connected = false;
		private bool isServer = false;
		private int retryCount = 0;
		private long ping = 0;
		private int[] pings = new int[28];
		private int pingIndex = 0;

		public CoopNetworking(int localPort, ICoopViewModel coopViewModel)
		{
			this.coopViewModel = coopViewModel ?? throw new ArgumentNullException(nameof(coopViewModel));
			this.LocalPort = localPort;
			this.MessageQueue = new ConcurrentQueue<byte[]>();
		}

		public CoopNetworking(IPAddress remoteServerIp, int remoteServerPort, ICoopViewModel coopViewModel)
		{
			this.coopViewModel = coopViewModel ?? throw new ArgumentNullException(nameof(coopViewModel));
			this.RemoteServerIp = remoteServerIp;
			this.RemoteServerPort = remoteServerPort;
			this.MessageQueue = new ConcurrentQueue<byte[]>();
		}

		public int LocalPort { get; set; }
		public IPAddress RemoteServerIp { get; set; }
		public int RemoteServerPort { get; set; }
		public ConcurrentQueue<byte[]> MessageQueue { get; }

		public void Start()
		{
			if (started)
			{
				return;
			}
			isServer = true;
			dataReceiverTokenSource = new();
			acceptClientsTokenSource = new();
			pingTokenSource = new();
			sendTokenSource = new();
			ipEndPoint = new(IPAddress.Any, LocalPort);
			server = new TcpListener(ipEndPoint);
			started = true;
			server.Start();
			Task.Run(() => AcceptClients().ConfigureAwait(false));
		}

		public void Stop()
		{
			if (!started)
			{
				return;
			}
			started = false;
			dataReceiverTokenSource.Cancel();
			acceptClientsTokenSource.Cancel();
			pingTokenSource.Cancel();
			sendTokenSource.Cancel();
			server.Stop();
			server.Server.Dispose();
			if (client is not null)
			{
				client.Close();
				client.Dispose();
			}
		}

		public void Connect()
		{
			if (connected)
			{
				return;
			}
			isServer = false;
			tryConnectCancellationSource = new();
			dataReceiverTokenSource = new();
			sendTokenSource = new();
			client = new TcpClient();
			Task.Run(() => TryConnect().ConfigureAwait(false));
		}

		public void Disconnect()
		{
			if (!connected)
			{
				return;
			}
			coopViewModel.Status = NetworkStatus.ManuallyDisconnected;
			tryConnectCancellationSource.Cancel();
			dataReceiverTokenSource.Cancel();
			sendTokenSource.Cancel();
			client.Close();
			client.Dispose();
		}

		public void Restart()
		{
			if (isServer)
			{
				Stop();
				Start();
			}
			else
			{
				Disconnect();
				Connect();
			}
		}

		private async Task TryConnect()
		{
			coopViewModel.Status = NetworkStatus.Reconnecting;
			var token = tryConnectCancellationSource.Token;
			while (!connected && !token.IsCancellationRequested)
			{
				try
				{
					await client.ConnectAsync(RemoteServerIp, RemoteServerPort);
					stream = client.GetStream();
					connected = true;
					_ = Task.Run(() => DataReceiver().ConfigureAwait(false));
					coopViewModel.Status = NetworkStatus.Connected;
				}
				catch (OperationCanceledException)
				{
					coopViewModel.Status = NetworkStatus.ManuallyDisconnected;
					return;
				}
				catch (Exception ex)
				{
					retryCount++;
					if (retryCount > MaxRetries)
					{
						coopViewModel.Status = NetworkStatus.TimedOut;
						return;
					}
					coopViewModel.Status = NetworkStatus.Reconnecting;
					Thread.Sleep(1000);
				}
			}
		}

		public void Send(byte[] data)
		{
			if (client == null || !client.Connected)
			{
				return;
			}
			sendQueue.Enqueue(data);
			_ = Task.Run(() => SendInternal().ConfigureAwait(false));
		}

		private async Task AcceptClients()
		{
			coopViewModel.Status = NetworkStatus.Started;
			CancellationToken token = acceptClientsTokenSource.Token;
			while (started && !token.IsCancellationRequested)
			{
				try
				{
					client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
					stream = client.GetStream();
					coopViewModel.Status = NetworkStatus.ClientConnected;
					_ = Task.Run(() => DataReceiver().ConfigureAwait(false));
					_ = Task.Run(() => SendPingMessagesAsync().ConfigureAwait(false));
					return;
				}
				catch (OperationCanceledException)
				{
					coopViewModel.Status = NetworkStatus.Stopped;
					return;
				}
				catch (ObjectDisposedException)
				{
					coopViewModel.Status = NetworkStatus.Stopped;
					return;
				}
				catch (Exception ex)
				{
					coopViewModel.Status = NetworkStatus.ServerError;
					Thread.Sleep(100);
				}
			}
		}

		private async Task DataReceiver()
		{
			var token = dataReceiverTokenSource.Token;
			while (client != null && !token.IsCancellationRequested)
			{
				if (!stream.DataAvailable)
				{
					await Task.Delay(TickInterval);
					continue;
				}
				byte[] buffer = new byte[24];
				try
				{
					int read = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);

					if (read > 0)
					{
						ProcessMessage(buffer);
					}
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (Exception ex)
				{
					Restart();
					if (isServer)
					{
						coopViewModel.Status = NetworkStatus.Started;
					}
					else
					{
						coopViewModel.Status = NetworkStatus.Reconnecting;
					}
					return;
				}
				await Task.Delay(TickInterval);
			}
		}

		private async Task SendInternal()
		{
			if (client == null || !client.Connected)
			{
				return;
			}
			var token = sendTokenSource.Token;
			while (!token.IsCancellationRequested && client.Connected)
			{
				if (sendQueue.IsEmpty)
				{
					return;
				}

				byte[] data;
				if (!sendQueue.TryDequeue(out data))
				{
					await Task.Delay(TickInterval);
					continue;
				}
				try
				{
					if (data[0] == (byte) MessageType.Ping)
					{
						DateTime now = DateTime.UtcNow;
						byte[] pingMessage = new byte[9];
						byte[] timeBytes = BitConverter.GetBytes(now.Ticks);
						pingMessage[0] = (byte) MessageType.Ping;
						for (int i = 0; i < timeBytes.Length; i++)
						{
							pingMessage[i + 1] = timeBytes[i];
						}
						data = pingMessage;
					}
					if (data[0] == (byte) MessageType.Pong)
					{
						DateTime now = DateTime.UtcNow;
						byte[] pongMessage = new byte[9];
						byte[] timeBytes = BitConverter.GetBytes(now.Ticks);
						pongMessage[0] = (byte) MessageType.Pong;
						for (int i = 0; i < timeBytes.Length; i++)
						{
							pongMessage[i + 1] = timeBytes[i];
						}
						data = pongMessage;
					}
					await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (Exception ex)
				{
					Restart();
					if (isServer)
					{
						coopViewModel.Status = NetworkStatus.Started;
					}
					else
					{
						coopViewModel.Status = NetworkStatus.Reconnecting;
					}
					return;
				}
				await Task.Delay(TickInterval);
			}
		}

		private async Task SendPingMessagesAsync()
		{
			var token = pingTokenSource.Token;
			while (!token.IsCancellationRequested)
			{
				DateTime now = DateTime.UtcNow;
				var elapsed = now - lastPong;
				if (elapsed.Milliseconds > ClientTimeout)
				{
					coopViewModel.Status = NetworkStatus.TimedOut;
					Restart();
					return;
				}

				byte[] pingMessage = new byte[1];
				pingMessage[0] = (byte) MessageType.Ping;
				Send(pingMessage);

				await Task.Delay(PingInterval);
			}
		}

		private void ProcessMessage(byte[] data)
		{
			MessageType msgType = (MessageType) data[0];
			if (msgType == MessageType.Pong)
			{
				lastPong = DateTime.UtcNow;
				long sentAt = BitConverter.ToInt64(data, 1);
				TimeSpan span = new TimeSpan(lastPong.Ticks - sentAt);
				SetPing(span.Milliseconds);
			}
			else if (msgType == MessageType.Ping)
			{
				long sentAt = BitConverter.ToInt64(data, 1);
				DateTime now = DateTime.UtcNow;
				TimeSpan span = new TimeSpan(now.Ticks - sentAt);
				SetPing(span.Milliseconds);
				byte[] pongMessage = new byte[1];
				pongMessage[0] = (byte) MessageType.Pong;
				Send(pongMessage);
			}
			else
			{
				MessageQueue.Enqueue(data);
			}
		}

		private void SetPing(int milliseconds)
		{
			pings[pingIndex] = milliseconds;
			pingIndex = (pingIndex + 1) % pings.Length;
			int totalMs = 0;
			for (int i = 0; i < pings.Length; i++)
			{
				totalMs += pings[i];
			}
			int ping = (totalMs / pings.Length) - TickInterval;
			if (ping < 0)
			{
				ping = 0;
			}
			coopViewModel.Ping = ping;
		}
	}
}

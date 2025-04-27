using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Interfaces;

namespace SotnRandoTools.Services
{
	internal sealed class WSClient
	{
		public int SocketId { get; set; }
		public WebSocket Socket { get; set; }
	}
	internal sealed class OverlaySocketServer : IOverlaySocketServer
	{
		private readonly IToolConfig toolConfig;
		private HttpListener server;
		private bool started = false;
		private List<WSClient> clients = new List<WSClient>();
		private CancellationTokenSource socketLoopTokenSource;
		private CancellationTokenSource listenerLoopTokenSource;
		private int socketCounter = 0;
		private byte[] objectsData = new byte[13];
		private int relics = 0;
		private int items = 0;
		private int bosses = 0;
		public OverlaySocketServer(IToolConfig toolConfig)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
		}

		public void StartServer()
		{
			if (started)
			{
				return;
			}
			socketLoopTokenSource = new CancellationTokenSource();
			listenerLoopTokenSource = new CancellationTokenSource();
			server = new HttpListener();
			server.Prefixes.Add(Globals.WebSocketUri);
			started = true;
			server.Start();
			Task.Run(() => AcceptClients().ConfigureAwait(false));
		}

		public async Task StopServer()
		{
			if (!started)
			{
				return;
			}
			await CloseAllSocketsAsync();
			try
			{
				server.Stop();
				server.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			started = false;
		}

		private async Task CloseAllSocketsAsync()
		{
			var disposeQueue = new List<WebSocket>(clients.Count);

			while (clients.Count > 0)
			{
				var client = clients[clients.Count - 1];

				if (client.Socket.State == WebSocketState.Open)
				{
					var timeout = new CancellationTokenSource(700);
					try
					{
						await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", timeout.Token);
					}
					catch (OperationCanceledException ex)
					{
						Console.Write(ex.ToString());
					}
				}

				lock (clients)
				{
					clients.Remove(client);
				}
				disposeQueue.Add(client.Socket);
			}

			socketLoopTokenSource.Cancel();

			for (int i = 0; i < disposeQueue.Count; i++)
			{
				disposeQueue[i].Dispose();
			}
		}

		private async Task AcceptClients()
		{
			CancellationToken token = listenerLoopTokenSource.Token;
			while (started && !token.IsCancellationRequested)
			{
				HttpListenerContext ctx;
				try
				{
					ctx = await server.GetContextAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					continue;
				}

				var endpoint = ctx.Request.RemoteEndPoint.Address.ToString();
				if ((endpoint != "127.0.0.1" && endpoint != "::1") || !ctx.Request.IsWebSocketRequest)
				{
					ctx.Response.StatusCode = 400;
					ctx.Response.Close();
					continue;
				}

				try
				{
					WebSocketContext wsContext = await ctx.AcceptWebSocketAsync(subProtocol: null);
					int socketId = Interlocked.Increment(ref socketCounter);
					WSClient client = new WSClient { SocketId = socketId, Socket = wsContext.WebSocket };
					lock (clients)
					{
						clients.Add(client);
					}
					_ = Task.Run(() => SocketProcessingLoopAsync(client).ConfigureAwait(false));
					UpdateLayout();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					ctx.Response.StatusCode = 500;
					ctx.Response.Close();
				}
			}
		}

		private async Task SocketProcessingLoopAsync(WSClient client)
		{
			var socket = client.Socket;
			var loopToken = socketLoopTokenSource.Token;
			try
			{
				ArraySegment<byte> buffer = WebSocket.CreateServerBuffer(512);
				while (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted && !loopToken.IsCancellationRequested)
				{
					WebSocketReceiveResult receiveResult = await client.Socket.ReceiveAsync(buffer, loopToken);

					if (loopToken.IsCancellationRequested)
					{
						break;
					}
					if (client.Socket.State == WebSocketState.CloseReceived && receiveResult.MessageType == WebSocketMessageType.Close)
					{
						await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge Close frame", CancellationToken.None);
					}
					else if (receiveResult.MessageType == WebSocketMessageType.Binary)
					{
						SaveSlots(buffer.Array);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Socket {client.SocketId}: {ex.ToString()}");
			}
			finally
			{
				if (client.Socket.State != WebSocketState.Closed)
				{
					client.Socket.Abort();
				}

				lock (clients)
				{
					clients.Remove(client);
				}
				socket.Dispose();
			}
		}

		private Task<bool> SendData(WSClient client, byte[] data)
		{
			Task<bool> task = MessageWriteAsync(client, new ArraySegment<byte>(data));
			return task;
		}

		private async Task<bool> MessageWriteAsync(WSClient client, ArraySegment<byte> data)
		{
			CancellationToken token = socketLoopTokenSource.Token;
			try
			{
				await client.Socket.SendAsync(data, WebSocketMessageType.Binary, true, token).ConfigureAwait(false);
				return true;
			}
			catch (Exception e)
			{
				lock (clients)
				{
					clients.Remove(client);
				}
			}
			finally
			{
				client = null;
			}

			return false;
		}

		public void UpdateLayout()
		{
			SendSlots();
			UpdateTracker(relics, items, bosses);
		}

		public void UpdateTracker(int trackerRelics, int trackerItems, int trackerBosses)
		{
			relics = trackerRelics;
			items = trackerItems;
			bosses = trackerBosses;
			objectsData[0] = 0;
			int dataIndex = 1;

			byte[] relicsBytes = BitConverter.GetBytes(relics);
			for (int i = 0; i < relicsBytes.Length; i++)
			{
				objectsData[dataIndex] = relicsBytes[i];
				dataIndex++;
			}

			byte[] itemsBytes = BitConverter.GetBytes(items);
			for (int i = 0; i < itemsBytes.Length; i++)
			{
				objectsData[dataIndex] = itemsBytes[i];
				dataIndex++;
			}

			byte[] bossesBytes = BitConverter.GetBytes(bosses);
			for (int i = 0; i < bossesBytes.Length; i++)
			{
				objectsData[dataIndex] = bossesBytes[i];
				dataIndex++;
			}

			for (int i = 0; i < clients.Count; i++)
			{
				SendData(clients[i], objectsData);
			}
		}

		public void SendSlots()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				SendData(clients[i], toolConfig.Tracker.OverlaySlots);
			}
		}

		public void SaveSlots(byte[] data)
		{
			byte[] trimmed = new byte[57];
			Array.Copy(data, trimmed, 57);
			toolConfig.Tracker.OverlaySlots = trimmed;
		}
	}
}

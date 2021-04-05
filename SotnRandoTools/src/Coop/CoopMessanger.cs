using System;
using System.Net;
using SimpleTcp;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Utils;

namespace SotnRandoTools.Coop
{
	public class CoopMessanger : ICoopMessanger
	{
		private readonly IToolConfig toolConfig;
		private readonly ICoopReceiver coopReceiver;
		private SimpleTcpServer? server;
		private SimpleTcpClient? client;
		private string connectedClientAddress = "";

		public CoopMessanger(IToolConfig toolConfig, ICoopReceiver coopReceiver)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (coopReceiver is null) throw new ArgumentNullException(nameof(coopReceiver));
			this.toolConfig = toolConfig;
			this.coopReceiver = coopReceiver;
		}

		public bool Connect(string hostIp, int port)
		{
			if (client is null)
			{
				client = new SimpleTcpClient(hostIp, port);
			}

			//Causes disconnects instead of keeping the connection up.
			//client.Keepalive.EnableTcpKeepAlives = true;

			try
			{
				client.ConnectWithRetries(4 * 1000);
			}
			catch (Exception)
			{
				Console.WriteLine("Connection timed out.");
				return false;
			}

			client.Events.DataReceived += DataReceived;
			client.Events.Connected += Connected;
			client.Events.Disconnected += Disconnected;

			return true;
		}

		public void Disconnect()
		{
			if (client is not null)
			{
				client.Disconnect();
				client.Dispose();
			}
		}

		public bool StartServer(int port)
		{
			string hostName = Dns.GetHostName();

			if (server is null)
			{
				server = new SimpleTcpServer("0.0.0.0", port);

				server.Events.ClientConnected += ClientConnected;
				server.Events.ClientDisconnected += ClientDisconnected;
				server.Events.DataReceived += DataReceived;
			}

			//Causes disconnects instead of keeping the connection up.
			//server.Keepalive.EnableTcpKeepAlives = true;

			server.Start();
			string myIP = WebRequests.getExternalIP().Replace("\n", "");
			System.Windows.Forms.Clipboard.SetText(myIP + ":" + port);

			toolConfig.Coop.InitiateServerSettings();
			Console.WriteLine($"Server started. Address copied to clipboard.");

			return true;
		}

		public void StopServer()
		{
			if (server is not null)
			{
				server.Stop();
				server.Dispose();
			}
		}

		public void SendData(MessageType type, byte[] data)
		{
			if (server is not null && connectedClientAddress != "")
			{
				server.Send(connectedClientAddress, new byte[] { (byte) type, data[0], data[1] });
			}
			else if (client is not null && client.IsConnected)
			{
				client.Send(new byte[] { (byte) type, data[0], data[1] });
			}
			else
			{
				Console.WriteLine("No connection!");
			}
		}

		private void Connected(object sender, ClientConnectedEventArgs e)
		{
			Console.WriteLine("Connected successfully.");
			if (toolConfig.Coop.StoreLastServer)
			{
				toolConfig.Coop.DefaultServer = client.ServerIpPort;
			}
		}

		private void Disconnected(object sender, ClientDisconnectedEventArgs e)
		{
			client.Dispose();
			Console.WriteLine($"Disconnected from host.");
		}

		private void ClientConnected(object sender, ClientConnectedEventArgs e)
		{
			Console.WriteLine($"Client has connected successfully.");
			connectedClientAddress = e.IpPort;
			Console.WriteLine($"Sending settings...");
			SendSettings();
		}

		private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			Console.WriteLine($"Client has disconnected.");
			if (connectedClientAddress == e.IpPort)
			{
				connectedClientAddress = "";
			}
		}

		private void DataReceived(object sender, DataReceivedEventArgs e)
		{
			coopReceiver.ProcessMessage(e.Data);
		}

		private void SendSettings()
		{
			int settings = 0;
			if (toolConfig.Coop.SendRelics)
			{
				settings = settings | (int) SettingsFlags.SendRelics;
			}
			if (toolConfig.Coop.ShareWarps)
			{
				settings = settings | (int) SettingsFlags.ShareWarps;
			}
			if (toolConfig.Coop.ShareShortcuts)
			{
				settings = settings | (int) SettingsFlags.ShareShortcuts;
			}
			if (toolConfig.Coop.SendItems)
			{
				settings = settings | (int) SettingsFlags.SendItems;
			}
			if (toolConfig.Coop.Assists)
			{
				settings = settings | (int) SettingsFlags.SendAssists;
			}

			SendData(MessageType.Settings, BitConverter.GetBytes((ushort) settings));
		}
	}
}

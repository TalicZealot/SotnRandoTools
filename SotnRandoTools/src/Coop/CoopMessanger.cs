using System;
using System.Net;
using SimpleTcp;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using SotnRandoTools.Utils;

namespace SotnRandoTools.Coop
{
	public class CoopMessanger : ICoopMessanger
	{
		private readonly IToolConfig toolConfig;
		private readonly ICoopReceiver coopReceiver;
		private readonly ICoopViewModel coopViewModel;

		private SimpleTcpServer? server;
		private SimpleTcpClient? client;
		private string connectedClientAddress = "";
		private bool manualDisconnect = false;

		public CoopMessanger(IToolConfig toolConfig, ICoopReceiver coopReceiver, ICoopViewModel coopViewModel)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (coopReceiver is null) throw new ArgumentNullException(nameof(coopReceiver));
			if (coopViewModel is null) throw new ArgumentNullException(nameof(coopViewModel));
			this.toolConfig = toolConfig;
			this.coopReceiver = coopReceiver;
			this.coopViewModel = coopViewModel;
		}

		public void Connect(string hostIp, int port)
		{
			if (client is null)
			{
				client = new SimpleTcpClient(hostIp, port);
				client.Events.DataReceived += DataReceived;
				client.Events.Connected += Connected;
				client.Events.Disconnected += Disconnected;
			}

			try
			{
				client.ConnectWithRetries(4 * 1000);
			}
			catch (Exception)
			{
				coopViewModel.Message = "Connection timed out!";
				Console.WriteLine("Connection timed out!");
				coopViewModel.ClientConnected = false;
				return;
			}

			return;
		}

		public void Disconnect()
		{
			if (client is not null && client.IsConnected)
			{
				manualDisconnect = true;
				client.Disconnect();
				coopViewModel.ClientConnected = false;
				coopViewModel.Message = "Disconnected";
			}
		}

		public void StartServer(int port)
		{
			string hostName = Dns.GetHostName();

			if (server is null)
			{
				server = new SimpleTcpServer("0.0.0.0", port);

				server.Events.ClientConnected += ClientConnected;
				server.Events.ClientDisconnected += ClientDisconnected;
				server.Events.DataReceived += DataReceived;
			}

			try
			{
				coopViewModel.ServerStarted = true;
				server.Start();
			}
			catch (Exception)
			{
				Console.WriteLine("Error: Could not start server!");
				coopViewModel.ServerStarted = false;
				return;
			}

			string myIP = WebRequests.getExternalIP().Replace("\n", "");
			System.Windows.Forms.Clipboard.SetText(myIP + ":" + port);
			toolConfig.Coop.InitiateServerSettings();
			Console.WriteLine($"Server started. Address copied to clipboard.");
			coopViewModel.Message = "Server started";

			return;
		}

		public void StopServer()
		{
			if (server is not null && server.IsListening)
			{
				foreach (var client in server.GetClients())
				{
					server.DisconnectClient(client);
				}
				server.Stop();
				coopViewModel.ServerStarted = false;
				connectedClientAddress = "";
				coopViewModel.Message = "Server stopped";
			}
		}

		public void DisposeAll()
		{
			if (server is not null)
			{
				server.Dispose();
			}
			if (client is not null)
			{
				client.Dispose();
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

		public bool IsConnected()
		{
			if (server is not null && connectedClientAddress != "")
			{
				return true;
			}
			else if (client is not null && client.IsConnected)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void Connected(object sender, ClientConnectedEventArgs e)
		{
			Console.WriteLine("Connected successfully.");
			coopViewModel.ClientConnected = true;
			coopViewModel.Message = "Connected";

			if (toolConfig.Coop.StoreLastServer)
			{
				toolConfig.Coop.DefaultServer = client.ServerIpPort;
			}
		}

		private void Disconnected(object sender, ClientDisconnectedEventArgs e)
		{
			Console.WriteLine("Disconnected from host.");
			if (!manualDisconnect)
			{
				Console.WriteLine($"Attempting to reconnect...");
				coopViewModel.Message = "Reconnecting...";
				try
				{
					client.ConnectWithRetries(2 * 1000);
				}
				catch (Exception)
				{
					coopViewModel.Message = "Disconnected";
					Console.WriteLine("Connection timed out!");
					coopViewModel.ClientConnected = false;
					return;
				}
			}
			else
			{
				coopViewModel.Message = "Disconnected";
				coopViewModel.ClientConnected = false;
				manualDisconnect = false;
			}
		}

		private void ClientConnected(object sender, ClientConnectedEventArgs e)
		{
			coopViewModel.Message = "Client connected";
			Console.WriteLine($"Client has connected successfully.");
			connectedClientAddress = e.IpPort;
			Console.WriteLine($"Sending settings...");
			SendSettings();
		}

		private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			coopViewModel.Message = "Client disconnected";
			Console.WriteLine($"Client has disconnected.");
			if (connectedClientAddress == e.IpPort)
			{
				connectedClientAddress = "";
			}
		}

		private void DataReceived(object sender, DataReceivedEventArgs e)
		{
			coopReceiver.EnqueMessage(e.Data);
		}

		private void SendSettings()
		{
			int settings = 0;
			if (toolConfig.Coop.ShareRelics)
			{
				settings = settings | (int) SettingsFlags.ShareRelics;
			}
			if (toolConfig.Coop.ShareWarps)
			{
				settings = settings | (int) SettingsFlags.ShareWarps;
			}
			if (toolConfig.Coop.SendItems)
			{
				settings = settings | (int) SettingsFlags.SendItems;
			}
			if (toolConfig.Coop.SendAssists)
			{
				settings = settings | (int) SettingsFlags.SendAssists;
			}
			if (toolConfig.Coop.ShareLocations)
			{
				settings = settings | (int) SettingsFlags.ShareLocations;
			}

			SendData(MessageType.Settings, BitConverter.GetBytes((ushort) settings));
		}
	}
}

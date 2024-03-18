using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Addresses.Alucard;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Interfaces;
using WatsonWebsocket;

namespace SotnRandoTools.Services
{
	internal sealed class TrackerObjects
	{
		[JsonProperty("relics")]
		public int Relics { get; set; }
		[JsonProperty("irems")]
		public int Items { get; set; }
		[JsonProperty("type")]
		public string Type { get; } = "relics";
	}
	internal sealed class TrackerSlots
	{
		[JsonProperty("slots")]
		public List<List<int>> Slots { get; set; }
		[JsonProperty("type")]
		public string Type { get; } = "slots";
	}
	internal sealed class OverlaySocketServer : IOverlaySocketServer
	{
		private readonly IToolConfig toolConfig;
		private WatsonWsServer socketServer;
		private readonly List<string> clients = new();
		private TrackerObjects trackerObjects = new();
		private TrackerSlots trackerSlots = new();
		public OverlaySocketServer(IToolConfig toolConfig)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));

			socketServer = new WatsonWsServer(new Uri(Globals.SocketUri));
			socketServer.ClientConnected += ClientConnected;
			socketServer.ClientDisconnected += ClientDisconnected;
			socketServer.MessageReceived += MessageReceived;
			trackerSlots.Slots = toolConfig.Tracker.OverlaySlots;
		}

		public void StartServer()
		{
			if (!socketServer.IsListening)
			{
				socketServer.Start();
			}
		}

		public void StopServer()
		{
			if (socketServer.IsListening)
			{
				socketServer.Stop();
			}
		}

		public void UpdateTracker(int relics, int items)
		{
			trackerObjects.Relics = relics;
			trackerObjects.Items = items;

			for (int i = 0; i < clients.Count; i++)
			{
				socketServer.SendAsync(clients[i], JsonConvert.SerializeObject(trackerObjects));
			}
		}

		private void ClientConnected(object sender, ClientConnectedEventArgs args)
		{
			clients.Add(args.IpPort);

			for (int i = 0; i < clients.Count; i++)
			{
				socketServer.SendAsync(clients[i], JsonConvert.SerializeObject(trackerSlots));
			}
		}

		private void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
		{
			clients.Remove(args.IpPort);
		}

		private void MessageReceived(object sender, MessageReceivedEventArgs args)
		{
			Console.WriteLine("Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
			JObject eventJson = JObject.Parse(Encoding.UTF8.GetString(args.Data));
			if (eventJson["event"] is not null && eventJson["slots"] is not null && eventJson["event"].ToString() == "save-slots")
			{
				string slotsData = eventJson["slots"].ToString();
				List<List<int>>? newSlots = JsonConvert.DeserializeObject<List<List<int>>>(slotsData);
				if (newSlots is not null)
				{
					toolConfig.Tracker.OverlaySlots = newSlots;
				}
			}

		}
	}
}

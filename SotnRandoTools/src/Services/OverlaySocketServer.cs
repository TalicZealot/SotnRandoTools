using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services.Interfaces;
using WatsonWebsocket;

namespace SotnRandoTools.Services
{
	public class OverlaySocketServer : IOverlaySocketServer
	{
		private readonly IToolConfig toolConfig;
		private WatsonWsServer socketServer;
		public OverlaySocketServer(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			socketServer = new WatsonWsServer(new Uri(Globals.SocketUri));
			socketServer.ClientConnected += ClientConnected;
			socketServer.ClientDisconnected += ClientDisconnected;
			socketServer.MessageReceived += MessageReceived;
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

		public void AddTimer(string name, int duration)
		{
			JObject data = JObject.FromObject(new
			{
				name = name.ToLower().Replace(" ", String.Empty),
				duration = duration, //in milliseconds
				type = "timer"
			});

			foreach (var client in socketServer.ListClients())
			{
				socketServer.SendAsync(client, data.ToString());
			}
		}

		public void UpdateQueue(List<QueuedAction> actionQueue)
		{
			string[] actions = actionQueue.Select(action => action.Name.ToLower().Replace(" ", String.Empty).Replace("'", String.Empty)).ToArray();

			JObject data = JObject.FromObject(new
			{
				actions = actions,
				type = "actions"
			});

			foreach (var client in socketServer.ListClients())
			{
				socketServer.SendAsync(client, data.ToString());
			}
		}

		public void UpdateTracker(int relics, int items)
		{
			JObject data = JObject.FromObject(new
			{
				relics = relics,
				items = items,
				type = "relics"
			});

			foreach (var client in socketServer.ListClients())
			{
				socketServer.SendAsync(client, data.ToString());
			}
		}

		private void ClientConnected(object sender, ClientConnectedEventArgs args)
		{
			Console.WriteLine("Client connected: " + args.IpPort);

			JObject data = JObject.FromObject(new
			{
				slots = toolConfig.Tracker.OverlaySlots,
				type = "slots"
			});

			foreach (var client in socketServer.ListClients())
			{
				socketServer.SendAsync(client, data.ToString());
			}
		}

		private void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
		{
			Console.WriteLine("Client disconnected: " + args.IpPort);
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

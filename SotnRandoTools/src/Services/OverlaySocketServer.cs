using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using WatsonWebsocket;

namespace SotnRandoTools.Services
{
	public class OverlaySocketServer : IOverlaySocketServer
	{
		private WatsonWsServer socketServer;
		public OverlaySocketServer()
		{
			socketServer = new WatsonWsServer(new Uri(Globals.SocketUri));
			socketServer.ClientConnected += ClientConnected;
			socketServer.ClientDisconnected += ClientDisconnected;
			socketServer.MessageReceived += MessageReceived;
		}

		public void StartServer()
		{
			socketServer.Start();
		}

		public void StopServer()
		{
			socketServer.Stop();
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

		private void ClientConnected(object sender, ClientConnectedEventArgs args)
		{
			Console.WriteLine("Client connected: " + args.IpPort);
		}

		private void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
		{
			Console.WriteLine("Client disconnected: " + args.IpPort);
		}

		private void MessageReceived(object sender, MessageReceivedEventArgs args)
		{
			Console.WriteLine("Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
		}
	}
}

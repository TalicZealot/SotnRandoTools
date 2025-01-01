using System;
using System.Net;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Coop.Models;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools.Coop
{
	internal sealed class CoopController : ICoopController
	{
		private readonly CoopReceiver coopReceiver;
		private readonly CoopSender coopSender;
		private readonly ICoopViewModel coopViewModel;
		private readonly ISotnApi sotnApi;
		private CoopNetworking? socket;
		private CoopState coopState;

		public CoopController(IToolConfig toolConfig, ISotnApi sotnApi, ICoopViewModel coopViewModel, INotificationService notificationService, ILocationTracker locationTracker)
		{
			this.coopViewModel = coopViewModel ?? throw new ArgumentNullException(nameof(coopViewModel));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			coopSender = new CoopSender(toolConfig, sotnApi, this);
			coopReceiver = new CoopReceiver(toolConfig, sotnApi, notificationService, this);
			coopState = new CoopState(sotnApi, locationTracker);
		}

		public CoopState CoopState
		{
			get
			{
				return coopState;
			} 
		}

		public bool SynchRequested { get; set; }

		public void Update()
		{
			if (socket == null)
			{
				return;
			}
			if (sotnApi.GameApi.InAlucardMode())
			{
				coopState.Update();
			}
			coopSender.Update();
			coopReceiver.Update();
		}

		public void Connect(string hostIp, int port)
		{
			if (port < Globals.PortMinimum || port > Globals.PortMaximum) throw new ArgumentOutOfRangeException($"Port must be between {Globals.PortMinimum} and {Globals.PortMaximum}");
			if (string.IsNullOrEmpty(hostIp)) throw new ArgumentNullException(nameof(hostIp));
			if (!IPAddress.TryParse(hostIp, out var ip)) throw new ArgumentException("Invalid Ip string.");

			if (socket is null)
			{
				socket = new CoopNetworking(IPAddress.Parse(hostIp), port, coopViewModel);
				coopReceiver.MessageQueue = socket.MessageQueue;
			}
			else
			{
				socket.RemoteServerIp = IPAddress.Parse(hostIp);
				socket.RemoteServerPort = port;
				coopReceiver.MessageQueue = socket.MessageQueue;
			}

			socket.Connect();

			return;
		}

		public void Disconnect()
		{
			if (socket is not null)
			{
				socket.Disconnect();
			}
		}

		public void StartServer(int port)
		{
			if (port < Globals.PortMinimum || port > Globals.PortMaximum) throw new ArgumentOutOfRangeException($"Port must be between {Globals.PortMinimum} and {Globals.PortMaximum}");
			string hostName = Dns.GetHostName();

			if (socket is null)
			{
				socket = new CoopNetworking(port, coopViewModel);
				coopReceiver.MessageQueue = socket.MessageQueue;
			}

			socket.Start();
			return;
		}

		public void StopServer()
		{
			if (socket is not null)
			{
				socket.Stop();
			}
		}

		public void DisposeAll()
		{
			if (socket is not null)
			{
				socket.Stop();
				socket.Disconnect();
			}
		}

		//TODO: investigate send queue
		public void SendData(byte[] data)
		{
			System.Diagnostics.Debug.Assert(data != null);
			System.Diagnostics.Debug.Assert(data.Length >= 1);

			if (socket is not null)
			{
				socket.Send(data);
			}
			else
			{
				Console.WriteLine("No connection!");
			}
		}

		public bool IsConnected()
		{
			if (socket is not null && (coopViewModel.Status == NetworkStatus.Connected || coopViewModel.Status == NetworkStatus.ClientConnected))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	internal sealed partial class CoopForm : Form
	{
		private readonly IInputService inputService;
		private readonly IToolConfig toolConfig;
		private CoopSender? coopSender;
		private CoopMessanger? coopMessanger;
		private CoopReceiver? coopReceiver;
		private readonly INotificationService notificationService;
		private CoopViewModel coopViewModel = new CoopViewModel();
		private bool addressValidated = false;
		private Color BaseBackground = Color.FromArgb(17, 0, 17);

		public CoopForm(IToolConfig toolConfig, IWatchlistService watchlistService, IInputService inputService, ISotnApi sotnApi, IJoypadApi joypadApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (joypadApi is null) throw new ArgumentNullException(nameof(joypadApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.notificationService = notificationService;
			this.inputService = inputService;

			this.coopReceiver = new CoopReceiver(toolConfig, sotnApi, notificationService, watchlistService);
			this.coopMessanger = new CoopMessanger(toolConfig, coopReceiver, coopViewModel);
			this.coopSender = new CoopSender(toolConfig, watchlistService, inputService, sotnApi, coopMessanger);
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			this.portNumeric.Controls[0].Visible = false;
			coopViewModel.PropertyChanged += CoopViewModelPropertyChanged;
		}

		private void CoopViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(CoopViewModel.Message):
					notificationService.AddMessage(coopViewModel.Message);
					break;
				case nameof(CoopViewModel.ServerStatus):
					SetServerStatus(coopViewModel.ServerStatus);
					break;
				case nameof(CoopViewModel.ClientStatus):
					SetClientStatus(coopViewModel.ClientStatus);
					break;
				default:
					break;
			}
		}

		private void SetServerStatus(ServerStatus status)
		{
			Console.WriteLine(status.ToString());
			switch (status)
			{
				case ServerStatus.Started:
					this.hostButton.Text = "Stop";
					this.hostButton.ForeColor = Color.Black;
					this.hostButton.BackColor = Color.Aqua;
					this.connectButton.BackColor = BaseBackground;
					this.connectButton.ForeColor = Color.White;
					this.connectButton.Enabled = false;
					this.targetIp.Enabled = false;
					this.portNumeric.Enabled = false;
					this.IPlabel.Enabled = false;
					break;
				case ServerStatus.Stopped:
					this.hostButton.Text = "Host";
					this.hostButton.BackColor = BaseBackground;
					this.hostButton.ForeColor = Color.White;
					this.connectButton.Enabled = true;
					this.targetIp.Enabled = true;
					this.portNumeric.Enabled = true;
					this.IPlabel.Enabled = true;
					break;
				case ServerStatus.Error:
					this.hostButton.BackColor = Color.Crimson;
					break;
				case ServerStatus.ClientConnected:
					this.hostButton.BackColor = Color.SpringGreen;
					break;
				case ServerStatus.ClientDisconnected:
					this.hostButton.BackColor = Color.Crimson;
					break;
				default: break;
			}
		}

		private void SetClientStatus(ClientStatus status)
		{
			Console.WriteLine(status.ToString());
			switch (status)
			{
				case ClientStatus.Connected:
					this.connectButton.Text = "Disconnect";
					this.connectButton.ForeColor = Color.Black;
					this.connectButton.BackColor = Color.SpringGreen;
					this.hostButton.BackColor = BaseBackground;
					this.hostButton.ForeColor = Color.White;
					this.hostButton.Enabled = false;
					this.targetIp.Enabled = false;
					this.portNumeric.Enabled = false;
					break;
				case ClientStatus.Reconnecting:
					this.connectButton.Text = "Reconnecting";
					this.connectButton.BackColor = Color.Coral;
					break;
				case ClientStatus.Disconnected:
					this.connectButton.Text = "Connect";
					this.connectButton.BackColor = Color.Crimson;
					this.connectButton.ForeColor = Color.White;
					this.hostButton.Enabled = true;
					this.targetIp.Enabled = true;
					this.portNumeric.Enabled = true;
					break;
				case ClientStatus.ManuallyDisconnected:
					this.connectButton.Text = "Connect";
					this.connectButton.BackColor = BaseBackground;
					this.connectButton.ForeColor = Color.White;
					this.hostButton.Enabled = true;
					this.targetIp.Enabled = true;
					this.portNumeric.Enabled = true;
					break;
				default: break;
			}
		}

		public void UpdateCoop()
		{
			if (coopSender is not null && coopReceiver is not null)
			{
				coopSender.Update();
				coopReceiver.ExecuteMessage();
			}
		}

		private void CoopForm_Load(object sender, EventArgs e)
		{
			if (SystemInformation.VirtualScreen.Width > toolConfig.Coop.Location.X && SystemInformation.VirtualScreen.Height > toolConfig.Coop.Location.Y)
			{
				this.Location = toolConfig.Coop.Location;
			}
			this.portNumeric.Value = toolConfig.Coop.DefaultPort;
			this.targetIp.Text = toolConfig.Coop.DefaultServer;
			inputService.Polling++;
			ValidateAddress();

#if WIN
			this.Icon = SotnRandoTools.Properties.Resources.Icon;
#endif
		}

		private void CoopForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Coop.Location = this.Location;
			}
		}

		private void CoopForm_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void hostButton_Click(object sender, EventArgs e)
		{
			if (coopViewModel.ServerStatus != ServerStatus.Stopped && coopViewModel.ServerStatus != ServerStatus.Error)
			{
				coopMessanger.StopServer();
				return;
			}

			int port = (int) this.portNumeric.Value;

			if (port > 1024 && port < 49151)
			{
				coopMessanger.StartServer(port);
			}
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			if (coopViewModel.ClientStatus != ClientStatus.Disconnected && coopViewModel.ClientStatus != ClientStatus.ManuallyDisconnected)
			{
				coopMessanger.Disconnect();
				return;
			}

			string[] hostAddress = this.targetIp.Text.Split(':');

			if (!addressValidated)
			{
				Console.WriteLine("Invalid address!");
			}
			else
			{
				Console.WriteLine("Connecting...");
				coopMessanger.Connect(hostAddress[0], Int32.Parse(hostAddress[1]));
			}
		}

		private void targetIp_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//If validation is triggered by the form, return in order to ignore it and let the user close the window.
			if (this.ActiveControl.Equals(sender))
				return;
			if (!ValidateAddress())
			{
				e.Cancel = true;
			}
		}

		private bool ValidateAddress()
		{
			Regex validIpPort = new Regex(@"^(\d{1,3})\.(\d{1,3})\.(\d{1,3}).(\d{1,3})\:(\d{3,5})$");
			if (!validIpPort.IsMatch(this.targetIp.Text))
			{
				addressValidated = false;
				this.targetIp.Text = "";
				this.targetIp.BackColor = Color.Red;
				this.addressTooltip.SetToolTip(targetIp, "Invalid address!");
				this.addressTooltip.ToolTipIcon = ToolTipIcon.Warning;
				this.addressTooltip.Active = true;
				return false;
			}
			addressValidated = true;
			return true;
		}

		private void CoopForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			inputService.Polling--;
			coopMessanger.Disconnect();
			coopMessanger.StopServer();
			coopMessanger.DisposeAll();
			coopMessanger = null;
			coopReceiver = null;
			coopSender = null;
		}

		private void targetIp_TextChanged(object sender, EventArgs e)
		{
			this.targetIp.BackColor = Color.White;
		}

		private void targetIp_Validated(object sender, EventArgs e)
		{
			addressValidated = true;
			this.targetIp.BackColor = Color.White;
			this.addressTooltip.Active = false;
		}
	}
}

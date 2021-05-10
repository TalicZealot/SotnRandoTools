using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop;
using SotnRandoTools.Coop.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class CoopForm : Form
	{
		private readonly IToolConfig toolConfig;
		private readonly CoopSender coopSender;
		private readonly CoopMessanger coopMessanger;
		private readonly CoopReceiver coopReceiver;
		private readonly INotificationService notificationService;
		private CoopViewModel coopViewModel = new CoopViewModel();
		private bool addressValidated = false;

		public CoopForm(IToolConfig toolConfig, IWatchlistService watchlistService, IInputService inputService, IGameApi gameApi, IAlucardApi alucardApi, IJoypadApi joypadApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (joypadApi is null) throw new ArgumentNullException(nameof(joypadApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.notificationService = notificationService;

			this.coopReceiver = new CoopReceiver(toolConfig, gameApi, alucardApi, notificationService, watchlistService);
			this.coopMessanger = new CoopMessanger(toolConfig, coopReceiver, coopViewModel);
			this.coopSender = new CoopSender(toolConfig, watchlistService, inputService, gameApi, alucardApi, coopMessanger);
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			this.portNumeric.Controls[0].Visible = false;
			coopViewModel.PropertyChanged += CoopViewModelPropertyChanged;
		}

		private void CoopViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Message")
			{
				notificationService.AddMessage(coopViewModel.Message);
				return;
			}

			if (!coopViewModel.ClientConnected && !coopViewModel.ServerStarted)
			{
				this.connectButton.Text = "Connect";
				this.hostButton.Text = "Host";
				this.hostButton.Enabled = true;
				this.connectButton.Enabled = true;
				this.targetIp.Enabled = true;
				this.portNumeric.Enabled = true;
			}
			else if (coopViewModel.ClientConnected)
			{
				this.connectButton.Text = "Disconnect";
				this.hostButton.Enabled = false;
				this.targetIp.Enabled = false;
				this.portNumeric.Enabled = false;
			}
			else if (coopViewModel.ServerStarted)
			{
				this.hostButton.Text = "Stop";
				this.connectButton.Enabled = false;
				this.targetIp.Enabled = false;
				this.portNumeric.Enabled = false;
			}
		}

		public void UpdateCoop()
		{
			coopSender.Update();
			coopReceiver.ExecuteMessage();
		}

		private void CoopForm_Load(object sender, EventArgs e)
		{
			this.Location = toolConfig.Coop.Location;
			this.portNumeric.Value = toolConfig.Coop.DefaultPort;
			this.targetIp.Text = toolConfig.Coop.DefaultServer;
			ValidateAddress();
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
			if (coopViewModel.ServerStarted)
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
			if (coopViewModel.ClientConnected)
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
			coopMessanger.Disconnect();
			coopMessanger.StopServer();
			coopMessanger.DisposeAll();
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

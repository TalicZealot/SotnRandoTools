using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class CoopForm : Form
	{
		private readonly IToolConfig toolConfig;
		private readonly CoopSender coopSender;
		private readonly CoopMessanger coopMessanger;
		private readonly CoopReceiver coopReceiver;

		public CoopForm(IToolConfig toolConfig, IWatchlistService watchlistService, IGameApi gameApi, IAlucardApi alucardApi, IJoypadApi joypadApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			this.coopReceiver = new CoopReceiver(toolConfig, gameApi, alucardApi, notificationService, watchlistService);
			this.coopMessanger = new CoopMessanger(toolConfig, coopReceiver);
			this.coopSender = new CoopSender(toolConfig, watchlistService, gameApi, alucardApi, joypadApi, coopMessanger);
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			this.portNumeric.Controls[0].Visible = false;
		}

		public void UpdateCoop()
		{
			coopSender.Update();
		}

		private void CoopForm_Load(object sender, EventArgs e)
		{
			this.Location = toolConfig.Coop.Location;
			this.portNumeric.Value = toolConfig.Coop.DefaultPort;
			this.targetIp.Text = toolConfig.Coop.DefaultServer;
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
			int port = (int) this.portNumeric.Value;

			if (port > 1024 && port < 49151)
			{
				coopMessanger.StartServer(port);
			}
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			string[] hostAddress = this.targetIp.Text.Split(':');
			string[] ip = hostAddress[0].Split('.');
			Regex validIpPort = new Regex(@"^(\d{1,3})\.(\d{1,3})\.(\d{1,3}).(\d{1,3})\:(\d{2,5})$");
			if (!validIpPort.IsMatch(this.targetIp.Text))
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
			Regex validIpPort = new Regex(@"^(\d{1,3})\.(\d{1,3})\.(\d{1,3}).(\d{1,3})\:(\d{2,5})$");
			if (!validIpPort.IsMatch(this.targetIp.Text))
			{
				e.Cancel = true;
			}
		}

		private void CoopForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			coopMessanger.Disconnect();
			coopMessanger.StopServer();
		}

		private void targetIp_TextChanged(object sender, EventArgs e)
		{
			//Until I can get the client connected event to fire.
			if (toolConfig.Coop.StoreLastServer)
			{
				toolConfig.Coop.DefaultServer = targetIp.Text;
			}
		}
	}
}

using System;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class CoopSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;

		public CoopSettingsPanel(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeComponent();
		}

		internal INotificationService NotificationService { get; set; }

		private void MultiplayerSettingsPanel_Load(object sender, EventArgs e)
		{
			portTextBox.Text = toolConfig.Coop.DefaultPort.ToString();
			volumeBar.Value = toolConfig.Coop.Volume;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void portTextBox_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.DefaultPort = Int32.Parse(portTextBox.Text);
		}

		private void volumeBar_Scroll(object sender, EventArgs e)
		{
			toolConfig.Coop.Volume = volumeBar.Value;
			if (NotificationService is not null)
			{
				NotificationService.Volume = (double) volumeBar.Value / 10F;
			}
		}
	}
}

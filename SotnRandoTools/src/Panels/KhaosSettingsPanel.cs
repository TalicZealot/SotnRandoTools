using System;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;

namespace SotnRandoTools
{
	public partial class KhaosSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;
		private BindingSource configBindingSource = new BindingSource();

		public KhaosSettingsPanel(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeComponent();
		}

		private void KhaosSettingsPanel_Load(object sender, EventArgs e)
		{
			alertsCheckbox.Checked = toolConfig.Khaos.Alerts;
			if (toolConfig.Khaos.BotActionsFilePath is not null)
			{
				botCommandsPath.Text = toolConfig.Khaos.BotActionsFilePath;
			}
			//volumeTrackBar.Value = toolConfig.Khaos.Volume;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void botActionFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			botCommandsPath.Text = botActionFileDialog.FileName;
			toolConfig.Khaos.BotActionsFilePath = botActionFileDialog.FileName;
		}

		private void volumeTrackBar_Scroll(object sender, EventArgs e)
		{
			//toolConfig.Khaos.Volume = volumeTrackBar.Value;
		}

		private void botCommandsBrowseButton_Click(object sender, EventArgs e)
		{
			botActionFileDialog.ShowDialog();
		}

		private void alertsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.Alerts = alertsCheckbox.Checked;
		}

		private void botCommandsPath_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BotActionsFilePath = botCommandsPath.Text;
		}
	}
}

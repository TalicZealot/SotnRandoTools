using System;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;

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

		private void MultiplayerSettingsPanel_Load(object sender, EventArgs e)
		{
			sendItemsCheckbox.Checked = toolConfig.Coop.SendItems;
			shareWarpsCheckbox.Checked = toolConfig.Coop.ShareWarps;
			shareShortcutsCheckbox.Checked = toolConfig.Coop.ShareShortcuts;
			sendAssistsCheckbox.Checked = toolConfig.Coop.SendAssists;
			shareLocationsCheckbox.Checked = toolConfig.Coop.ShareLocations;

			saveServerCheckbox.Checked = toolConfig.Coop.StoreLastServer;
			portTextBox.Text = toolConfig.Coop.DefaultPort.ToString();
			serverTextBox.Text = toolConfig.Coop.DefaultServer;

			shareRelicsRadio.Checked = toolConfig.Coop.ShareRelics;
			sendRelicsRadio.Checked = toolConfig.Coop.SendRelics;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void shareRelicsRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (shareRelicsRadio.Checked)
			{
				toolConfig.Coop.ShareRelics = true;
				toolConfig.Coop.SendRelics = false;
			}
			else
			{
				toolConfig.Coop.ShareRelics = false;
				toolConfig.Coop.SendRelics = true;
			}
		}

		private void sendItemsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.SendItems = sendItemsCheckbox.Checked;
		}

		private void shareWarpsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareWarps = shareWarpsCheckbox.Checked;
		}

		private void shareShortcutsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareShortcuts = shareShortcutsCheckbox.Checked;
		}

		private void portTextBox_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.DefaultPort = Int32.Parse(portTextBox.Text);
		}

		private void saveServerCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.StoreLastServer = saveServerCheckbox.Checked;
		}

		private void serverTextBox_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.DefaultServer = serverTextBox.Text;
		}

		private void sendAssistsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.SendAssists  = sendAssistsCheckbox.Checked;
		}

		private void shareLocationsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareLocations = shareLocationsCheckbox.Checked;
		}
	}
}

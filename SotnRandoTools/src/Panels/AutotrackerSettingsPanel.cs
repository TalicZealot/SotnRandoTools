using System;
using System.IO;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools
{
	public partial class AutotrackerSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;
		private BindingSource configBindingSource = new BindingSource();

		public AutotrackerSettingsPanel(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeComponent();
		}

		private void AutotrackerSettingsPanel_Load(object sender, EventArgs e)
		{
			this.configBindingSource.DataSource = toolConfig;

			if (toolConfig.Tracker.ProgressionRelicsOnly)
			{
				radioProgression.Checked = true;
			}
			else
			{
				radioAllRelics.Checked = true;
			}

			if (toolConfig.Tracker.GridLayout)
			{
				radioGrid.Checked = true;
			}
			else
			{
				radioCollected.Checked = true;
			}

			locationsCheckbox.Checked = toolConfig.Tracker.Locations;
			replaysCheckBox.Checked = toolConfig.Tracker.SaveReplays;
			overlayCheckBox.Checked = toolConfig.Tracker.UseOverlay;
			autosplitterCheckBox.Checked = toolConfig.Tracker.EnableAutosplitter;
			muteCheckBox.Checked = toolConfig.Tracker.MuteMusic;
			stereoCheckBox.Checked = toolConfig.Tracker.Stereo;
			alwaysOpTopCheckbox.Checked = toolConfig.Tracker.AlwaysOnTop;

			customExtension.Text = toolConfig.Tracker.CustomExtension;
			customLocationsGuardedRadio.Checked = toolConfig.Tracker.CustomLocationsGuarded;
			customLocationsEquipmentRadio.Checked = toolConfig.Tracker.CustomLocationsEquipment;
			customLocationsClassicRadio.Checked = toolConfig.Tracker.CustomLocationsClassic;
			customLocationsSpreadRadio.Checked = toolConfig.Tracker.CustomLocationsSpread;
			customLocationsCustomExtensionRadio.Checked = toolConfig.Tracker.CustomLocationsCustom;

			username.Text = toolConfig.Tracker.Username;

			openLayoutDialog.InitialDirectory = Directory.GetCurrentDirectory() + Paths.OverlayPath;
			saveLayoutDialog.InitialDirectory = Directory.GetCurrentDirectory() + Paths.OverlayPath;
		}

		private void radioProgression_CheckedChanged(object sender, EventArgs e)
		{
			if (radioProgression.Checked)
			{
				toolConfig.Tracker.ProgressionRelicsOnly = true;
			}
			else
			{
				toolConfig.Tracker.ProgressionRelicsOnly = false;
			}
		}

		private void radioCollected_CheckedChanged(object sender, EventArgs e)
		{
			if (radioCollected.Checked)
			{
				toolConfig.Tracker.GridLayout = false;
			}
			else
			{
				toolConfig.Tracker.GridLayout = true;
			}
		}
		private void locationsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.Locations = locationsCheckbox.Checked;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void trackerDerfaultsButton_Click(object sender, EventArgs e)
		{
			toolConfig.Tracker.Default();
		}

		private void replaysCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.SaveReplays = replaysCheckBox.Checked;
		}

		private void customLocationsGuardedRadio_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomLocationsGuarded = customLocationsGuardedRadio.Checked;
			toolConfig.Tracker.CustomExtension = "guarded";
		}

		private void customLocationsEquipmentRadio_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomLocationsEquipment = customLocationsEquipmentRadio.Checked;
			toolConfig.Tracker.CustomExtension = "equipment";
		}

		private void customLocationsClassicRadio_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomLocationsClassic = customLocationsClassicRadio.Checked;
			toolConfig.Tracker.CustomExtension = "classic";
		}

		private void customLocationsSpreadRadio_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomLocationsSpread = customLocationsSpreadRadio.Checked;
			toolConfig.Tracker.CustomExtension = "spread";
		}

		private void customLocationsCustomExtensionRadio_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomLocationsCustom = customLocationsCustomExtensionRadio.Checked;
			toolConfig.Tracker.CustomExtension = customExtension.Text;
		}

		private void customExtension_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomExtension = customExtension.Text;
		}

		private void username_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.Username = username.Text;
		}

		private void overlayCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.UseOverlay = overlayCheckBox.Checked;
		}

		private void autosplitterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.EnableAutosplitter = autosplitterCheckBox.Checked;
		}

		private void muteCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.MuteMusic = muteCheckBox.Checked;
		}

		private void stereoCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.Stereo = stereoCheckBox.Checked;
		}

		private void alwaysOpTopCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.AlwaysOnTop = alwaysOpTopCheckbox.Checked;
		}

		private void loadLayoutButton_Click(object sender, EventArgs e)
		{
			openLayoutDialog.ShowDialog();
		}

		private void saveLayoutButton_Click(object sender, EventArgs e)
		{
			saveLayoutDialog.ShowDialog();
		}

		private void openLayoutDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				toolConfig.Tracker.LoadOverlayLayout(openLayoutDialog.FileName);
			}
			catch (Exception)
			{
				string message = "Unsupported or outdated file format!";
				string caption = "Unsupported or outdated file format!";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				DialogResult result;

				result = MessageBox.Show(message, caption, buttons);
			}
		}

		private void saveLayoutDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			toolConfig.Tracker.SaveOverlayLayout(saveLayoutDialog.FileName);
		}
	}
}

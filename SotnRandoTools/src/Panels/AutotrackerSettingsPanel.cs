using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services;

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

		internal INotificationService NotificationService { get; set; }

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

			List<string> extensions = Directory.GetFiles(Paths.ExtensionPath).Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
			for (int i = 0; i < extensions.Count; i++)
			{
				customExtensionCombo.Items.Add(extensions[i]);
			}
			int extensionIndex = customExtensionCombo.Items.IndexOf(toolConfig.Tracker.CustomExtension);
			if (extensionIndex < 0)
			{
				extensionIndex = 0;
			}
			customExtensionCombo.SelectedItem = customExtensionCombo.Items[extensionIndex];

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
				if (NotificationService is not null)
				{
					NotificationService.UpdateOverlayLayout();
				}
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

		private void customExtensionCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.CustomExtension = customExtensionCombo.SelectedItem.ToString();
		}
	}
}

using System;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;

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

			alwaysOnTopCheckBox.Checked = toolConfig.Tracker.AlwaysOnTop;
			locationsCheckbox.Checked = toolConfig.Tracker.Locations;
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

		private void alwaysOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.AlwaysOnTop = alwaysOnTopCheckBox.Checked;
		}

		private void locationsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Tracker.Locations = locationsCheckbox.Checked;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}
	}
}

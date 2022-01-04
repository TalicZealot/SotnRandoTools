using System;
using System.Drawing;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class KhaosSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;
		private BindingSource actionSettingsSource = new();

		public KhaosSettingsPanel(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeComponent();
		}
		public INotificationService NotificationService { get; set; }

		private void KhaosSettingsPanel_Load(object sender, EventArgs e)
		{
			alertsCheckbox.Checked = toolConfig.Khaos.Alerts;
			volumeTrackBar.Value = toolConfig.Khaos.Volume;
			crippleTextBox.Text = (toolConfig.Khaos.CrippleFactor * 100) + "%";
			hasteTextBox.Text = (toolConfig.Khaos.HasteFactor * 100) + "%";
			weakenTextBox.Text = (toolConfig.Khaos.WeakenFactor * 100) + "%";
			thirstTextBox.Text = toolConfig.Khaos.ThirstDrainPerSecond.ToString();
			queueTextBox.Text = toolConfig.Khaos.QueueInterval.ToString();
			dynamicIntervalCheckBox.Checked = toolConfig.Khaos.DynamicInterval;
			keepVladRelicsCheckbox.Checked = toolConfig.Khaos.KeepVladRelics;
			costDecayCheckBox.Checked = toolConfig.Khaos.CostDecay;
			pandoraMinTextBox.Text = toolConfig.Khaos.PandoraMinItems.ToString();
			pandoraMaxTextBox.Text = toolConfig.Khaos.PandoraMaxItems.ToString();

			foreach (var action in toolConfig.Khaos.Actions)
			{
				actionSettingsSource.Add(action);
			}
			alertsGridView.AutoGenerateColumns = false;
			alertsGridView.DataSource = actionSettingsSource;
			alertsGridView.CellClick += AlertsGridView_BrowseClick;
			actionsGridView.AutoGenerateColumns = false;
			actionsGridView.DataSource = actionSettingsSource;
			actionCooldownsGridView.AutoGenerateColumns = false;
			actionCooldownsGridView.DataSource = actionSettingsSource;
			actionPricingGridView.AutoGenerateColumns = false;
			actionPricingGridView.DataSource = actionSettingsSource;
			actionPricingGridView.CellValidating += ActionPricingGridView_CellValidating;
		}

		private void ActionPricingGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			string property = actionPricingGridView.Columns[e.ColumnIndex].DataPropertyName;

			if (property.Equals("Scaling"))
			{
				if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText =
						"Scaling must not be empty.";
					e.Cancel = true;
				}
				else if (Convert.ToDouble(e.FormattedValue) < 1)
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText =
						"Scaling cannot be lower than 1.";
					e.Cancel = true;
				}
				else if (Convert.ToDouble(e.FormattedValue) > 10)
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText =
						"Scaling cannot be higher than 10.";
					e.Cancel = true;
				}
				else
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText = "";
					e.Cancel = false;
				}
			}
			else if (property.Equals("MaximumChannelPoints"))
			{
				if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText =
						"Maximum Channel Points must not be empty.";
					e.Cancel = true;
				}
				else if (Convert.ToUInt32(e.FormattedValue) <= toolConfig.Khaos.Actions[e.RowIndex].ChannelPoints && Convert.ToUInt32(e.FormattedValue) != 0)
				{
					actionPricingGridView.Rows[e.RowIndex].ErrorText =
						"Maximum Channel Points must be higher than Channel Points or 0(uncapped).";
					e.Cancel = true;
				}
			}
		}

		private void AlertsGridView_BrowseClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex !=
			alertsGridView.Columns["Browse"].Index) return;
			alertFileDialog.Tag = e.RowIndex;
			alertFileDialog.ShowDialog();
		}
		private void alertFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			alertsGridView.Rows[(int) alertFileDialog.Tag].Cells[1].Value = alertFileDialog.FileName;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void volumeTrackBar_Scroll(object sender, EventArgs e)
		{
			toolConfig.Khaos.Volume = volumeTrackBar.Value;
			if (NotificationService is not null)
			{
				NotificationService.Volume = (double) volumeTrackBar.Value / 10F;
			}
		}

		private void alertsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.Alerts = alertsCheckbox.Checked;
		}

		private void crippleTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = crippleTextBox.Text.Replace("%", "");
			int cripplePercentage;
			bool result = Int32.TryParse(boxText, out cripplePercentage);
			if (result)
			{
				toolConfig.Khaos.CrippleFactor = (cripplePercentage / 100F);
			}
			crippleTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void crippleTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//if (this.ActiveControl.Equals(sender))
			//return;
			string boxText = crippleTextBox.Text.Replace("%", "");
			int cripplePercentage;
			bool result = Int32.TryParse(boxText, out cripplePercentage);
			if (!result || cripplePercentage < 0 || cripplePercentage > 90)
			{
				this.crippleTextBox.Text = "";
				this.crippleTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(crippleTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void hasteTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = hasteTextBox.Text.Replace("%", "");
			int hastePercentage;
			bool result = Int32.TryParse(boxText, out hastePercentage);
			if (result)
			{
				toolConfig.Khaos.HasteFactor = (hastePercentage / 100F);
			}
			hasteTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void hasteTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = hasteTextBox.Text.Replace("%", "");
			int hastePercentage;
			bool result = Int32.TryParse(boxText, out hastePercentage);
			if (!result || hastePercentage < 100 || hastePercentage > 1000)
			{
				this.hasteTextBox.Text = "";
				this.hasteTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(hasteTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void weakenTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = weakenTextBox.Text.Replace("%", "");
			int weakenPercentage;
			bool result = Int32.TryParse(boxText, out weakenPercentage);
			if (result)
			{
				toolConfig.Khaos.WeakenFactor = (weakenPercentage / 100F);
			}
			weakenTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void weakenTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = weakenTextBox.Text.Replace("%", "");
			int weakenPercentage;
			bool result = Int32.TryParse(boxText, out weakenPercentage);
			if (!result || weakenPercentage < 10 || weakenPercentage > 90)
			{
				this.weakenTextBox.Text = "";
				this.weakenTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(weakenTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void thirstTextBox_Validated(object sender, EventArgs e)
		{
			int thirstDrain;
			bool result = Int32.TryParse(thirstTextBox.Text, out thirstDrain);
			if (result)
			{
				toolConfig.Khaos.ThirstDrainPerSecond = (uint) thirstDrain;
			}
			thirstTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void thirstTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int thirstDrain;
			bool result = Int32.TryParse(thirstTextBox.Text, out thirstDrain);
			if (!result || thirstDrain < 1 || thirstDrain > 100)
			{
				this.thirstTextBox.Text = "";
				this.thirstTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(thirstTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandoraMinTextBox_Validated(object sender, EventArgs e)
		{
			int pandoraMinItems;
			bool result = Int32.TryParse(pandoraMinTextBox.Text, out pandoraMinItems);
			if (result)
			{
				toolConfig.Khaos.PandoraMinItems = pandoraMinItems;
			}
			pandoraMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandoraMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int pandoraMinItems;
			bool result = Int32.TryParse(pandoraMinTextBox.Text, out pandoraMinItems);
			if (!result || pandoraMinItems < 0 || pandoraMinItems > 100)
			{
				this.pandoraMinTextBox.Text = "";
				this.pandoraMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(pandoraMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandoraMaxTextBox_Validated(object sender, EventArgs e)
		{
			int pandoraMaxItems;
			bool result = Int32.TryParse(pandoraMaxTextBox.Text, out pandoraMaxItems);
			if (result)
			{
				toolConfig.Khaos.PandoraMaxItems = pandoraMaxItems;
			}
			pandoraMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandoraMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int pandoraMaxItems;
			bool result = Int32.TryParse(pandoraMaxTextBox.Text, out pandoraMaxItems);
			if (!result || pandoraMaxItems < 1 || pandoraMaxItems > 100)
			{
				this.pandoraMaxTextBox.Text = "";
				this.pandoraMaxTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(pandoraMaxTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void queueTextBox_Validated(object sender, EventArgs e)
		{
			TimeSpan queueInterval;
			bool result = TimeSpan.TryParse(queueTextBox.Text, out queueInterval);
			if (result)
			{
				toolConfig.Khaos.QueueInterval = queueInterval;
			}
			queueTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void queueTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TimeSpan queueInterval;
			TimeSpan minSpan = new TimeSpan(0, 0, 10);
			TimeSpan maxSpan = new TimeSpan(0, 10, 0);
			bool result = TimeSpan.TryParse(queueTextBox.Text, out queueInterval);
			if (!result)
			{
				this.queueTextBox.Text = "";
				this.queueTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(queueTextBox, "Invalid value! Format: (hh:mm:ss)");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
			if (queueInterval < minSpan || queueInterval > maxSpan)
			{
				this.queueTextBox.Text = "";
				this.queueTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(queueTextBox, "Value must be greater than 10 seconds and lower than 10 minutes!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void dynamicIntervalCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.DynamicInterval = dynamicIntervalCheckBox.Checked;
		}

		private void keepVladRelicsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.KeepVladRelics = keepVladRelicsCheckbox.Checked;
		}

		private void costDecayCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.CostDecay = costDecayCheckBox.Checked;
		}
	}
}

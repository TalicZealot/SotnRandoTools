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
		private readonly INotificationService notificationService;
		private BindingSource actionsAlertsSource = new();
		private BindingSource actionsOtherSource = new();

		public KhaosSettingsPanel(IToolConfig toolConfig, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.notificationService = notificationService;

			InitializeComponent();
		}

		private void KhaosSettingsPanel_Load(object sender, EventArgs e)
		{
			alertsCheckbox.Checked = toolConfig.Khaos.Alerts;
			namesPath.Text = toolConfig.Khaos.NamesFilePath;
			botApiKey.Text = toolConfig.Khaos.BotApiKey;
			volumeTrackBar.Value = toolConfig.Khaos.Volume;
			crippleTextBox.Text = (toolConfig.Khaos.CrippleFactor * 100) + "%";
			hasteTextBox.Text = (toolConfig.Khaos.HasteFactor * 100) + "%";
			weakenTextBox.Text = (toolConfig.Khaos.WeakenFactor * 100) + "%";
			thirstTextBox.Text = toolConfig.Khaos.ThirstDrainPerSecond.ToString();
			queueTextBox.Text = toolConfig.Khaos.QueueInterval.ToString();
			dynamicIntervalCheckBox.Checked = toolConfig.Khaos.DynamicInterval;
			keepVladRelicsCheckbox.Checked = toolConfig.Khaos.KeepVladRelics;
			pandoraMinTextBox.Text = toolConfig.Khaos.PandoraMinItems.ToString();
			pandoraMaxTextBox.Text = toolConfig.Khaos.PandoraMaxItems.ToString();

			foreach (var action in toolConfig.Khaos.Actions)
			{
				actionsAlertsSource.Add(action);
				actionsOtherSource.Add(action);
			}
			alertsGridView.AutoGenerateColumns = false;
			alertsGridView.DataSource = actionsAlertsSource;
			alertsGridView.CellClick += AlertsGridView_BrowseClick;
			actionsGridView.AutoGenerateColumns = false;
			actionsGridView.DataSource = actionsOtherSource;
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
			notificationService.Volume = (double) volumeTrackBar.Value / 10F;
		}

		private void alertsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.Alerts = alertsCheckbox.Checked;
		}

		private void namesBrowseButton_Click(object sender, EventArgs e)
		{
			namesFileDialog.ShowDialog();
		}

		private void namesFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			namesPath.Text = namesFileDialog.FileName;
			toolConfig.Khaos.NamesFilePath = namesFileDialog.FileName;
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

		private void botApiKey_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BotApiKey = botApiKey.Text;
		}

		private void keepVladRelicsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.KeepVladRelics = keepVladRelicsCheckbox.Checked;
		}
	}
}

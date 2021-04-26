using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;
using System.Linq;

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
			if (toolConfig.Khaos.BotActionsFilePath is not null)
			{
				botCommandsPath.Text = toolConfig.Khaos.BotActionsFilePath;
			}
			if (toolConfig.Khaos.NamesFilePath is not null)
			{
				namesPath.Text = toolConfig.Khaos.NamesFilePath;
			}
			volumeTrackBar.Value = toolConfig.Khaos.Volume;
			crippleTextBox.Text = (toolConfig.Khaos.CrippleFactor * 100) + "%";
			hasteTextBox.Text = (toolConfig.Khaos.HasteFactor * 100) + "%";
			weakenTextBox.Text = (toolConfig.Khaos.WeakenFactor * 100) + "%";
			thirstTextBox.Text = toolConfig.Khaos.ThirstDrainPerSecond.ToString();
			queueTextBox.Text = toolConfig.Khaos.QueueInterval.ToString();
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
			alertsGridView.Rows[(int)alertFileDialog.Tag].Cells[1].Value = alertFileDialog.FileName;
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
			toolConfig.Khaos.Volume = volumeTrackBar.Value;
			notificationService.Volume = (double) volumeTrackBar.Value / 10F;
		}

		private void alertsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.Alerts = alertsCheckbox.Checked;
		}

		private void botCommandsBrowseButton_Click(object sender, EventArgs e)
		{
			botActionFileDialog.ShowDialog();
		}

		private void botCommandsPath_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BotActionsFilePath = botCommandsPath.Text;
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
			if (!result)
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
			if (!result)
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
			if (!result)
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
				toolConfig.Khaos.ThirstDrainPerSecond = (uint)thirstDrain;
			}
			thirstTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void thirstTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int thirstDrain;
			bool result = Int32.TryParse(thirstTextBox.Text, out thirstDrain);
			if (!result)
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
			if (!result && pandoraMinItems > 0)
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
			if (!result && pandoraMaxItems < 257)
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
		}
	}
}

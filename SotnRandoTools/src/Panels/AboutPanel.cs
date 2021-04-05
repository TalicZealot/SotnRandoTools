using System;
using System.ComponentModel;
using System.Windows.Forms;
using SotnRandoTools.Utils;

namespace SotnRandoTools
{
	public partial class AboutPanel : UserControl
	{
		[Browsable(true)]
		[Category("Action")]
		[Description("Invoked when user clicks the update button")]
		public event EventHandler UpdateButton_Click;

		public AboutPanel()
		{
			InitializeComponent();
		}

		private async void AboutPanel_Load(object sender, EventArgs e)
		{
			string currentVersion = typeof(AboutPanel).Assembly.GetName().Version.ToString().Substring(0, 5);
			this.versionLabel.Text = "Version " + currentVersion;

			if (await WebRequests.NewReleaseAvaiable(currentVersion))
			{
				updateButton.Visible = true;
			}
			else
			{
				this.versionLabel.Text += " ✔️";
				this.upToDateToolTip.SetToolTip(versionLabel, "Up to date!");
			}
		}

		private void updateButton_Click(object sender, EventArgs e)
		{
			if (this.UpdateButton_Click != null)
				this.UpdateButton_Click(this, e);
		}
	}
}

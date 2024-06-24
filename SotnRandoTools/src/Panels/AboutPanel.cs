﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SotnRandoTools.Constants;
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
				this.versionLabel.Text += " ❌";
				this.upToDateToolTip.SetToolTip(versionLabel, "New version available!");
				this.upToDateToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.versionLabel.ForeColor = Color.PaleVioletRed;
			}
			else
			{
				patchNotesLink.Visible = true;
				this.versionLabel.Text += " ✔️";
				this.upToDateToolTip.SetToolTip(versionLabel, "Up to date!");
				this.upToDateToolTip.ToolTipIcon = ToolTipIcon.None;
				this.versionLabel.ForeColor = Color.SpringGreen;
			}
		}

		private void updateButton_Click(object sender, EventArgs e)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (this.UpdateButton_Click != null)
					this.UpdateButton_Click(this, e);
			}
			else
			{
				Process.Start(Paths.LatestReleaseUrl);
			}
		}

		private void sotnApiLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			sotnApiLink.LinkVisited = true;
			Process.Start(Paths.ApiLink);
		}

		private void updaterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			updaterLink.LinkVisited = true;
			Process.Start(Paths.UpdaterLink);
		}

		private void randoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			randoLink.LinkVisited = true;
			Process.Start(Paths.RandoSourceLink);
		}

		private void readmeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			readmeLink.LinkVisited = true;
			Process.Start(Paths.ReadmeLink);
		}

		private void sourceLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			sourceLink.LinkVisited = true;
			Process.Start(Paths.SourceLink);
		}

		private void patchNotesLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string path = Directory.GetCurrentDirectory();
			patchNotesLink.LinkVisited = true;
			Process.Start(path + Paths.ChangeLogPath);
		}
	}
}

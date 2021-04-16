
namespace SotnRandoTools
{
	partial class AboutPanel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.autotrackerPanelTitle = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.upToDateToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.readmeLink = new System.Windows.Forms.LinkLabel();
            this.sourceLink = new System.Windows.Forms.LinkLabel();
            this.donateLink = new System.Windows.Forms.LinkLabel();
            this.sotnApiLink = new System.Windows.Forms.LinkLabel();
            this.updaterLink = new System.Windows.Forms.LinkLabel();
            this.randoLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // autotrackerPanelTitle
            // 
            this.autotrackerPanelTitle.AutoSize = true;
            this.autotrackerPanelTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autotrackerPanelTitle.Location = new System.Drawing.Point(1, 0);
            this.autotrackerPanelTitle.Name = "autotrackerPanelTitle";
            this.autotrackerPanelTitle.Size = new System.Drawing.Size(74, 29);
            this.autotrackerPanelTitle.TabIndex = 0;
            this.autotrackerPanelTitle.Text = "About";
            // 
            // divider
            // 
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider.Location = new System.Drawing.Point(6, 38);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(382, 2);
            this.divider.TabIndex = 1;
            // 
            // versionLabel
            // 
            this.versionLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.versionLabel.Location = new System.Drawing.Point(6, 51);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(382, 19);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "Version";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // updateButton
            // 
            this.updateButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.updateButton.FlatAppearance.BorderSize = 2;
            this.updateButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.updateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.updateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.updateButton.ForeColor = System.Drawing.Color.OrangeRed;
            this.updateButton.Location = new System.Drawing.Point(6, 83);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(382, 54);
            this.updateButton.TabIndex = 4;
            this.updateButton.Text = "Update Now";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Visible = false;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.descriptionLabel.Location = new System.Drawing.Point(7, 149);
            this.descriptionLabel.MaximumSize = new System.Drawing.Size(400, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(365, 65);
            this.descriptionLabel.TabIndex = 5;
            this.descriptionLabel.Text = "SotN Randomizer Tools is a free and open source set of tools to enhance the SotN " +
    "randomizer experience.\r\nMade by TalicZealot.\r\n\r\nAssociated Projects:";
            // 
            // readmeLink
            // 
            this.readmeLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.readmeLink.AutoSize = true;
            this.readmeLink.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.readmeLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.readmeLink.Location = new System.Drawing.Point(7, 343);
            this.readmeLink.Name = "readmeLink";
            this.readmeLink.Size = new System.Drawing.Size(68, 18);
            this.readmeLink.TabIndex = 6;
            this.readmeLink.TabStop = true;
            this.readmeLink.Text = "Readme";
            this.readmeLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.readmeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.readmeLink_LinkClicked);
            // 
            // sourceLink
            // 
            this.sourceLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.sourceLink.AutoSize = true;
            this.sourceLink.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sourceLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.sourceLink.Location = new System.Drawing.Point(171, 343);
            this.sourceLink.Name = "sourceLink";
            this.sourceLink.Size = new System.Drawing.Size(60, 18);
            this.sourceLink.TabIndex = 7;
            this.sourceLink.TabStop = true;
            this.sourceLink.Text = "Source";
            this.sourceLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.sourceLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.sourceLink_LinkClicked);
            // 
            // donateLink
            // 
            this.donateLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.donateLink.AutoSize = true;
            this.donateLink.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.donateLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.donateLink.Location = new System.Drawing.Point(324, 343);
            this.donateLink.Name = "donateLink";
            this.donateLink.Size = new System.Drawing.Size(61, 18);
            this.donateLink.TabIndex = 8;
            this.donateLink.TabStop = true;
            this.donateLink.Text = "Donate";
            this.donateLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.donateLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.donateLink_LinkClicked);
            // 
            // sotnApiLink
            // 
            this.sotnApiLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.sotnApiLink.AutoSize = true;
            this.sotnApiLink.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sotnApiLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.sotnApiLink.Location = new System.Drawing.Point(18, 224);
            this.sotnApiLink.Name = "sotnApiLink";
            this.sotnApiLink.Size = new System.Drawing.Size(57, 14);
            this.sotnApiLink.TabIndex = 9;
            this.sotnApiLink.TabStop = true;
            this.sotnApiLink.Text = "SotnApi";
            this.sotnApiLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.sotnApiLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.sotnApiLink_LinkClicked);
            // 
            // updaterLink
            // 
            this.updaterLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.updaterLink.AutoSize = true;
            this.updaterLink.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.updaterLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.updaterLink.Location = new System.Drawing.Point(18, 247);
            this.updaterLink.Name = "updaterLink";
            this.updaterLink.Size = new System.Drawing.Size(181, 14);
            this.updaterLink.TabIndex = 10;
            this.updaterLink.TabStop = true;
            this.updaterLink.Text = "SimpleLatestReleaseUpdater";
            this.updaterLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.updaterLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.updaterLink_LinkClicked);
            // 
            // randoLink
            // 
            this.randoLink.ActiveLinkColor = System.Drawing.Color.Maroon;
            this.randoLink.AutoSize = true;
            this.randoLink.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.randoLink.LinkColor = System.Drawing.Color.DodgerBlue;
            this.randoLink.Location = new System.Drawing.Point(18, 271);
            this.randoLink.Name = "randoLink";
            this.randoLink.Size = new System.Drawing.Size(113, 14);
            this.randoLink.TabIndex = 11;
            this.randoLink.TabStop = true;
            this.randoLink.Text = "SotN Randomizer";
            this.randoLink.VisitedLinkColor = System.Drawing.Color.Cyan;
            this.randoLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.randoLink_LinkClicked);
            // 
            // AboutPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.randoLink);
            this.Controls.Add(this.updaterLink);
            this.Controls.Add(this.sotnApiLink);
            this.Controls.Add(this.donateLink);
            this.Controls.Add(this.sourceLink);
            this.Controls.Add(this.readmeLink);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.autotrackerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "AboutPanel";
            this.Size = new System.Drawing.Size(395, 368);
            this.Load += new System.EventHandler(this.AboutPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label autotrackerPanelTitle;
		private System.Windows.Forms.Label divider;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Button updateButton;
		private System.Windows.Forms.ToolTip upToDateToolTip;
		private System.Windows.Forms.Label descriptionLabel;
		private System.Windows.Forms.LinkLabel readmeLink;
		private System.Windows.Forms.LinkLabel sourceLink;
		private System.Windows.Forms.LinkLabel donateLink;
		private System.Windows.Forms.LinkLabel sotnApiLink;
		private System.Windows.Forms.LinkLabel updaterLink;
		private System.Windows.Forms.LinkLabel randoLink;
	}
}

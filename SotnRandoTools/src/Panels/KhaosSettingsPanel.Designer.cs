
namespace SotnRandoTools
{
    partial class KhaosSettingsPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.khaosPanelTitle = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.khaosTabs = new System.Windows.Forms.TabControl();
            this.prefsTab = new System.Windows.Forms.TabPage();
            this.audioBox = new System.Windows.Forms.GroupBox();
            this.volumeLabel = new System.Windows.Forms.Label();
            this.volumeTrackBar = new System.Windows.Forms.TrackBar();
            this.alertsCheckbox = new System.Windows.Forms.CheckBox();
            this.botInputGroup = new System.Windows.Forms.GroupBox();
            this.botCommandsBrowseButton = new System.Windows.Forms.Button();
            this.botCommandsPath = new System.Windows.Forms.TextBox();
            this.botCommandLabel = new System.Windows.Forms.Label();
            this.pricingBuffsTab = new System.Windows.Forms.TabPage();
            this.actionPricesGridView = new System.Windows.Forms.DataGridView();
            this.actionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.channelPoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.donation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.botActionFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.khaosTabs.SuspendLayout();
            this.prefsTab.SuspendLayout();
            this.audioBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
            this.botInputGroup.SuspendLayout();
            this.pricingBuffsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionPricesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // khaosPanelTitle
            // 
            this.khaosPanelTitle.AutoSize = true;
            this.khaosPanelTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.khaosPanelTitle.Location = new System.Drawing.Point(1, 0);
            this.khaosPanelTitle.Name = "khaosPanelTitle";
            this.khaosPanelTitle.Size = new System.Drawing.Size(169, 29);
            this.khaosPanelTitle.TabIndex = 0;
            this.khaosPanelTitle.Text = "Khaos Settings";
            // 
            // divider
            // 
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider.Location = new System.Drawing.Point(6, 38);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(382, 2);
            this.divider.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.saveButton.FlatAppearance.BorderSize = 2;
            this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveButton.Location = new System.Drawing.Point(304, 340);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(84, 25);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // khaosTabs
            // 
            this.khaosTabs.Controls.Add(this.prefsTab);
            this.khaosTabs.Controls.Add(this.pricingBuffsTab);
            this.khaosTabs.Location = new System.Drawing.Point(6, 43);
            this.khaosTabs.Name = "khaosTabs";
            this.khaosTabs.SelectedIndex = 0;
            this.khaosTabs.Size = new System.Drawing.Size(382, 291);
            this.khaosTabs.TabIndex = 3;
            // 
            // prefsTab
            // 
            this.prefsTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.prefsTab.Controls.Add(this.audioBox);
            this.prefsTab.Controls.Add(this.botInputGroup);
            this.prefsTab.ForeColor = System.Drawing.Color.White;
            this.prefsTab.Location = new System.Drawing.Point(4, 22);
            this.prefsTab.Name = "prefsTab";
            this.prefsTab.Padding = new System.Windows.Forms.Padding(3);
            this.prefsTab.Size = new System.Drawing.Size(374, 265);
            this.prefsTab.TabIndex = 0;
            this.prefsTab.Text = "Preferences";
            // 
            // audioBox
            // 
            this.audioBox.Controls.Add(this.volumeLabel);
            this.audioBox.Controls.Add(this.volumeTrackBar);
            this.audioBox.Controls.Add(this.alertsCheckbox);
            this.audioBox.ForeColor = System.Drawing.Color.White;
            this.audioBox.Location = new System.Drawing.Point(6, 118);
            this.audioBox.Name = "audioBox";
            this.audioBox.Size = new System.Drawing.Size(361, 92);
            this.audioBox.TabIndex = 1;
            this.audioBox.TabStop = false;
            this.audioBox.Text = "Sound";
            // 
            // volumeLabel
            // 
            this.volumeLabel.AutoSize = true;
            this.volumeLabel.Location = new System.Drawing.Point(6, 46);
            this.volumeLabel.Name = "volumeLabel";
            this.volumeLabel.Size = new System.Drawing.Size(41, 13);
            this.volumeLabel.TabIndex = 2;
            this.volumeLabel.Text = "Volume";
            // 
            // volumeTrackBar
            // 
            this.volumeTrackBar.Location = new System.Drawing.Point(53, 43);
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new System.Drawing.Size(302, 45);
            this.volumeTrackBar.TabIndex = 1;
            this.volumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.volumeTrackBar.Value = 10;
            this.volumeTrackBar.Scroll += new System.EventHandler(this.volumeTrackBar_Scroll);
            // 
            // alertsCheckbox
            // 
            this.alertsCheckbox.AutoSize = true;
            this.alertsCheckbox.Location = new System.Drawing.Point(9, 20);
            this.alertsCheckbox.Name = "alertsCheckbox";
            this.alertsCheckbox.Size = new System.Drawing.Size(219, 17);
            this.alertsCheckbox.TabIndex = 0;
            this.alertsCheckbox.Text = "Enable audio alerts on reward activation";
            this.alertsCheckbox.UseVisualStyleBackColor = true;
            this.alertsCheckbox.CheckedChanged += new System.EventHandler(this.alertsCheckbox_CheckedChanged);
            // 
            // botInputGroup
            // 
            this.botInputGroup.Controls.Add(this.botCommandsBrowseButton);
            this.botInputGroup.Controls.Add(this.botCommandsPath);
            this.botInputGroup.Controls.Add(this.botCommandLabel);
            this.botInputGroup.ForeColor = System.Drawing.Color.White;
            this.botInputGroup.Location = new System.Drawing.Point(6, 40);
            this.botInputGroup.Name = "botInputGroup";
            this.botInputGroup.Size = new System.Drawing.Size(362, 57);
            this.botInputGroup.TabIndex = 0;
            this.botInputGroup.TabStop = false;
            this.botInputGroup.Text = "Bot Input";
            // 
            // botCommandsBrowseButton
            // 
            this.botCommandsBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.botCommandsBrowseButton.Location = new System.Drawing.Point(288, 19);
            this.botCommandsBrowseButton.Name = "botCommandsBrowseButton";
            this.botCommandsBrowseButton.Size = new System.Drawing.Size(68, 21);
            this.botCommandsBrowseButton.TabIndex = 2;
            this.botCommandsBrowseButton.Text = "Browse...";
            this.botCommandsBrowseButton.UseVisualStyleBackColor = true;
            this.botCommandsBrowseButton.Click += new System.EventHandler(this.botCommandsBrowseButton_Click);
            // 
            // botCommandsPath
            // 
            this.botCommandsPath.Location = new System.Drawing.Point(93, 20);
            this.botCommandsPath.Name = "botCommandsPath";
            this.botCommandsPath.Size = new System.Drawing.Size(189, 21);
            this.botCommandsPath.TabIndex = 1;
            this.botCommandsPath.TextChanged += new System.EventHandler(this.botCommandsPath_TextChanged);
            // 
            // botCommandLabel
            // 
            this.botCommandLabel.AutoSize = true;
            this.botCommandLabel.Location = new System.Drawing.Point(6, 23);
            this.botCommandLabel.Name = "botCommandLabel";
            this.botCommandLabel.Size = new System.Drawing.Size(81, 13);
            this.botCommandLabel.TabIndex = 0;
            this.botCommandLabel.Text = "Bot actions file:";
            // 
            // pricingBuffsTab
            // 
            this.pricingBuffsTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.pricingBuffsTab.Controls.Add(this.actionPricesGridView);
            this.pricingBuffsTab.ForeColor = System.Drawing.Color.White;
            this.pricingBuffsTab.Location = new System.Drawing.Point(4, 22);
            this.pricingBuffsTab.Name = "pricingBuffsTab";
            this.pricingBuffsTab.Padding = new System.Windows.Forms.Padding(3);
            this.pricingBuffsTab.Size = new System.Drawing.Size(374, 265);
            this.pricingBuffsTab.TabIndex = 1;
            this.pricingBuffsTab.Text = "Pricing: Buffs";
            // 
            // actionPricesGridView
            // 
            this.actionPricesGridView.AllowUserToAddRows = false;
            this.actionPricesGridView.AllowUserToDeleteRows = false;
            this.actionPricesGridView.AllowUserToResizeColumns = false;
            this.actionPricesGridView.AllowUserToResizeRows = false;
            this.actionPricesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.actionPricesGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.actionPricesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.actionPricesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.actionPricesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.actionName,
            this.channelPoints,
            this.bits,
            this.donation});
            this.actionPricesGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionPricesGridView.Location = new System.Drawing.Point(3, 3);
            this.actionPricesGridView.Name = "actionPricesGridView";
            this.actionPricesGridView.RowHeadersVisible = false;
            this.actionPricesGridView.Size = new System.Drawing.Size(368, 259);
            this.actionPricesGridView.TabIndex = 8;
            // 
            // actionName
            // 
            this.actionName.HeaderText = "Name";
            this.actionName.Name = "actionName";
            this.actionName.ReadOnly = true;
            this.actionName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // channelPoints
            // 
            this.channelPoints.HeaderText = "Channel Points";
            this.channelPoints.Name = "channelPoints";
            this.channelPoints.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // bits
            // 
            this.bits.HeaderText = "Bits";
            this.bits.Name = "bits";
            this.bits.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // donation
            // 
            this.donation.HeaderText = "Donation";
            this.donation.Name = "donation";
            this.donation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // botActionFileDialog
            // 
            this.botActionFileDialog.FileName = "actions.txt";
            this.botActionFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.botActionFileDialog_FileOk);
            // 
            // KhaosSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.khaosTabs);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.khaosPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "KhaosSettingsPanel";
            this.Size = new System.Drawing.Size(395, 368);
            this.Load += new System.EventHandler(this.KhaosSettingsPanel_Load);
            this.khaosTabs.ResumeLayout(false);
            this.prefsTab.ResumeLayout(false);
            this.audioBox.ResumeLayout(false);
            this.audioBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
            this.botInputGroup.ResumeLayout(false);
            this.botInputGroup.PerformLayout();
            this.pricingBuffsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.actionPricesGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label khaosPanelTitle;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.TabControl khaosTabs;
		private System.Windows.Forms.TabPage prefsTab;
		private System.Windows.Forms.TabPage pricingBuffsTab;
		private System.Windows.Forms.GroupBox botInputGroup;
		private System.Windows.Forms.Label botCommandLabel;
		private System.Windows.Forms.Button botCommandsBrowseButton;
		private System.Windows.Forms.TextBox botCommandsPath;
		private System.Windows.Forms.GroupBox audioBox;
		private System.Windows.Forms.CheckBox alertsCheckbox;
		private System.Windows.Forms.Label volumeLabel;
		private System.Windows.Forms.TrackBar volumeTrackBar;
		private System.Windows.Forms.OpenFileDialog botActionFileDialog;
		private System.Windows.Forms.DataGridView actionPricesGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn actionName;
		private System.Windows.Forms.DataGridViewTextBoxColumn channelPoints;
		private System.Windows.Forms.DataGridViewTextBoxColumn bits;
		private System.Windows.Forms.DataGridViewTextBoxColumn donation;
	}
}

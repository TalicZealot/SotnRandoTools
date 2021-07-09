
namespace SotnRandoTools
{
    partial class CoopSettingsPanel
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
            this.multiplayerPanelTitle = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.optionsBox = new System.Windows.Forms.GroupBox();
            this.sendRelicsCheckbox = new System.Windows.Forms.CheckBox();
            this.shareLocationsCheckbox = new System.Windows.Forms.CheckBox();
            this.sendAssistsCheckbox = new System.Windows.Forms.CheckBox();
            this.shareWarpsCheckbox = new System.Windows.Forms.CheckBox();
            this.sendItemsCheckbox = new System.Windows.Forms.CheckBox();
            this.connectionGroup = new System.Windows.Forms.GroupBox();
            this.saveServerCheckbox = new System.Windows.Forms.CheckBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.optionsBox.SuspendLayout();
            this.connectionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplayerPanelTitle
            // 
            this.multiplayerPanelTitle.AutoSize = true;
            this.multiplayerPanelTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.multiplayerPanelTitle.Location = new System.Drawing.Point(1, 0);
            this.multiplayerPanelTitle.Name = "multiplayerPanelTitle";
            this.multiplayerPanelTitle.Size = new System.Drawing.Size(167, 29);
            this.multiplayerPanelTitle.TabIndex = 0;
            this.multiplayerPanelTitle.Text = "Co-op Settings";
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
            // optionsBox
            // 
            this.optionsBox.Controls.Add(this.sendRelicsCheckbox);
            this.optionsBox.Controls.Add(this.shareLocationsCheckbox);
            this.optionsBox.Controls.Add(this.sendAssistsCheckbox);
            this.optionsBox.Controls.Add(this.shareWarpsCheckbox);
            this.optionsBox.Controls.Add(this.sendItemsCheckbox);
            this.optionsBox.ForeColor = System.Drawing.Color.White;
            this.optionsBox.Location = new System.Drawing.Point(6, 56);
            this.optionsBox.Name = "optionsBox";
            this.optionsBox.Size = new System.Drawing.Size(182, 141);
            this.optionsBox.TabIndex = 4;
            this.optionsBox.TabStop = false;
            this.optionsBox.Text = "Options";
            // 
            // sendRelicsCheckbox
            // 
            this.sendRelicsCheckbox.AutoSize = true;
            this.sendRelicsCheckbox.Location = new System.Drawing.Point(5, 20);
            this.sendRelicsCheckbox.Name = "sendRelicsCheckbox";
            this.sendRelicsCheckbox.Size = new System.Drawing.Size(81, 17);
            this.sendRelicsCheckbox.TabIndex = 5;
            this.sendRelicsCheckbox.Text = "Share relics";
            this.sendRelicsCheckbox.UseVisualStyleBackColor = true;
            this.sendRelicsCheckbox.CheckedChanged += new System.EventHandler(this.sendRelicsCheckbox_CheckedChanged);
            // 
            // shareLocationsCheckbox
            // 
            this.shareLocationsCheckbox.AutoSize = true;
            this.shareLocationsCheckbox.Location = new System.Drawing.Point(5, 112);
            this.shareLocationsCheckbox.Name = "shareLocationsCheckbox";
            this.shareLocationsCheckbox.Size = new System.Drawing.Size(99, 17);
            this.shareLocationsCheckbox.TabIndex = 4;
            this.shareLocationsCheckbox.Text = "Share locations";
            this.shareLocationsCheckbox.UseVisualStyleBackColor = true;
            this.shareLocationsCheckbox.CheckedChanged += new System.EventHandler(this.shareLocationsCheckbox_CheckedChanged);
            // 
            // sendAssistsCheckbox
            // 
            this.sendAssistsCheckbox.AutoSize = true;
            this.sendAssistsCheckbox.Location = new System.Drawing.Point(5, 89);
            this.sendAssistsCheckbox.Name = "sendAssistsCheckbox";
            this.sendAssistsCheckbox.Size = new System.Drawing.Size(85, 17);
            this.sendAssistsCheckbox.TabIndex = 3;
            this.sendAssistsCheckbox.Text = "Send assists";
            this.sendAssistsCheckbox.UseVisualStyleBackColor = true;
            this.sendAssistsCheckbox.CheckedChanged += new System.EventHandler(this.sendAssistsCheckbox_CheckedChanged);
            // 
            // shareWarpsCheckbox
            // 
            this.shareWarpsCheckbox.AutoSize = true;
            this.shareWarpsCheckbox.Location = new System.Drawing.Point(5, 66);
            this.shareWarpsCheckbox.Name = "shareWarpsCheckbox";
            this.shareWarpsCheckbox.Size = new System.Drawing.Size(86, 17);
            this.shareWarpsCheckbox.TabIndex = 1;
            this.shareWarpsCheckbox.Text = "Share warps";
            this.shareWarpsCheckbox.UseVisualStyleBackColor = true;
            this.shareWarpsCheckbox.CheckedChanged += new System.EventHandler(this.shareWarpsCheckbox_CheckedChanged);
            // 
            // sendItemsCheckbox
            // 
            this.sendItemsCheckbox.AutoSize = true;
            this.sendItemsCheckbox.Location = new System.Drawing.Point(5, 43);
            this.sendItemsCheckbox.Name = "sendItemsCheckbox";
            this.sendItemsCheckbox.Size = new System.Drawing.Size(78, 17);
            this.sendItemsCheckbox.TabIndex = 0;
            this.sendItemsCheckbox.Text = "Send items";
            this.sendItemsCheckbox.UseVisualStyleBackColor = true;
            this.sendItemsCheckbox.CheckedChanged += new System.EventHandler(this.sendItemsCheckbox_CheckedChanged);
            // 
            // connectionGroup
            // 
            this.connectionGroup.Controls.Add(this.saveServerCheckbox);
            this.connectionGroup.Controls.Add(this.serverLabel);
            this.connectionGroup.Controls.Add(this.serverTextBox);
            this.connectionGroup.Controls.Add(this.portLabel);
            this.connectionGroup.Controls.Add(this.portTextBox);
            this.connectionGroup.ForeColor = System.Drawing.Color.White;
            this.connectionGroup.Location = new System.Drawing.Point(206, 56);
            this.connectionGroup.Name = "connectionGroup";
            this.connectionGroup.Size = new System.Drawing.Size(182, 110);
            this.connectionGroup.TabIndex = 5;
            this.connectionGroup.TabStop = false;
            this.connectionGroup.Text = "Connection";
            // 
            // saveServerCheckbox
            // 
            this.saveServerCheckbox.AutoSize = true;
            this.saveServerCheckbox.Location = new System.Drawing.Point(6, 43);
            this.saveServerCheckbox.Name = "saveServerCheckbox";
            this.saveServerCheckbox.Size = new System.Drawing.Size(104, 17);
            this.saveServerCheckbox.TabIndex = 4;
            this.saveServerCheckbox.Text = "Save last server";
            this.saveServerCheckbox.UseVisualStyleBackColor = true;
            this.saveServerCheckbox.CheckedChanged += new System.EventHandler(this.saveServerCheckbox_CheckedChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(6, 74);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(80, 13);
            this.serverLabel.TabIndex = 3;
            this.serverLabel.Text = "Default server:";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(91, 71);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(85, 21);
            this.serverTextBox.TabIndex = 2;
            this.serverTextBox.UseSystemPasswordChar = true;
            this.serverTextBox.TextChanged += new System.EventHandler(this.serverTextBox_TextChanged);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(6, 17);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(69, 13);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Default port:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(91, 14);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(85, 21);
            this.portTextBox.TabIndex = 0;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // CoopSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.connectionGroup);
            this.Controls.Add(this.optionsBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.multiplayerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CoopSettingsPanel";
            this.Size = new System.Drawing.Size(395, 368);
            this.Load += new System.EventHandler(this.MultiplayerSettingsPanel_Load);
            this.optionsBox.ResumeLayout(false);
            this.optionsBox.PerformLayout();
            this.connectionGroup.ResumeLayout(false);
            this.connectionGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label multiplayerPanelTitle;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.GroupBox optionsBox;
		private System.Windows.Forms.CheckBox sendItemsCheckbox;
		private System.Windows.Forms.CheckBox shareWarpsCheckbox;
		private System.Windows.Forms.GroupBox connectionGroup;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.Label serverLabel;
		private System.Windows.Forms.TextBox serverTextBox;
		private System.Windows.Forms.CheckBox saveServerCheckbox;
		private System.Windows.Forms.CheckBox shareLocationsCheckbox;
		private System.Windows.Forms.CheckBox sendAssistsCheckbox;
		private System.Windows.Forms.CheckBox sendRelicsCheckbox;
	}
}

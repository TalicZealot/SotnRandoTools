
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
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.volumeBox = new System.Windows.Forms.GroupBox();
            this.volumeBar = new System.Windows.Forms.TrackBar();
            this.optionsBox.SuspendLayout();
            this.volumeBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).BeginInit();
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
            this.saveButton.Location = new System.Drawing.Point(304, 380);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(84, 25);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // optionsBox
            // 
            this.optionsBox.Controls.Add(this.portLabel);
            this.optionsBox.Controls.Add(this.portTextBox);
            this.optionsBox.ForeColor = System.Drawing.Color.White;
            this.optionsBox.Location = new System.Drawing.Point(6, 56);
            this.optionsBox.Name = "optionsBox";
            this.optionsBox.Size = new System.Drawing.Size(382, 141);
            this.optionsBox.TabIndex = 4;
            this.optionsBox.TabStop = false;
            this.optionsBox.Text = "Options";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(6, 26);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(69, 13);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Default port:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(91, 23);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(85, 21);
            this.portTextBox.TabIndex = 0;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // volumeBox
            // 
            this.volumeBox.Controls.Add(this.volumeBar);
            this.volumeBox.ForeColor = System.Drawing.Color.White;
            this.volumeBox.Location = new System.Drawing.Point(10, 212);
            this.volumeBox.Name = "volumeBox";
            this.volumeBox.Size = new System.Drawing.Size(378, 72);
            this.volumeBox.TabIndex = 6;
            this.volumeBox.TabStop = false;
            this.volumeBox.Text = "Notification Volume";
            // 
            // volumeBar
            // 
            this.volumeBar.Location = new System.Drawing.Point(1, 20);
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(371, 45);
            this.volumeBar.TabIndex = 0;
            this.volumeBar.Scroll += new System.EventHandler(this.volumeBar_Scroll);
            // 
            // CoopSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.volumeBox);
            this.Controls.Add(this.optionsBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.multiplayerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CoopSettingsPanel";
            this.Size = new System.Drawing.Size(395, 408);
            this.Load += new System.EventHandler(this.MultiplayerSettingsPanel_Load);
            this.optionsBox.ResumeLayout(false);
            this.optionsBox.PerformLayout();
            this.volumeBox.ResumeLayout(false);
            this.volumeBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label multiplayerPanelTitle;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.GroupBox optionsBox;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.GroupBox volumeBox;
        private System.Windows.Forms.TrackBar volumeBar;
    }
}

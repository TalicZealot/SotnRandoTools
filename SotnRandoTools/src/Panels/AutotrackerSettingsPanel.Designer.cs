
namespace SotnRandoTools
{
    partial class AutotrackerSettingsPanel
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
            this.autotrackerPanelTitle = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.relicDisplayGroup = new System.Windows.Forms.GroupBox();
            this.radioAllRelics = new System.Windows.Forms.RadioButton();
            this.radioProgression = new System.Windows.Forms.RadioButton();
            this.layoutGroup = new System.Windows.Forms.GroupBox();
            this.radioGrid = new System.Windows.Forms.RadioButton();
            this.radioCollected = new System.Windows.Forms.RadioButton();
            this.optionsGroup = new System.Windows.Forms.GroupBox();
            this.alwaysOpTopCheckbox = new System.Windows.Forms.CheckBox();
            this.stereoCheckBox = new System.Windows.Forms.CheckBox();
            this.muteCheckBox = new System.Windows.Forms.CheckBox();
            this.autosplitterCheckBox = new System.Windows.Forms.CheckBox();
            this.overlayCheckBox = new System.Windows.Forms.CheckBox();
            this.replaysCheckBox = new System.Windows.Forms.CheckBox();
            this.locationsCheckbox = new System.Windows.Forms.CheckBox();
            this.customSeedGroup = new System.Windows.Forms.GroupBox();
            this.customExtensionCombo = new System.Windows.Forms.ComboBox();
            this.username = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackerDerfaultsButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.loadLayoutButton = new System.Windows.Forms.Button();
            this.saveLayoutButton = new System.Windows.Forms.Button();
            this.openLayoutDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveLayoutDialog = new System.Windows.Forms.SaveFileDialog();
            this.relicDisplayGroup.SuspendLayout();
            this.layoutGroup.SuspendLayout();
            this.optionsGroup.SuspendLayout();
            this.customSeedGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // autotrackerPanelTitle
            // 
            this.autotrackerPanelTitle.AutoSize = true;
            this.autotrackerPanelTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autotrackerPanelTitle.Location = new System.Drawing.Point(1, 0);
            this.autotrackerPanelTitle.Name = "autotrackerPanelTitle";
            this.autotrackerPanelTitle.Size = new System.Drawing.Size(228, 29);
            this.autotrackerPanelTitle.TabIndex = 0;
            this.autotrackerPanelTitle.Text = "Autotracker Settings";
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
            // relicDisplayGroup
            // 
            this.relicDisplayGroup.Controls.Add(this.radioAllRelics);
            this.relicDisplayGroup.Controls.Add(this.radioProgression);
            this.relicDisplayGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.relicDisplayGroup.ForeColor = System.Drawing.Color.White;
            this.relicDisplayGroup.Location = new System.Drawing.Point(6, 56);
            this.relicDisplayGroup.Name = "relicDisplayGroup";
            this.relicDisplayGroup.Size = new System.Drawing.Size(182, 75);
            this.relicDisplayGroup.TabIndex = 3;
            this.relicDisplayGroup.TabStop = false;
            this.relicDisplayGroup.Text = "Relic Selection";
            // 
            // radioAllRelics
            // 
            this.radioAllRelics.AutoSize = true;
            this.radioAllRelics.Location = new System.Drawing.Point(6, 43);
            this.radioAllRelics.Name = "radioAllRelics";
            this.radioAllRelics.Size = new System.Drawing.Size(91, 17);
            this.radioAllRelics.TabIndex = 1;
            this.radioAllRelics.TabStop = true;
            this.radioAllRelics.Text = "Show all relics";
            this.radioAllRelics.UseVisualStyleBackColor = true;
            // 
            // radioProgression
            // 
            this.radioProgression.AutoSize = true;
            this.radioProgression.Location = new System.Drawing.Point(6, 20);
            this.radioProgression.Name = "radioProgression";
            this.radioProgression.Size = new System.Drawing.Size(137, 17);
            this.radioProgression.TabIndex = 0;
            this.radioProgression.TabStop = true;
            this.radioProgression.Text = "Show progression relics";
            this.radioProgression.UseVisualStyleBackColor = true;
            this.radioProgression.CheckedChanged += new System.EventHandler(this.radioProgression_CheckedChanged);
            // 
            // layoutGroup
            // 
            this.layoutGroup.Controls.Add(this.radioGrid);
            this.layoutGroup.Controls.Add(this.radioCollected);
            this.layoutGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layoutGroup.ForeColor = System.Drawing.Color.White;
            this.layoutGroup.Location = new System.Drawing.Point(6, 137);
            this.layoutGroup.Name = "layoutGroup";
            this.layoutGroup.Size = new System.Drawing.Size(182, 81);
            this.layoutGroup.TabIndex = 4;
            this.layoutGroup.TabStop = false;
            this.layoutGroup.Text = "Layout";
            // 
            // radioGrid
            // 
            this.radioGrid.AutoSize = true;
            this.radioGrid.Location = new System.Drawing.Point(6, 43);
            this.radioGrid.Name = "radioGrid";
            this.radioGrid.Size = new System.Drawing.Size(85, 17);
            this.radioGrid.TabIndex = 1;
            this.radioGrid.TabStop = true;
            this.radioGrid.Text = "Show all grid";
            this.radioGrid.UseVisualStyleBackColor = true;
            // 
            // radioCollected
            // 
            this.radioCollected.AutoSize = true;
            this.radioCollected.Location = new System.Drawing.Point(6, 20);
            this.radioCollected.Name = "radioCollected";
            this.radioCollected.Size = new System.Drawing.Size(138, 17);
            this.radioCollected.TabIndex = 0;
            this.radioCollected.TabStop = true;
            this.radioCollected.Text = "Show collected dynamic";
            this.radioCollected.UseVisualStyleBackColor = true;
            this.radioCollected.CheckedChanged += new System.EventHandler(this.radioCollected_CheckedChanged);
            // 
            // optionsGroup
            // 
            this.optionsGroup.Controls.Add(this.alwaysOpTopCheckbox);
            this.optionsGroup.Controls.Add(this.stereoCheckBox);
            this.optionsGroup.Controls.Add(this.muteCheckBox);
            this.optionsGroup.Controls.Add(this.autosplitterCheckBox);
            this.optionsGroup.Controls.Add(this.overlayCheckBox);
            this.optionsGroup.Controls.Add(this.replaysCheckBox);
            this.optionsGroup.Controls.Add(this.locationsCheckbox);
            this.optionsGroup.ForeColor = System.Drawing.Color.White;
            this.optionsGroup.Location = new System.Drawing.Point(206, 56);
            this.optionsGroup.Name = "optionsGroup";
            this.optionsGroup.Size = new System.Drawing.Size(182, 209);
            this.optionsGroup.TabIndex = 6;
            this.optionsGroup.TabStop = false;
            this.optionsGroup.Text = "Options";
            // 
            // alwaysOpTopCheckbox
            // 
            this.alwaysOpTopCheckbox.AutoSize = true;
            this.alwaysOpTopCheckbox.Location = new System.Drawing.Point(6, 157);
            this.alwaysOpTopCheckbox.Name = "alwaysOpTopCheckbox";
            this.alwaysOpTopCheckbox.Size = new System.Drawing.Size(94, 17);
            this.alwaysOpTopCheckbox.TabIndex = 6;
            this.alwaysOpTopCheckbox.Text = "Always on top";
            this.alwaysOpTopCheckbox.UseVisualStyleBackColor = true;
            this.alwaysOpTopCheckbox.CheckedChanged += new System.EventHandler(this.alwaysOpTopCheckbox_CheckedChanged);
            // 
            // stereoCheckBox
            // 
            this.stereoCheckBox.AutoSize = true;
            this.stereoCheckBox.Location = new System.Drawing.Point(6, 134);
            this.stereoCheckBox.Name = "stereoCheckBox";
            this.stereoCheckBox.Size = new System.Drawing.Size(58, 17);
            this.stereoCheckBox.TabIndex = 5;
            this.stereoCheckBox.Text = "Stereo";
            this.stereoCheckBox.UseVisualStyleBackColor = true;
            this.stereoCheckBox.CheckedChanged += new System.EventHandler(this.stereoCheckBox_CheckedChanged);
            // 
            // muteCheckBox
            // 
            this.muteCheckBox.AutoSize = true;
            this.muteCheckBox.Location = new System.Drawing.Point(6, 111);
            this.muteCheckBox.Name = "muteCheckBox";
            this.muteCheckBox.Size = new System.Drawing.Size(79, 17);
            this.muteCheckBox.TabIndex = 4;
            this.muteCheckBox.Text = "Mute music";
            this.muteCheckBox.UseVisualStyleBackColor = true;
            this.muteCheckBox.CheckedChanged += new System.EventHandler(this.muteCheckBox_CheckedChanged);
            // 
            // autosplitterCheckBox
            // 
            this.autosplitterCheckBox.AutoSize = true;
            this.autosplitterCheckBox.Location = new System.Drawing.Point(6, 65);
            this.autosplitterCheckBox.Name = "autosplitterCheckBox";
            this.autosplitterCheckBox.Size = new System.Drawing.Size(117, 17);
            this.autosplitterCheckBox.TabIndex = 3;
            this.autosplitterCheckBox.Text = "Enable Autosplitter";
            this.autosplitterCheckBox.UseVisualStyleBackColor = true;
            this.autosplitterCheckBox.CheckedChanged += new System.EventHandler(this.autosplitterCheckBox_CheckedChanged);
            // 
            // overlayCheckBox
            // 
            this.overlayCheckBox.AutoSize = true;
            this.overlayCheckBox.Location = new System.Drawing.Point(6, 88);
            this.overlayCheckBox.Name = "overlayCheckBox";
            this.overlayCheckBox.Size = new System.Drawing.Size(83, 17);
            this.overlayCheckBox.TabIndex = 2;
            this.overlayCheckBox.Text = "Use overlay";
            this.overlayCheckBox.UseVisualStyleBackColor = true;
            this.overlayCheckBox.CheckedChanged += new System.EventHandler(this.overlayCheckBox_CheckedChanged);
            // 
            // replaysCheckBox
            // 
            this.replaysCheckBox.AutoSize = true;
            this.replaysCheckBox.Location = new System.Drawing.Point(6, 44);
            this.replaysCheckBox.Name = "replaysCheckBox";
            this.replaysCheckBox.Size = new System.Drawing.Size(88, 17);
            this.replaysCheckBox.TabIndex = 1;
            this.replaysCheckBox.Text = "Save replays";
            this.replaysCheckBox.UseVisualStyleBackColor = true;
            this.replaysCheckBox.CheckedChanged += new System.EventHandler(this.replaysCheckBox_CheckedChanged);
            // 
            // locationsCheckbox
            // 
            this.locationsCheckbox.AutoSize = true;
            this.locationsCheckbox.Location = new System.Drawing.Point(6, 21);
            this.locationsCheckbox.Name = "locationsCheckbox";
            this.locationsCheckbox.Size = new System.Drawing.Size(97, 17);
            this.locationsCheckbox.TabIndex = 0;
            this.locationsCheckbox.Text = "Track locations";
            this.locationsCheckbox.UseVisualStyleBackColor = true;
            this.locationsCheckbox.CheckedChanged += new System.EventHandler(this.locationsCheckbox_CheckedChanged);
            // 
            // customSeedGroup
            // 
            this.customSeedGroup.Controls.Add(this.customExtensionCombo);
            this.customSeedGroup.ForeColor = System.Drawing.Color.White;
            this.customSeedGroup.Location = new System.Drawing.Point(206, 271);
            this.customSeedGroup.Name = "customSeedGroup";
            this.customSeedGroup.Size = new System.Drawing.Size(182, 103);
            this.customSeedGroup.TabIndex = 7;
            this.customSeedGroup.TabStop = false;
            this.customSeedGroup.Text = "Custom Location Extension";
            // 
            // customExtensionCombo
            // 
            this.customExtensionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customExtensionCombo.Location = new System.Drawing.Point(6, 20);
            this.customExtensionCombo.Name = "customExtensionCombo";
            this.customExtensionCombo.Size = new System.Drawing.Size(170, 21);
            this.customExtensionCombo.TabIndex = 0;
            this.customExtensionCombo.SelectedIndexChanged += new System.EventHandler(this.customExtensionCombo_SelectedIndexChanged);
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(5, 20);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(170, 21);
            this.username.TabIndex = 5;
            this.username.TextChanged += new System.EventHandler(this.username_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.username);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(6, 271);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 49);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "Username";
            this.groupBox1.Text = "Username";
            // 
            // trackerDerfaultsButton
            // 
            this.trackerDerfaultsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.trackerDerfaultsButton.FlatAppearance.BorderSize = 2;
            this.trackerDerfaultsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.trackerDerfaultsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.trackerDerfaultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.trackerDerfaultsButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.trackerDerfaultsButton.Location = new System.Drawing.Point(3, 380);
            this.trackerDerfaultsButton.Name = "trackerDerfaultsButton";
            this.trackerDerfaultsButton.Size = new System.Drawing.Size(118, 25);
            this.trackerDerfaultsButton.TabIndex = 9;
            this.trackerDerfaultsButton.Text = "Tracker Defaults";
            this.trackerDerfaultsButton.UseVisualStyleBackColor = true;
            this.trackerDerfaultsButton.Click += new System.EventHandler(this.trackerDerfaultsButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.loadLayoutButton);
            this.groupBox2.Controls.Add(this.saveLayoutButton);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(6, 319);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 55);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Custom Overlay Layout";
            // 
            // loadLayoutButton
            // 
            this.loadLayoutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.loadLayoutButton.FlatAppearance.BorderSize = 2;
            this.loadLayoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.loadLayoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.loadLayoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadLayoutButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadLayoutButton.Location = new System.Drawing.Point(6, 20);
            this.loadLayoutButton.Name = "loadLayoutButton";
            this.loadLayoutButton.Size = new System.Drawing.Size(84, 25);
            this.loadLayoutButton.TabIndex = 4;
            this.loadLayoutButton.Text = "Load";
            this.loadLayoutButton.UseVisualStyleBackColor = true;
            this.loadLayoutButton.Click += new System.EventHandler(this.loadLayoutButton_Click);
            // 
            // saveLayoutButton
            // 
            this.saveLayoutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.saveLayoutButton.FlatAppearance.BorderSize = 2;
            this.saveLayoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.saveLayoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.saveLayoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveLayoutButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveLayoutButton.Location = new System.Drawing.Point(92, 20);
            this.saveLayoutButton.Name = "saveLayoutButton";
            this.saveLayoutButton.Size = new System.Drawing.Size(84, 25);
            this.saveLayoutButton.TabIndex = 3;
            this.saveLayoutButton.Text = "Save";
            this.saveLayoutButton.UseVisualStyleBackColor = true;
            this.saveLayoutButton.Click += new System.EventHandler(this.saveLayoutButton_Click);
            // 
            // openLayoutDialog
            // 
            this.openLayoutDialog.FileName = "Layout";
            this.openLayoutDialog.Filter = "layout config files|*.ini";
            this.openLayoutDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openLayoutDialog_FileOk);
            // 
            // saveLayoutDialog
            // 
            this.saveLayoutDialog.FileName = "Layout";
            this.saveLayoutDialog.Filter = "layout config files|*.ini";
            this.saveLayoutDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveLayoutDialog_FileOk);
            // 
            // AutotrackerSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.trackerDerfaultsButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.customSeedGroup);
            this.Controls.Add(this.optionsGroup);
            this.Controls.Add(this.layoutGroup);
            this.Controls.Add(this.relicDisplayGroup);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.autotrackerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "AutotrackerSettingsPanel";
            this.Size = new System.Drawing.Size(395, 408);
            this.Load += new System.EventHandler(this.AutotrackerSettingsPanel_Load);
            this.relicDisplayGroup.ResumeLayout(false);
            this.relicDisplayGroup.PerformLayout();
            this.layoutGroup.ResumeLayout(false);
            this.layoutGroup.PerformLayout();
            this.optionsGroup.ResumeLayout(false);
            this.optionsGroup.PerformLayout();
            this.customSeedGroup.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label autotrackerPanelTitle;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox relicDisplayGroup;
        private System.Windows.Forms.RadioButton radioAllRelics;
        private System.Windows.Forms.RadioButton radioProgression;
        private System.Windows.Forms.GroupBox layoutGroup;
        private System.Windows.Forms.RadioButton radioGrid;
        private System.Windows.Forms.RadioButton radioCollected;
		private System.Windows.Forms.GroupBox optionsGroup;
		private System.Windows.Forms.CheckBox locationsCheckbox;
		private System.Windows.Forms.CheckBox replaysCheckBox;
		private System.Windows.Forms.GroupBox customSeedGroup;
		private System.Windows.Forms.TextBox username;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox overlayCheckBox;
		private System.Windows.Forms.CheckBox autosplitterCheckBox;
		private System.Windows.Forms.Button trackerDerfaultsButton;
		private System.Windows.Forms.CheckBox muteCheckBox;
		private System.Windows.Forms.CheckBox stereoCheckBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button loadLayoutButton;
		private System.Windows.Forms.Button saveLayoutButton;
		private System.Windows.Forms.OpenFileDialog openLayoutDialog;
		private System.Windows.Forms.SaveFileDialog saveLayoutDialog;
		private System.Windows.Forms.CheckBox alwaysOpTopCheckbox;
		private System.Windows.Forms.ComboBox customExtensionCombo;
	}
}

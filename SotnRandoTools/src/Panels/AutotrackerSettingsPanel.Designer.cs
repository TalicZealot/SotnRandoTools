
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
            this.windowGroup = new System.Windows.Forms.GroupBox();
            this.alwaysOnTopCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsGroup = new System.Windows.Forms.GroupBox();
            this.replaysCheckBox = new System.Windows.Forms.CheckBox();
            this.locationsCheckbox = new System.Windows.Forms.CheckBox();
            this.customSeedGroup = new System.Windows.Forms.GroupBox();
            this.customLocationsClassicRadio = new System.Windows.Forms.RadioButton();
            this.customLocationsEquipmentRadio = new System.Windows.Forms.RadioButton();
            this.customLocationsGuardedRadio = new System.Windows.Forms.RadioButton();
            this.username = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.relicDisplayGroup.SuspendLayout();
            this.layoutGroup.SuspendLayout();
            this.windowGroup.SuspendLayout();
            this.optionsGroup.SuspendLayout();
            this.customSeedGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.saveButton.Location = new System.Drawing.Point(304, 340);
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
            this.layoutGroup.Location = new System.Drawing.Point(6, 149);
            this.layoutGroup.Name = "layoutGroup";
            this.layoutGroup.Size = new System.Drawing.Size(182, 75);
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
            // windowGroup
            // 
            this.windowGroup.Controls.Add(this.alwaysOnTopCheckBox);
            this.windowGroup.ForeColor = System.Drawing.Color.White;
            this.windowGroup.Location = new System.Drawing.Point(6, 245);
            this.windowGroup.Name = "windowGroup";
            this.windowGroup.Size = new System.Drawing.Size(182, 75);
            this.windowGroup.TabIndex = 5;
            this.windowGroup.TabStop = false;
            this.windowGroup.Text = "Window";
            // 
            // alwaysOnTopCheckBox
            // 
            this.alwaysOnTopCheckBox.AutoSize = true;
            this.alwaysOnTopCheckBox.Location = new System.Drawing.Point(6, 20);
            this.alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
            this.alwaysOnTopCheckBox.Size = new System.Drawing.Size(94, 17);
            this.alwaysOnTopCheckBox.TabIndex = 0;
            this.alwaysOnTopCheckBox.Text = "Always on top";
            this.alwaysOnTopCheckBox.UseVisualStyleBackColor = true;
            this.alwaysOnTopCheckBox.CheckedChanged += new System.EventHandler(this.alwaysOnTopCheckBox_CheckedChanged);
            // 
            // optionsGroup
            // 
            this.optionsGroup.Controls.Add(this.replaysCheckBox);
            this.optionsGroup.Controls.Add(this.locationsCheckbox);
            this.optionsGroup.ForeColor = System.Drawing.Color.White;
            this.optionsGroup.Location = new System.Drawing.Point(206, 56);
            this.optionsGroup.Name = "optionsGroup";
            this.optionsGroup.Size = new System.Drawing.Size(182, 111);
            this.optionsGroup.TabIndex = 6;
            this.optionsGroup.TabStop = false;
            this.optionsGroup.Text = "Options";
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
            this.customSeedGroup.Controls.Add(this.customLocationsClassicRadio);
            this.customSeedGroup.Controls.Add(this.customLocationsEquipmentRadio);
            this.customSeedGroup.Controls.Add(this.customLocationsGuardedRadio);
            this.customSeedGroup.ForeColor = System.Drawing.Color.White;
            this.customSeedGroup.Location = new System.Drawing.Point(206, 224);
            this.customSeedGroup.Name = "customSeedGroup";
            this.customSeedGroup.Size = new System.Drawing.Size(182, 96);
            this.customSeedGroup.TabIndex = 7;
            this.customSeedGroup.TabStop = false;
            this.customSeedGroup.Text = "Custom Seed Locations";
            // 
            // customLocationsClassicRadio
            // 
            this.customLocationsClassicRadio.AutoSize = true;
            this.customLocationsClassicRadio.Location = new System.Drawing.Point(6, 66);
            this.customLocationsClassicRadio.Name = "customLocationsClassicRadio";
            this.customLocationsClassicRadio.Size = new System.Drawing.Size(57, 17);
            this.customLocationsClassicRadio.TabIndex = 3;
            this.customLocationsClassicRadio.TabStop = true;
            this.customLocationsClassicRadio.Text = "Classic";
            this.customLocationsClassicRadio.UseVisualStyleBackColor = true;
            this.customLocationsClassicRadio.CheckedChanged += new System.EventHandler(this.customLocationsClassicRadio_CheckedChanged);
            // 
            // customLocationsEquipmentRadio
            // 
            this.customLocationsEquipmentRadio.AutoSize = true;
            this.customLocationsEquipmentRadio.Location = new System.Drawing.Point(6, 43);
            this.customLocationsEquipmentRadio.Name = "customLocationsEquipmentRadio";
            this.customLocationsEquipmentRadio.Size = new System.Drawing.Size(75, 17);
            this.customLocationsEquipmentRadio.TabIndex = 2;
            this.customLocationsEquipmentRadio.TabStop = true;
            this.customLocationsEquipmentRadio.Text = "Equipment";
            this.customLocationsEquipmentRadio.UseVisualStyleBackColor = true;
            this.customLocationsEquipmentRadio.CheckedChanged += new System.EventHandler(this.customLocationsEquipmentRadio_CheckedChanged);
            // 
            // customLocationsGuardedRadio
            // 
            this.customLocationsGuardedRadio.AutoSize = true;
            this.customLocationsGuardedRadio.Location = new System.Drawing.Point(6, 20);
            this.customLocationsGuardedRadio.Name = "customLocationsGuardedRadio";
            this.customLocationsGuardedRadio.Size = new System.Drawing.Size(66, 17);
            this.customLocationsGuardedRadio.TabIndex = 1;
            this.customLocationsGuardedRadio.TabStop = true;
            this.customLocationsGuardedRadio.Text = "Guarded";
            this.customLocationsGuardedRadio.UseVisualStyleBackColor = true;
            this.customLocationsGuardedRadio.CheckedChanged += new System.EventHandler(this.customLocationsGuardedRadio_CheckedChanged);
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(6, 19);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(170, 21);
            this.username.TabIndex = 5;
            this.username.TextChanged += new System.EventHandler(this.username_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.username);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(206, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 49);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "Username";
            this.groupBox1.Text = "Username";
            // 
            // AutotrackerSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.customSeedGroup);
            this.Controls.Add(this.optionsGroup);
            this.Controls.Add(this.windowGroup);
            this.Controls.Add(this.layoutGroup);
            this.Controls.Add(this.relicDisplayGroup);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.autotrackerPanelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "AutotrackerSettingsPanel";
            this.Size = new System.Drawing.Size(395, 368);
            this.Load += new System.EventHandler(this.AutotrackerSettingsPanel_Load);
            this.relicDisplayGroup.ResumeLayout(false);
            this.relicDisplayGroup.PerformLayout();
            this.layoutGroup.ResumeLayout(false);
            this.layoutGroup.PerformLayout();
            this.windowGroup.ResumeLayout(false);
            this.windowGroup.PerformLayout();
            this.optionsGroup.ResumeLayout(false);
            this.optionsGroup.PerformLayout();
            this.customSeedGroup.ResumeLayout(false);
            this.customSeedGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox windowGroup;
        private System.Windows.Forms.CheckBox alwaysOnTopCheckBox;
		private System.Windows.Forms.GroupBox optionsGroup;
		private System.Windows.Forms.CheckBox locationsCheckbox;
		private System.Windows.Forms.CheckBox replaysCheckBox;
		private System.Windows.Forms.GroupBox customSeedGroup;
		private System.Windows.Forms.RadioButton customLocationsClassicRadio;
		private System.Windows.Forms.RadioButton customLocationsEquipmentRadio;
		private System.Windows.Forms.RadioButton customLocationsGuardedRadio;
		private System.Windows.Forms.TextBox username;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

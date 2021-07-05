
namespace SotnRandoTools
{
    partial class ToolMainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolMainForm));
            this.mainMenuPanel = new System.Windows.Forms.Panel();
            this.aboutButton = new System.Windows.Forms.Button();
            this.multiplayerLaunch = new System.Windows.Forms.Button();
            this.khaosChatLaunch = new System.Windows.Forms.Button();
            this.autotrackerLaunch = new System.Windows.Forms.Button();
            this.autotrackerSelect = new System.Windows.Forms.Button();
            this.multiplayerSelect = new System.Windows.Forms.Button();
            this.khaosChatSelect = new System.Windows.Forms.Button();
            this.mainMenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuPanel
            // 
            this.mainMenuPanel.Controls.Add(this.autotrackerSelect);
            this.mainMenuPanel.Controls.Add(this.aboutButton);
            this.mainMenuPanel.Controls.Add(this.multiplayerLaunch);
            this.mainMenuPanel.Controls.Add(this.khaosChatLaunch);
            this.mainMenuPanel.Controls.Add(this.multiplayerSelect);
            this.mainMenuPanel.Controls.Add(this.khaosChatSelect);
            this.mainMenuPanel.Controls.Add(this.autotrackerLaunch);
            this.mainMenuPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.mainMenuPanel.Name = "mainMenuPanel";
            this.mainMenuPanel.Size = new System.Drawing.Size(394, 127);
            this.mainMenuPanel.TabIndex = 0;
            // 
            // aboutButton
            // 
            this.aboutButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.aboutButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.aboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.aboutButton.FlatAppearance.BorderSize = 2;
            this.aboutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.aboutButton.Image = global::SotnRandoTools.Properties.Resources.LogoTZtwtr1;
            this.aboutButton.Location = new System.Drawing.Point(294, 12);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(88, 70);
            this.aboutButton.TabIndex = 4;
            this.aboutButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // multiplayerLaunch
            // 
            this.multiplayerLaunch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.multiplayerLaunch.FlatAppearance.BorderSize = 2;
            this.multiplayerLaunch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.multiplayerLaunch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.multiplayerLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.multiplayerLaunch.Location = new System.Drawing.Point(200, 88);
            this.multiplayerLaunch.Name = "multiplayerLaunch";
            this.multiplayerLaunch.Size = new System.Drawing.Size(87, 29);
            this.multiplayerLaunch.TabIndex = 3;
            this.multiplayerLaunch.Text = "Launch";
            this.multiplayerLaunch.UseVisualStyleBackColor = true;
            this.multiplayerLaunch.Visible = false;
            this.multiplayerLaunch.Click += new System.EventHandler(this.multiplayerLaunch_Click);
            // 
            // khaosChatLaunch
            // 
            this.khaosChatLaunch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.khaosChatLaunch.FlatAppearance.BorderSize = 2;
            this.khaosChatLaunch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.khaosChatLaunch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.khaosChatLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.khaosChatLaunch.Location = new System.Drawing.Point(106, 88);
            this.khaosChatLaunch.Name = "khaosChatLaunch";
            this.khaosChatLaunch.Size = new System.Drawing.Size(87, 29);
            this.khaosChatLaunch.TabIndex = 3;
            this.khaosChatLaunch.Text = "Launch";
            this.khaosChatLaunch.UseVisualStyleBackColor = true;
            this.khaosChatLaunch.Visible = false;
            this.khaosChatLaunch.Click += new System.EventHandler(this.khaosChatLaunch_Click);
            // 
            // autotrackerLaunch
            // 
            this.autotrackerLaunch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.autotrackerLaunch.FlatAppearance.BorderSize = 2;
            this.autotrackerLaunch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.autotrackerLaunch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.autotrackerLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autotrackerLaunch.Location = new System.Drawing.Point(12, 88);
            this.autotrackerLaunch.Name = "autotrackerLaunch";
            this.autotrackerLaunch.Size = new System.Drawing.Size(87, 29);
            this.autotrackerLaunch.TabIndex = 1;
            this.autotrackerLaunch.Text = "Launch";
            this.autotrackerLaunch.UseVisualStyleBackColor = true;
            this.autotrackerLaunch.Visible = false;
            this.autotrackerLaunch.Click += new System.EventHandler(this.autotrackerLaunch_Click);
            // 
            // autotrackerSelect
            // 
            this.autotrackerSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.autotrackerSelect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.autotrackerSelect.FlatAppearance.BorderSize = 2;
            this.autotrackerSelect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.autotrackerSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.autotrackerSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autotrackerSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autotrackerSelect.Image = global::SotnRandoTools.Properties.Resources.tracker;
            this.autotrackerSelect.Location = new System.Drawing.Point(12, 12);
            this.autotrackerSelect.Name = "autotrackerSelect";
            this.autotrackerSelect.Size = new System.Drawing.Size(88, 70);
            this.autotrackerSelect.TabIndex = 5;
            this.autotrackerSelect.UseVisualStyleBackColor = true;
            this.autotrackerSelect.Click += new System.EventHandler(this.autotrackerSelect_Click);
            // 
            // multiplayerSelect
            // 
            this.multiplayerSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.multiplayerSelect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.multiplayerSelect.FlatAppearance.BorderSize = 2;
            this.multiplayerSelect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.multiplayerSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.multiplayerSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.multiplayerSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.multiplayerSelect.Image = global::SotnRandoTools.Properties.Resources.coop;
            this.multiplayerSelect.Location = new System.Drawing.Point(200, 12);
            this.multiplayerSelect.Name = "multiplayerSelect";
            this.multiplayerSelect.Size = new System.Drawing.Size(88, 70);
            this.multiplayerSelect.TabIndex = 2;
            this.multiplayerSelect.UseVisualStyleBackColor = true;
            this.multiplayerSelect.Click += new System.EventHandler(this.multiplayerSelect_Click);
            // 
            // khaosChatSelect
            // 
            this.khaosChatSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.khaosChatSelect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.khaosChatSelect.FlatAppearance.BorderSize = 2;
            this.khaosChatSelect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.khaosChatSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.khaosChatSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.khaosChatSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.khaosChatSelect.Image = global::SotnRandoTools.Properties.Resources.khaosb;
            this.khaosChatSelect.Location = new System.Drawing.Point(106, 12);
            this.khaosChatSelect.Name = "khaosChatSelect";
            this.khaosChatSelect.Size = new System.Drawing.Size(88, 70);
            this.khaosChatSelect.TabIndex = 2;
            this.khaosChatSelect.UseVisualStyleBackColor = true;
            this.khaosChatSelect.Click += new System.EventHandler(this.khaosChatSelect_Click);
            // 
            // ToolMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(394, 501);
            this.Controls.Add(this.mainMenuPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ToolMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolMainForm_FormClosing);
            this.Load += new System.EventHandler(this.ToolMainForm_Load);
            this.Move += new System.EventHandler(this.ToolMainForm_Move);
            this.mainMenuPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainMenuPanel;
        private System.Windows.Forms.Button autotrackerLaunch;
        private System.Windows.Forms.Button multiplayerLaunch;
        private System.Windows.Forms.Button khaosChatLaunch;
        private System.Windows.Forms.Button multiplayerSelect;
        private System.Windows.Forms.Button khaosChatSelect;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button autotrackerSelect;
	}
}


﻿
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
            this.mainMenuPanel = new System.Windows.Forms.Panel();
            this.autotrackerSelect = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.coopLaunch = new System.Windows.Forms.Button();
            this.coopSelect = new System.Windows.Forms.Button();
            this.autotrackerLaunch = new System.Windows.Forms.Button();
            this.mainMenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuPanel
            // 
            this.mainMenuPanel.Controls.Add(this.autotrackerSelect);
            this.mainMenuPanel.Controls.Add(this.aboutButton);
            this.mainMenuPanel.Controls.Add(this.coopLaunch);
            this.mainMenuPanel.Controls.Add(this.coopSelect);
            this.mainMenuPanel.Controls.Add(this.autotrackerLaunch);
            this.mainMenuPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.mainMenuPanel.Name = "mainMenuPanel";
            this.mainMenuPanel.Size = new System.Drawing.Size(394, 127);
            this.mainMenuPanel.TabIndex = 0;
            // 
            // autotrackerSelect
            // 
            this.autotrackerSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(35)))));
            this.autotrackerSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.autotrackerSelect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.autotrackerSelect.FlatAppearance.BorderSize = 2;
            this.autotrackerSelect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.autotrackerSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.autotrackerSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autotrackerSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autotrackerSelect.Location = new System.Drawing.Point(12, 12);
            this.autotrackerSelect.Name = "autotrackerSelect";
            this.autotrackerSelect.Size = new System.Drawing.Size(88, 70);
            this.autotrackerSelect.TabIndex = 5;
            this.autotrackerSelect.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.autotrackerSelect.UseVisualStyleBackColor = false;
            this.autotrackerSelect.Click += new System.EventHandler(this.autotrackerSelect_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(35)))));
            this.aboutButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.aboutButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.aboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.aboutButton.FlatAppearance.BorderSize = 2;
            this.aboutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.aboutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.aboutButton.Location = new System.Drawing.Point(200, 12);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(88, 70);
            this.aboutButton.TabIndex = 4;
            this.aboutButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.aboutButton.UseVisualStyleBackColor = false;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // coopLaunch
            // 
            this.coopLaunch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.coopLaunch.FlatAppearance.BorderSize = 2;
            this.coopLaunch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.coopLaunch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.coopLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coopLaunch.Location = new System.Drawing.Point(106, 88);
            this.coopLaunch.Name = "coopLaunch";
            this.coopLaunch.Size = new System.Drawing.Size(87, 29);
            this.coopLaunch.TabIndex = 3;
            this.coopLaunch.Text = "Launch Co-Op";
            this.coopLaunch.UseVisualStyleBackColor = true;
            this.coopLaunch.Visible = false;
            this.coopLaunch.Click += new System.EventHandler(this.coopLaunch_Click);
            // 
            // coopSelect
            // 
            this.coopSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(35)))));
            this.coopSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.coopSelect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.coopSelect.FlatAppearance.BorderSize = 2;
            this.coopSelect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.coopSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.coopSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coopSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.coopSelect.Location = new System.Drawing.Point(106, 12);
            this.coopSelect.Name = "coopSelect";
            this.coopSelect.Size = new System.Drawing.Size(88, 70);
            this.coopSelect.TabIndex = 2;
            this.coopSelect.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.coopSelect.UseVisualStyleBackColor = false;
            this.coopSelect.Click += new System.EventHandler(this.coopSelect_Click);
            // 
            // autotrackerLaunch
            // 
            this.autotrackerLaunch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.autotrackerLaunch.FlatAppearance.BorderSize = 2;
            this.autotrackerLaunch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.autotrackerLaunch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.autotrackerLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autotrackerLaunch.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autotrackerLaunch.Location = new System.Drawing.Point(12, 88);
            this.autotrackerLaunch.Name = "autotrackerLaunch";
            this.autotrackerLaunch.Size = new System.Drawing.Size(87, 29);
            this.autotrackerLaunch.TabIndex = 1;
            this.autotrackerLaunch.Text = "Launch Tracker";
            this.autotrackerLaunch.UseVisualStyleBackColor = true;
            this.autotrackerLaunch.Visible = false;
            this.autotrackerLaunch.Click += new System.EventHandler(this.autotrackerLaunch_Click);
            // 
            // ToolMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(394, 541);
            this.Controls.Add(this.mainMenuPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
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
        private System.Windows.Forms.Button coopLaunch;
        private System.Windows.Forms.Button coopSelect;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button autotrackerSelect;
	}
}


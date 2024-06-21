
namespace SotnRandoTools
{
    partial class CoopForm
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
            this.components = new System.ComponentModel.Container();
            this.hostButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.targetIp = new System.Windows.Forms.TextBox();
            this.IPlabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.portNumeric = new System.Windows.Forms.NumericUpDown();
            this.serverGroup = new System.Windows.Forms.GroupBox();
            this.clientGroup = new System.Windows.Forms.GroupBox();
            this.addressTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ping = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.portNumeric)).BeginInit();
            this.serverGroup.SuspendLayout();
            this.clientGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hostButton
            // 
            this.hostButton.CausesValidation = false;
            this.hostButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.hostButton.FlatAppearance.BorderSize = 2;
            this.hostButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.hostButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.hostButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hostButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hostButton.Location = new System.Drawing.Point(8, 18);
            this.hostButton.Name = "hostButton";
            this.hostButton.Size = new System.Drawing.Size(85, 23);
            this.hostButton.TabIndex = 0;
            this.hostButton.Text = "Host";
            this.hostButton.UseVisualStyleBackColor = true;
            this.hostButton.Click += new System.EventHandler(this.hostButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(21)))), ((int)(((byte)(57)))));
            this.connectButton.FlatAppearance.BorderSize = 2;
            this.connectButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(35)))), ((int)(((byte)(67)))));
            this.connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connectButton.Location = new System.Drawing.Point(8, 14);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(261, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // targetIp
            // 
            this.targetIp.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.targetIp.Location = new System.Drawing.Point(49, 44);
            this.targetIp.Name = "targetIp";
            this.targetIp.Size = new System.Drawing.Size(219, 21);
            this.targetIp.TabIndex = 2;
            this.targetIp.TextChanged += new System.EventHandler(this.targetIp_TextChanged);
            this.targetIp.Validating += new System.ComponentModel.CancelEventHandler(this.targetIp_Validating);
            this.targetIp.Validated += new System.EventHandler(this.targetIp_Validated);
            // 
            // IPlabel
            // 
            this.IPlabel.AutoSize = true;
            this.IPlabel.BackColor = System.Drawing.Color.Transparent;
            this.IPlabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IPlabel.ForeColor = System.Drawing.Color.White;
            this.IPlabel.Location = new System.Drawing.Point(5, 47);
            this.IPlabel.Margin = new System.Windows.Forms.Padding(0);
            this.IPlabel.Name = "IPlabel";
            this.IPlabel.Size = new System.Drawing.Size(41, 13);
            this.IPlabel.TabIndex = 4;
            this.IPlabel.Text = "IP:port";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portLabel.ForeColor = System.Drawing.Color.White;
            this.portLabel.Location = new System.Drawing.Point(99, 23);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(31, 13);
            this.portLabel.TabIndex = 5;
            this.portLabel.Text = "Port:";
            // 
            // portNumeric
            // 
            this.portNumeric.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portNumeric.Location = new System.Drawing.Point(129, 20);
            this.portNumeric.Maximum = new decimal(new int[] {
            49151,
            0,
            0,
            0});
            this.portNumeric.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.portNumeric.Name = "portNumeric";
            this.portNumeric.Size = new System.Drawing.Size(139, 21);
            this.portNumeric.TabIndex = 6;
            this.portNumeric.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // serverGroup
            // 
            this.serverGroup.Controls.Add(this.hostButton);
            this.serverGroup.Controls.Add(this.portNumeric);
            this.serverGroup.Controls.Add(this.portLabel);
            this.serverGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.serverGroup.ForeColor = System.Drawing.Color.White;
            this.serverGroup.Location = new System.Drawing.Point(4, 12);
            this.serverGroup.Name = "serverGroup";
            this.serverGroup.Size = new System.Drawing.Size(275, 53);
            this.serverGroup.TabIndex = 7;
            this.serverGroup.TabStop = false;
            this.serverGroup.Text = "Server";
            // 
            // clientGroup
            // 
            this.clientGroup.Controls.Add(this.connectButton);
            this.clientGroup.Controls.Add(this.targetIp);
            this.clientGroup.Controls.Add(this.IPlabel);
            this.clientGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clientGroup.ForeColor = System.Drawing.Color.White;
            this.clientGroup.Location = new System.Drawing.Point(4, 71);
            this.clientGroup.Name = "clientGroup";
            this.clientGroup.Size = new System.Drawing.Size(275, 73);
            this.clientGroup.TabIndex = 8;
            this.clientGroup.TabStop = false;
            this.clientGroup.Text = "Client";
            // 
            // addressTooltip
            // 
            this.addressTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ping);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(4, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 47);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // ping
            // 
            this.ping.AutoSize = true;
            this.ping.BackColor = System.Drawing.Color.Transparent;
            this.ping.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ping.ForeColor = System.Drawing.Color.White;
            this.ping.Location = new System.Drawing.Point(36, 21);
            this.ping.Name = "ping";
            this.ping.Size = new System.Drawing.Size(15, 16);
            this.ping.TabIndex = 7;
            this.ping.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Ping:";
            // 
            // CoopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(284, 203);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.clientGroup);
            this.Controls.Add(this.serverGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 800);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "CoopForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Co-op";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoopForm_FormClosing);
            this.Load += new System.EventHandler(this.CoopForm_Load);
            this.Move += new System.EventHandler(this.CoopForm_Move);
            ((System.ComponentModel.ISupportInitialize)(this.portNumeric)).EndInit();
            this.serverGroup.ResumeLayout(false);
            this.serverGroup.PerformLayout();
            this.clientGroup.ResumeLayout(false);
            this.clientGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.Button hostButton;
		private System.Windows.Forms.Button connectButton;
		private System.Windows.Forms.TextBox targetIp;
		private System.Windows.Forms.Label IPlabel;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.NumericUpDown portNumeric;
		private System.Windows.Forms.GroupBox serverGroup;
		private System.Windows.Forms.GroupBox clientGroup;
		private System.Windows.Forms.ToolTip addressTooltip;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label ping;
		private System.Windows.Forms.Label label1;
	}
}


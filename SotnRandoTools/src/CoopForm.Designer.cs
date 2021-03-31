
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoopForm));
            this.hostButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.targetIp = new System.Windows.Forms.TextBox();
            this.IPlabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.portNumeric = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.portNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // hostButton
            // 
            this.hostButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hostButton.Location = new System.Drawing.Point(4, 8);
            this.hostButton.Name = "hostButton";
            this.hostButton.Size = new System.Drawing.Size(85, 21);
            this.hostButton.TabIndex = 0;
            this.hostButton.Text = "Host";
            this.hostButton.UseVisualStyleBackColor = true;
            this.hostButton.Click += new System.EventHandler(this.hostButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connectButton.Location = new System.Drawing.Point(5, 33);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(85, 21);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // targetIp
            // 
            this.targetIp.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.targetIp.Location = new System.Drawing.Point(141, 33);
            this.targetIp.Name = "targetIp";
            this.targetIp.Size = new System.Drawing.Size(139, 21);
            this.targetIp.TabIndex = 2;
            this.targetIp.UseSystemPasswordChar = true;
            this.targetIp.TextChanged += new System.EventHandler(this.targetIp_TextChanged);
            this.targetIp.Validating += new System.ComponentModel.CancelEventHandler(this.targetIp_Validating);
            // 
            // IPlabel
            // 
            this.IPlabel.AutoSize = true;
            this.IPlabel.BackColor = System.Drawing.Color.Transparent;
            this.IPlabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IPlabel.ForeColor = System.Drawing.Color.White;
            this.IPlabel.Location = new System.Drawing.Point(91, 37);
            this.IPlabel.Margin = new System.Windows.Forms.Padding(0);
            this.IPlabel.Name = "IPlabel";
            this.IPlabel.Size = new System.Drawing.Size(50, 13);
            this.IPlabel.TabIndex = 4;
            this.IPlabel.Text = "Address:";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portLabel.ForeColor = System.Drawing.Color.White;
            this.portLabel.Location = new System.Drawing.Point(109, 12);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(31, 13);
            this.portLabel.TabIndex = 5;
            this.portLabel.Text = "Port:";
            // 
            // portNumeric
            // 
            this.portNumeric.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portNumeric.Location = new System.Drawing.Point(141, 8);
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
            // CoopForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(284, 61);
            this.Controls.Add(this.portNumeric);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.IPlabel);
            this.Controls.Add(this.targetIp);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.hostButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.Button hostButton;
		private System.Windows.Forms.Button connectButton;
		private System.Windows.Forms.TextBox targetIp;
		private System.Windows.Forms.Label IPlabel;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.NumericUpDown portNumeric;
	}
}


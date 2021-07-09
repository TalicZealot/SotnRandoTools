
namespace SotnRandoTools
{
    partial class TrackerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackerForm));
            this.SuspendLayout();
            // 
            // TrackerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(284, 361);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 800);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "TrackerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Autotracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackerForm_FormClosing);
            this.Load += new System.EventHandler(this.TrackerForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TrackerForm_Paint);
            this.Move += new System.EventHandler(this.TrackerForm_Move);
            this.Resize += new System.EventHandler(this.TrackerForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}


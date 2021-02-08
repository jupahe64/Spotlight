namespace Spotlight
{
    partial class LoadLevelForm
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
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.LevelTimer = new System.Windows.Forms.Timer(this.components);
            this.TalkTimer = new System.Windows.Forms.Timer(this.components);
            this.CloseTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainProgressBar.Location = new System.Drawing.Point(0, 0);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(328, 32);
            this.MainProgressBar.Step = 20;
            this.MainProgressBar.TabIndex = 0;
            // 
            // LevelTimer
            // 
            this.LevelTimer.Interval = 1000;
            this.LevelTimer.Tick += new System.EventHandler(this.LevelTimer_Tick);
            // 
            // TalkTimer
            // 
            this.TalkTimer.Interval = 900;
            this.TalkTimer.Tick += new System.EventHandler(this.TalkTimer_Tick);
            // 
            // CloseTimer
            // 
            this.CloseTimer.Interval = 1;
            this.CloseTimer.Tick += new System.EventHandler(this.CloseTimer_Tick);
            // 
            // LoadLevelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 32);
            this.ControlBox = false;
            this.Controls.Add(this.MainProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoadLevelForm";
            this.Text = "[Message]";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar MainProgressBar;
        private System.Windows.Forms.Timer LevelTimer;
        private System.Windows.Forms.Timer TalkTimer;
        private System.Windows.Forms.Timer CloseTimer;
    }
}
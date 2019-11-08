namespace SpotLight
{
    partial class AddObjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddObjectForm));
            this.ObjectSelectListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // ObjectSelectListView
            // 
            this.ObjectSelectListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.ObjectSelectListView.HideSelection = false;
            this.ObjectSelectListView.Location = new System.Drawing.Point(0, 0);
            this.ObjectSelectListView.Name = "ObjectSelectListView";
            this.ObjectSelectListView.Size = new System.Drawing.Size(384, 307);
            this.ObjectSelectListView.TabIndex = 0;
            this.ObjectSelectListView.UseCompatibleStateImageBehavior = false;
            this.ObjectSelectListView.View = System.Windows.Forms.View.Details;
            // 
            // AddObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 511);
            this.Controls.Add(this.ObjectSelectListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 550);
            this.Name = "AddObjectForm";
            this.ShowInTaskbar = false;
            this.Text = "Spotlight - Add Object";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ObjectSelectListView;
    }
}
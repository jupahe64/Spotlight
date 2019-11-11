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
            this.ClassNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CategoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjectDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.SelectObjectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ObjectSelectListView
            // 
            this.ObjectSelectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CategoryColumnHeader,
            this.ClassNameColumnHeader});
            this.ObjectSelectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectSelectListView.FullRowSelect = true;
            this.ObjectSelectListView.HideSelection = false;
            this.ObjectSelectListView.Location = new System.Drawing.Point(0, 0);
            this.ObjectSelectListView.MultiSelect = false;
            this.ObjectSelectListView.Name = "ObjectSelectListView";
            this.ObjectSelectListView.Size = new System.Drawing.Size(384, 340);
            this.ObjectSelectListView.TabIndex = 0;
            this.ObjectSelectListView.UseCompatibleStateImageBehavior = false;
            this.ObjectSelectListView.View = System.Windows.Forms.View.Details;
            // 
            // ClassNameColumnHeader
            // 
            this.ClassNameColumnHeader.Text = "Class Name";
            this.ClassNameColumnHeader.Width = 100;
            // 
            // CategoryColumnHeader
            // 
            this.CategoryColumnHeader.Text = "Category";
            // 
            // ObjectDescriptionTextBox
            // 
            this.ObjectDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ObjectDescriptionTextBox.Location = new System.Drawing.Point(0, 340);
            this.ObjectDescriptionTextBox.Multiline = true;
            this.ObjectDescriptionTextBox.Name = "ObjectDescriptionTextBox";
            this.ObjectDescriptionTextBox.ReadOnly = true;
            this.ObjectDescriptionTextBox.Size = new System.Drawing.Size(384, 148);
            this.ObjectDescriptionTextBox.TabIndex = 1;
            // 
            // SelectObjectButton
            // 
            this.SelectObjectButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SelectObjectButton.Location = new System.Drawing.Point(0, 488);
            this.SelectObjectButton.Name = "SelectObjectButton";
            this.SelectObjectButton.Size = new System.Drawing.Size(384, 23);
            this.SelectObjectButton.TabIndex = 2;
            this.SelectObjectButton.Text = "Select";
            this.SelectObjectButton.UseVisualStyleBackColor = true;
            // 
            // AddObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 511);
            this.Controls.Add(this.ObjectSelectListView);
            this.Controls.Add(this.ObjectDescriptionTextBox);
            this.Controls.Add(this.SelectObjectButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 550);
            this.Name = "AddObjectForm";
            this.ShowInTaskbar = false;
            this.Text = "Spotlight - Add Object";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ObjectSelectListView;
        private System.Windows.Forms.ColumnHeader ClassNameColumnHeader;
        private System.Windows.Forms.ColumnHeader CategoryColumnHeader;
        private System.Windows.Forms.TextBox ObjectDescriptionTextBox;
        private System.Windows.Forms.Button SelectObjectButton;
    }
}
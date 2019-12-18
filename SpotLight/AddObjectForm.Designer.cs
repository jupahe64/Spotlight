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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Areas", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Checkpoints", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Cutscenes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Goals", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Objects", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Starting Points", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Skies", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Linkables", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Rails", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("Uncategorized", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddObjectForm));
            this.ObjectSelectListView = new System.Windows.Forms.ListView();
            this.ClassNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjectDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.SelectObjectButton = new System.Windows.Forms.Button();
            this.ObjectNameGroupBox = new System.Windows.Forms.GroupBox();
            this.ObjectNameGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ObjectSelectListView
            // 
            this.ObjectSelectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClassNameColumnHeader});
            this.ObjectSelectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectSelectListView.FullRowSelect = true;
            this.ObjectSelectListView.GridLines = true;
            listViewGroup1.Header = "Areas";
            listViewGroup1.Name = "AreaListViewGroup";
            listViewGroup2.Header = "Checkpoints";
            listViewGroup2.Name = "CheckPointListViewGroup";
            listViewGroup3.Header = "Cutscenes";
            listViewGroup3.Name = "DemoListViewGroup";
            listViewGroup4.Header = "Goals";
            listViewGroup4.Name = "GoalListViewGroup";
            listViewGroup5.Header = "Objects";
            listViewGroup5.Name = "ObjectListViewGroup";
            listViewGroup6.Header = "Starting Points";
            listViewGroup6.Name = "PlayerListViewGroup";
            listViewGroup7.Header = "Skies";
            listViewGroup7.Name = "SkyListViewGroup";
            listViewGroup8.Header = "Linkables";
            listViewGroup8.Name = "LinkListViewGroup";
            listViewGroup9.Header = "Rails";
            listViewGroup9.Name = "RailListViewGroup";
            listViewGroup10.Header = "Uncategorized";
            listViewGroup10.Name = "UncategorizedListViewGroup";
            this.ObjectSelectListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10});
            this.ObjectSelectListView.HideSelection = false;
            this.ObjectSelectListView.Location = new System.Drawing.Point(0, 0);
            this.ObjectSelectListView.MultiSelect = false;
            this.ObjectSelectListView.Name = "ObjectSelectListView";
            this.ObjectSelectListView.Size = new System.Drawing.Size(384, 334);
            this.ObjectSelectListView.TabIndex = 0;
            this.ObjectSelectListView.UseCompatibleStateImageBehavior = false;
            this.ObjectSelectListView.View = System.Windows.Forms.View.Details;
            this.ObjectSelectListView.SelectedIndexChanged += new System.EventHandler(this.ObjectSelectListView_SelectedIndexChanged);
            // 
            // ClassNameColumnHeader
            // 
            this.ClassNameColumnHeader.Text = "Class Name";
            this.ClassNameColumnHeader.Width = 200;
            // 
            // ObjectDescriptionTextBox
            // 
            this.ObjectDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectDescriptionTextBox.Location = new System.Drawing.Point(3, 16);
            this.ObjectDescriptionTextBox.Multiline = true;
            this.ObjectDescriptionTextBox.Name = "ObjectDescriptionTextBox";
            this.ObjectDescriptionTextBox.Size = new System.Drawing.Size(378, 135);
            this.ObjectDescriptionTextBox.TabIndex = 1;
            this.ObjectDescriptionTextBox.TextChanged += new System.EventHandler(this.ObjectDescriptionTextBox_TextChanged);
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
            this.SelectObjectButton.Click += new System.EventHandler(this.SelectObjectButton_Click);
            // 
            // ObjectNameGroupBox
            // 
            this.ObjectNameGroupBox.Controls.Add(this.ObjectDescriptionTextBox);
            this.ObjectNameGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ObjectNameGroupBox.Location = new System.Drawing.Point(0, 334);
            this.ObjectNameGroupBox.Name = "ObjectNameGroupBox";
            this.ObjectNameGroupBox.Size = new System.Drawing.Size(384, 154);
            this.ObjectNameGroupBox.TabIndex = 3;
            this.ObjectNameGroupBox.TabStop = false;
            // 
            // AddObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 511);
            this.Controls.Add(this.ObjectSelectListView);
            this.Controls.Add(this.ObjectNameGroupBox);
            this.Controls.Add(this.SelectObjectButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 550);
            this.Name = "AddObjectForm";
            this.ShowInTaskbar = false;
            this.Text = "Spotlight - Add Object";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddObjectForm_FormClosing);
            this.ObjectNameGroupBox.ResumeLayout(false);
            this.ObjectNameGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ObjectSelectListView;
        private System.Windows.Forms.ColumnHeader ClassNameColumnHeader;
        private System.Windows.Forms.TextBox ObjectDescriptionTextBox;
        private System.Windows.Forms.Button SelectObjectButton;
        private System.Windows.Forms.GroupBox ObjectNameGroupBox;
    }
}
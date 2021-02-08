namespace Spotlight
{
    partial class LevelSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelSelectForm));
            this.LevelsListView = new System.Windows.Forms.ListView();
            this.CourseIDColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LevelNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ChooseLevelButton = new System.Windows.Forms.Button();
            this.SectionComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // LevelsListView
            // 
            this.LevelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CourseIDColumnHeader,
            this.LevelNameColumnHeader});
            this.LevelsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LevelsListView.FullRowSelect = true;
            this.LevelsListView.GridLines = true;
            this.LevelsListView.HideSelection = false;
            this.LevelsListView.Location = new System.Drawing.Point(0, 21);
            this.LevelsListView.MultiSelect = false;
            this.LevelsListView.Name = "LevelsListView";
            this.LevelsListView.Size = new System.Drawing.Size(326, 264);
            this.LevelsListView.TabIndex = 0;
            this.LevelsListView.UseCompatibleStateImageBehavior = false;
            this.LevelsListView.View = System.Windows.Forms.View.Details;
            this.LevelsListView.SelectedIndexChanged += new System.EventHandler(this.LevelsListView_SelectedIndexChanged);
            this.LevelsListView.DoubleClick += new System.EventHandler(this.LevelsListView_DoubleClick);
            // 
            // CourseIDColumnHeader
            // 
            this.CourseIDColumnHeader.Text = "CourseID";
            this.CourseIDColumnHeader.Width = 65;
            // 
            // LevelNameColumnHeader
            // 
            this.LevelNameColumnHeader.Text = "Level Name";
            this.LevelNameColumnHeader.Width = 220;
            // 
            // ChooseLevelButton
            // 
            this.ChooseLevelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ChooseLevelButton.Location = new System.Drawing.Point(0, 285);
            this.ChooseLevelButton.Name = "ChooseLevelButton";
            this.ChooseLevelButton.Size = new System.Drawing.Size(326, 23);
            this.ChooseLevelButton.TabIndex = 1;
            this.ChooseLevelButton.Text = "Select";
            this.ChooseLevelButton.UseVisualStyleBackColor = true;
            this.ChooseLevelButton.Click += new System.EventHandler(this.ChooseLevelButton_Click);
            // 
            // SectionComboBox
            // 
            this.SectionComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.SectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SectionComboBox.FormattingEnabled = true;
            this.SectionComboBox.Items.AddRange(new object[] {
            "World 1",
            "World 2",
            "World 3",
            "World 4",
            "World 5",
            "World 6",
            "World Castle",
            "World Bowser",
            "World Star",
            "World Mushroom",
            "World Flower",
            "World Crown",
            "Misc",
            "Bowsers Fury"});
            this.SectionComboBox.Location = new System.Drawing.Point(0, 0);
            this.SectionComboBox.Name = "SectionComboBox";
            this.SectionComboBox.Size = new System.Drawing.Size(326, 21);
            this.SectionComboBox.TabIndex = 2;
            this.SectionComboBox.SelectedIndexChanged += new System.EventHandler(this.SectionComboBox_SelectedIndexChanged);
            // 
            // LevelSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 308);
            this.Controls.Add(this.LevelsListView);
            this.Controls.Add(this.SectionComboBox);
            this.Controls.Add(this.ChooseLevelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(342, 347);
            this.Name = "LevelSelectForm";
            this.Text = "Spotlight - Choose a Level";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelParamSelectForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LevelsListView;
        private System.Windows.Forms.Button ChooseLevelButton;
        private System.Windows.Forms.ColumnHeader LevelNameColumnHeader;
        private System.Windows.Forms.ColumnHeader CourseIDColumnHeader;
        private System.Windows.Forms.ComboBox SectionComboBox;
    }
}
namespace Spotlight.GUI
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ObjDBListView = new BrightIdeasSoftware.FastObjectListView();
            this.ClassNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.EnglishNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ObjListColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ObjectDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.EnglishNameTextBox = new System.Windows.Forms.TextBox();
            this.ClassNameLabel = new System.Windows.Forms.Label();
            this.PropertyListBox = new System.Windows.Forms.ListBox();
            this.PropertiesLabel = new System.Windows.Forms.Label();
            this.PropertyDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ToQuickFavoritesButton = new System.Windows.Forms.Button();
            this.SelectObjectButton = new System.Windows.Forms.Button();
            this.ObjectNameLabel = new System.Windows.Forms.Label();
            this.ModelNameLabel = new System.Windows.Forms.Label();
            this.SharedContentPanel = new System.Windows.Forms.Panel();
            this.ObjectTypeTabControl = new System.Windows.Forms.TabControl();
            this.ObjectsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ObjectNameTextBox = new GL_EditorFramework.SuggestingTextBox();
            this.ModelNameTextBox = new GL_EditorFramework.SuggestingTextBox();
            this.RailsTab = new System.Windows.Forms.TabPage();
            this.PathShapeSelector = new Spotlight.GUI.PathShapeSelector();
            this.AreasTab = new System.Windows.Forms.TabPage();
            this.AreaShapeComboBox = new System.Windows.Forms.ComboBox();
            this.AreaShapeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ObjDBListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SharedContentPanel.SuspendLayout();
            this.ObjectTypeTabControl.SuspendLayout();
            this.ObjectsTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.RailsTab.SuspendLayout();
            this.AreasTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer1.Panel1.Controls.Add(this.ObjDBListView);
            this.splitContainer1.Panel1.Controls.Add(this.SearchTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.SearchLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(868, 425);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 0;
            // 
            // ObjDBListView
            // 
            this.ObjDBListView.AllColumns.Add(this.ClassNameColumn);
            this.ObjDBListView.AllColumns.Add(this.EnglishNameColumn);
            this.ObjDBListView.AllColumns.Add(this.ObjListColumn);
            this.ObjDBListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjDBListView.CellEditUseWholeCell = false;
            this.ObjDBListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClassNameColumn,
            this.EnglishNameColumn});
            this.ObjDBListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.ObjDBListView.EmptyListMsg = "No Items Found";
            this.ObjDBListView.FullRowSelect = true;
            this.ObjDBListView.HideSelection = false;
            this.ObjDBListView.Location = new System.Drawing.Point(6, 32);
            this.ObjDBListView.Name = "ObjDBListView";
            this.ObjDBListView.ShowGroups = false;
            this.ObjDBListView.Size = new System.Drawing.Size(276, 391);
            this.ObjDBListView.TabIndex = 6;
            this.ObjDBListView.UseCompatibleStateImageBehavior = false;
            this.ObjDBListView.View = System.Windows.Forms.View.Details;
            this.ObjDBListView.VirtualMode = true;
            this.ObjDBListView.FullRowSelect = true;
            this.ObjDBListView.MultiSelect = false;
            this.ObjDBListView.SelectionChanged += new System.EventHandler(this.ObjDBListView_SelectionChanged);
            // 
            // ClassNameColumn
            // 
            this.ClassNameColumn.Groupable = false;
            this.ClassNameColumn.IsEditable = false;
            this.ClassNameColumn.MinimumWidth = 50;
            this.ClassNameColumn.Text = "Class Name";
            this.ClassNameColumn.Width = 104;
            // 
            // EnglishNameColumn
            // 
            this.EnglishNameColumn.Groupable = false;
            this.EnglishNameColumn.IsEditable = false;
            this.EnglishNameColumn.MinimumWidth = 50;
            this.EnglishNameColumn.Text = "English Name";
            this.EnglishNameColumn.Width = 109;
            // 
            // ObjListColumn
            // 
            this.ObjListColumn.IsEditable = false;
            this.ObjListColumn.IsVisible = false;
            this.ObjListColumn.MinimumWidth = 30;
            this.ObjListColumn.Text = "List";
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchTextBox.Location = new System.Drawing.Point(53, 5);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(229, 20);
            this.SearchTextBox.TabIndex = 1;
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Location = new System.Drawing.Point(6, 5);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.SearchLabel.Size = new System.Drawing.Size(44, 15);
            this.SearchLabel.TabIndex = 5;
            this.SearchLabel.Text = "Search:";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer2.Panel1.Controls.Add(this.ObjectDescriptionTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.EnglishNameTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.ClassNameLabel);
            this.splitContainer2.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.SplitContainer2_Panel2_Paint);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.PropertyListBox);
            this.splitContainer2.Panel2.Controls.Add(this.PropertiesLabel);
            this.splitContainer2.Panel2.Controls.Add(this.PropertyDescriptionTextBox);
            this.splitContainer2.Size = new System.Drawing.Size(573, 425);
            this.splitContainer2.SplitterDistance = 189;
            this.splitContainer2.TabIndex = 12;
            // 
            // ObjectDescriptionTextBox
            // 
            this.ObjectDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectDescriptionTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.ObjectDescriptionTextBox.Location = new System.Drawing.Point(3, 55);
            this.ObjectDescriptionTextBox.Multiline = true;
            this.ObjectDescriptionTextBox.Name = "ObjectDescriptionTextBox";
            this.ObjectDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ObjectDescriptionTextBox.Size = new System.Drawing.Size(563, 131);
            this.ObjectDescriptionTextBox.TabIndex = 8;
            this.ObjectDescriptionTextBox.Text = "A normal Goomba";
            this.ObjectDescriptionTextBox.TextChanged += new System.EventHandler(this.ObjectDescriptionTextBox_TextChanged);
            // 
            // EnglishNameTextBox
            // 
            this.EnglishNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EnglishNameTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.EnglishNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EnglishNameTextBox.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnglishNameTextBox.Location = new System.Drawing.Point(4, 6);
            this.EnglishNameTextBox.Name = "EnglishNameTextBox";
            this.EnglishNameTextBox.Size = new System.Drawing.Size(561, 23);
            this.EnglishNameTextBox.TabIndex = 7;
            this.EnglishNameTextBox.Text = "Goomba";
            this.EnglishNameTextBox.TextChanged += new System.EventHandler(this.EnglishNameTextBox_TextChanged);
            this.EnglishNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EnglishNameTextBox_KeyDown);
            // 
            // ClassNameLabel
            // 
            this.ClassNameLabel.AutoSize = true;
            this.ClassNameLabel.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ClassNameLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.ClassNameLabel.Location = new System.Drawing.Point(3, 31);
            this.ClassNameLabel.Name = "ClassNameLabel";
            this.ClassNameLabel.Size = new System.Drawing.Size(46, 15);
            this.ClassNameLabel.TabIndex = 9;
            this.ClassNameLabel.Text = "Kuribo";
            // 
            // PropertyListBox
            // 
            this.PropertyListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertyListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.PropertyListBox.FormattingEnabled = true;
            this.PropertyListBox.IntegralHeight = false;
            this.PropertyListBox.ItemHeight = 20;
            this.PropertyListBox.Location = new System.Drawing.Point(3, 20);
            this.PropertyListBox.Name = "PropertyListBox";
            this.PropertyListBox.Size = new System.Drawing.Size(236, 210);
            this.PropertyListBox.TabIndex = 15;
            this.PropertyListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.PropertyListBox_DrawItem);
            this.PropertyListBox.SelectedIndexChanged += new System.EventHandler(this.PropertyListBox_SelectedIndexChanged);
            // 
            // PropertiesLabel
            // 
            this.PropertiesLabel.AutoSize = true;
            this.PropertiesLabel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.PropertiesLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.PropertiesLabel.Location = new System.Drawing.Point(3, 0);
            this.PropertiesLabel.Name = "PropertiesLabel";
            this.PropertiesLabel.Size = new System.Drawing.Size(76, 17);
            this.PropertiesLabel.TabIndex = 11;
            this.PropertiesLabel.Text = "Properties";
            // 
            // PropertyDescriptionTextBox
            // 
            this.PropertyDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertyDescriptionTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.PropertyDescriptionTextBox.Enabled = false;
            this.PropertyDescriptionTextBox.Location = new System.Drawing.Point(245, 20);
            this.PropertyDescriptionTextBox.Multiline = true;
            this.PropertyDescriptionTextBox.Name = "PropertyDescriptionTextBox";
            this.PropertyDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PropertyDescriptionTextBox.Size = new System.Drawing.Size(321, 210);
            this.PropertyDescriptionTextBox.TabIndex = 10;
            this.PropertyDescriptionTextBox.TextChanged += new System.EventHandler(this.PropertyDescriptionTextBox_TextChanged);
            // 
            // ToQuickFavoritesButton
            // 
            this.ToQuickFavoritesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ToQuickFavoritesButton.Enabled = false;
            this.ToQuickFavoritesButton.Location = new System.Drawing.Point(749, 524);
            this.ToQuickFavoritesButton.Name = "ToQuickFavoritesButton";
            this.ToQuickFavoritesButton.Size = new System.Drawing.Size(132, 30);
            this.ToQuickFavoritesButton.TabIndex = 6;
            this.ToQuickFavoritesButton.Text = "To QuickFavorites";
            this.ToQuickFavoritesButton.UseVisualStyleBackColor = true;
            this.ToQuickFavoritesButton.Click += new System.EventHandler(this.ToQuickFavoritesButton_Click);
            // 
            // SelectObjectButton
            // 
            this.SelectObjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectObjectButton.Enabled = false;
            this.SelectObjectButton.Location = new System.Drawing.Point(6, 524);
            this.SelectObjectButton.Name = "SelectObjectButton";
            this.SelectObjectButton.Size = new System.Drawing.Size(737, 30);
            this.SelectObjectButton.TabIndex = 5;
            this.SelectObjectButton.Text = "Select";
            this.SelectObjectButton.UseVisualStyleBackColor = true;
            this.SelectObjectButton.Click += new System.EventHandler(this.SelectObjectButton_Click);
            // 
            // ObjectNameLabel
            // 
            this.ObjectNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ObjectNameLabel.AutoSize = true;
            this.ObjectNameLabel.Location = new System.Drawing.Point(3, 5);
            this.ObjectNameLabel.Name = "ObjectNameLabel";
            this.ObjectNameLabel.Size = new System.Drawing.Size(69, 13);
            this.ObjectNameLabel.TabIndex = 6;
            this.ObjectNameLabel.Text = "Object Name";
            // 
            // ModelNameLabel
            // 
            this.ModelNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ModelNameLabel.AutoSize = true;
            this.ModelNameLabel.Location = new System.Drawing.Point(433, 5);
            this.ModelNameLabel.Name = "ModelNameLabel";
            this.ModelNameLabel.Size = new System.Drawing.Size(67, 13);
            this.ModelNameLabel.TabIndex = 9;
            this.ModelNameLabel.Text = "Model Name";
            // 
            // SharedContentPanel
            // 
            this.SharedContentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SharedContentPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.SharedContentPanel.Controls.Add(this.splitContainer1);
            this.SharedContentPanel.Location = new System.Drawing.Point(10, 29);
            this.SharedContentPanel.Name = "SharedContentPanel";
            this.SharedContentPanel.Size = new System.Drawing.Size(868, 425);
            this.SharedContentPanel.TabIndex = 10;
            // 
            // ObjectTypeTabControl
            // 
            this.ObjectTypeTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectTypeTabControl.Controls.Add(this.ObjectsTab);
            this.ObjectTypeTabControl.Controls.Add(this.RailsTab);
            this.ObjectTypeTabControl.Controls.Add(this.AreasTab);
            this.ObjectTypeTabControl.Location = new System.Drawing.Point(6, 6);
            this.ObjectTypeTabControl.Name = "ObjectTypeTabControl";
            this.ObjectTypeTabControl.SelectedIndex = 0;
            this.ObjectTypeTabControl.Size = new System.Drawing.Size(876, 509);
            this.ObjectTypeTabControl.TabIndex = 12;
            this.ObjectTypeTabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // ObjectsTab
            // 
            this.ObjectsTab.Controls.Add(this.tableLayoutPanel1);
            this.ObjectsTab.Location = new System.Drawing.Point(4, 22);
            this.ObjectsTab.Name = "ObjectsTab";
            this.ObjectsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectsTab.Size = new System.Drawing.Size(868, 483);
            this.ObjectsTab.TabIndex = 0;
            this.ObjectsTab.Text = "Objects";
            this.ObjectsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ObjectNameTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ModelNameLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ObjectNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ModelNameTextBox, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 434);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.13044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.86956F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(861, 46);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // ObjectNameTextBox
            // 
            this.ObjectNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectNameTextBox.Enabled = false;
            this.ObjectNameTextBox.FilterSuggestions = true;
            this.ObjectNameTextBox.Location = new System.Drawing.Point(3, 21);
            this.ObjectNameTextBox.Name = "ObjectNameTextBox";
            this.ObjectNameTextBox.PossibleSuggestions = new string[0];
            this.ObjectNameTextBox.Size = new System.Drawing.Size(424, 20);
            this.ObjectNameTextBox.SuggestClear = false;
            this.ObjectNameTextBox.TabIndex = 3;
            this.ObjectNameTextBox.TabStop = false;
            // 
            // ModelNameTextBox
            // 
            this.ModelNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModelNameTextBox.Enabled = false;
            this.ModelNameTextBox.FilterSuggestions = true;
            this.ModelNameTextBox.Location = new System.Drawing.Point(433, 21);
            this.ModelNameTextBox.Name = "ModelNameTextBox";
            this.ModelNameTextBox.PossibleSuggestions = new string[0];
            this.ModelNameTextBox.Size = new System.Drawing.Size(425, 20);
            this.ModelNameTextBox.SuggestClear = true;
            this.ModelNameTextBox.TabIndex = 4;
            this.ModelNameTextBox.TabStop = false;
            // 
            // RailsTab
            // 
            this.RailsTab.Controls.Add(this.PathShapeSelector);
            this.RailsTab.Location = new System.Drawing.Point(4, 22);
            this.RailsTab.Name = "RailsTab";
            this.RailsTab.Padding = new System.Windows.Forms.Padding(3);
            this.RailsTab.Size = new System.Drawing.Size(868, 483);
            this.RailsTab.TabIndex = 1;
            this.RailsTab.Text = "Rails";
            this.RailsTab.UseVisualStyleBackColor = true;
            // 
            // PathShapeSelector
            // 
            this.PathShapeSelector.Location = new System.Drawing.Point(6, 430);
            this.PathShapeSelector.Name = "PathShapeSelector";
            this.PathShapeSelector.Size = new System.Drawing.Size(854, 50);
            this.PathShapeSelector.TabIndex = 11;
            // 
            // AreasTab
            // 
            this.AreasTab.Controls.Add(this.AreaShapeComboBox);
            this.AreasTab.Controls.Add(this.AreaShapeLabel);
            this.AreasTab.Location = new System.Drawing.Point(4, 22);
            this.AreasTab.Name = "AreasTab";
            this.AreasTab.Padding = new System.Windows.Forms.Padding(3);
            this.AreasTab.Size = new System.Drawing.Size(868, 483);
            this.AreasTab.TabIndex = 2;
            this.AreasTab.Text = "Areas";
            this.AreasTab.UseVisualStyleBackColor = true;
            // 
            // AreaShapeComboBox
            // 
            this.AreaShapeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AreaShapeComboBox.Enabled = false;
            this.AreaShapeComboBox.FormattingEnabled = true;
            this.AreaShapeComboBox.Location = new System.Drawing.Point(6, 456);
            this.AreaShapeComboBox.Name = "AreaShapeComboBox";
            this.AreaShapeComboBox.Size = new System.Drawing.Size(854, 21);
            this.AreaShapeComboBox.TabIndex = 12;
            // 
            // AreaShapeLabel
            // 
            this.AreaShapeLabel.AutoSize = true;
            this.AreaShapeLabel.Location = new System.Drawing.Point(6, 440);
            this.AreaShapeLabel.Name = "AreaShapeLabel";
            this.AreaShapeLabel.Size = new System.Drawing.Size(63, 13);
            this.AreaShapeLabel.TabIndex = 11;
            this.AreaShapeLabel.Text = "Area Shape";
            // 
            // AddObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 561);
            this.Controls.Add(this.ToQuickFavoritesButton);
            this.Controls.Add(this.SelectObjectButton);
            this.Controls.Add(this.SharedContentPanel);
            this.Controls.Add(this.ObjectTypeTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddObjectForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Spotlight - Add Object";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewAddObjectForm_FormClosing);
            this.Load += new System.EventHandler(this.NewAddObjectForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ObjDBListView)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.SharedContentPanel.ResumeLayout(false);
            this.ObjectTypeTabControl.ResumeLayout(false);
            this.ObjectsTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.RailsTab.ResumeLayout(false);
            this.AreasTab.ResumeLayout(false);
            this.AreasTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.TextBox PropertyDescriptionTextBox;
        private System.Windows.Forms.TextBox ObjectDescriptionTextBox;
        private System.Windows.Forms.Label ClassNameLabel;
        private System.Windows.Forms.TextBox EnglishNameTextBox;
        private GL_EditorFramework.SuggestingTextBox ObjectNameTextBox;
        private System.Windows.Forms.Button ToQuickFavoritesButton;
        private System.Windows.Forms.Button SelectObjectButton;
        private System.Windows.Forms.Label ObjectNameLabel;
        private System.Windows.Forms.Label ModelNameLabel;
        private GL_EditorFramework.SuggestingTextBox ModelNameTextBox;
        private System.Windows.Forms.Panel SharedContentPanel;
        private System.Windows.Forms.TabControl ObjectTypeTabControl;
        private System.Windows.Forms.TabPage ObjectsTab;
        private System.Windows.Forms.TabPage RailsTab;
        private System.Windows.Forms.Label PropertiesLabel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox PropertyListBox;
        private BrightIdeasSoftware.FastObjectListView ObjDBListView;
        private BrightIdeasSoftware.OLVColumn ClassNameColumn;
        private BrightIdeasSoftware.OLVColumn EnglishNameColumn;
        private BrightIdeasSoftware.OLVColumn ObjListColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabPage AreasTab;
        private System.Windows.Forms.ComboBox AreaShapeComboBox;
        private System.Windows.Forms.Label AreaShapeLabel;
        private Spotlight.GUI.PathShapeSelector PathShapeSelector;
    }
}
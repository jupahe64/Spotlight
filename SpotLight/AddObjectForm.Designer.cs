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
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("Search Results: {RESULT}", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddObjectForm));
            this.DBEntryListView = new System.Windows.Forms.ListView();
            this.ClassNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EnglishNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjectCountColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ModelCountColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjectDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.SelectObjectButton = new System.Windows.Forms.Button();
            this.ObjectTypeTabControl = new System.Windows.Forms.TabControl();
            this.ObjectFromDBTab = new System.Windows.Forms.TabPage();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.AddSettingsPanel = new System.Windows.Forms.Panel();
            this.AddSettingsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.SelectObjectListView = new System.Windows.Forms.ListView();
            this.ObjectNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SelectObjectLabel = new System.Windows.Forms.Label();
            this.SelectModelListView = new System.Windows.Forms.ListView();
            this.ModelNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SelectModelLabel = new System.Windows.Forms.Label();
            this.ClassNameLabel = new System.Windows.Forms.Label();
            this.EnglishNameTextBox = new System.Windows.Forms.TextBox();
            this.PropertyNotesListView = new System.Windows.Forms.ListView();
            this.NameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DescriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertyHintTextBox = new System.Windows.Forms.TextBox();
            this.PropertyLabel = new System.Windows.Forms.Label();
            this.RailTab = new System.Windows.Forms.TabPage();
            this.RailFormationListView = new System.Windows.Forms.ListView();
            this.RailNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RailDescriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RailBooleanPanel = new System.Windows.Forms.Panel();
            this.ReverseRailCheckBox = new System.Windows.Forms.CheckBox();
            this.CloseRailCheckBox = new System.Windows.Forms.CheckBox();
            this.RailTypePanel = new System.Windows.Forms.Panel();
            this.RailTypeComboBox = new System.Windows.Forms.ComboBox();
            this.RailTypeLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ToQuickFavoritesButton = new System.Windows.Forms.Button();
            this.DeselectModelButton = new System.Windows.Forms.Button();
            this.ObjectTypeTabControl.SuspendLayout();
            this.ObjectFromDBTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.SearchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.AddSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AddSettingsSplitContainer)).BeginInit();
            this.AddSettingsSplitContainer.Panel1.SuspendLayout();
            this.AddSettingsSplitContainer.Panel2.SuspendLayout();
            this.AddSettingsSplitContainer.SuspendLayout();
            this.RailTab.SuspendLayout();
            this.RailBooleanPanel.SuspendLayout();
            this.RailTypePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DBEntryListView
            // 
            this.DBEntryListView.AutoArrange = false;
            this.DBEntryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClassNameColumnHeader,
            this.EnglishNameColumnHeader,
            this.ObjectCountColumnHeader,
            this.ModelCountColumnHeader});
            this.DBEntryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DBEntryListView.FullRowSelect = true;
            this.DBEntryListView.GridLines = true;
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
            listViewGroup11.Header = "Search Results: {RESULT}";
            listViewGroup11.Name = "SearchResultListViewGroup";
            this.DBEntryListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10,
            listViewGroup11});
            this.DBEntryListView.HideSelection = false;
            this.DBEntryListView.Location = new System.Drawing.Point(0, 22);
            this.DBEntryListView.MultiSelect = false;
            this.DBEntryListView.Name = "DBEntryListView";
            this.DBEntryListView.Size = new System.Drawing.Size(273, 418);
            this.DBEntryListView.TabIndex = 0;
            this.DBEntryListView.UseCompatibleStateImageBehavior = false;
            this.DBEntryListView.View = System.Windows.Forms.View.Details;
            this.DBEntryListView.SelectedIndexChanged += new System.EventHandler(this.ObjectSelectListView_SelectedIndexChanged);
            this.DBEntryListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DBEntryListView_MouseDoubleClick);
            // 
            // ClassNameColumnHeader
            // 
            this.ClassNameColumnHeader.Text = "Class Name";
            this.ClassNameColumnHeader.Width = 134;
            // 
            // EnglishNameColumnHeader
            // 
            this.EnglishNameColumnHeader.Text = "Name";
            this.EnglishNameColumnHeader.Width = 120;
            // 
            // ObjectCountColumnHeader
            // 
            this.ObjectCountColumnHeader.Text = "Objects";
            this.ObjectCountColumnHeader.Width = 48;
            // 
            // ModelCountColumnHeader
            // 
            this.ModelCountColumnHeader.Text = "Models";
            this.ModelCountColumnHeader.Width = 47;
            // 
            // ObjectDescriptionTextBox
            // 
            this.ObjectDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ObjectDescriptionTextBox.Location = new System.Drawing.Point(0, 53);
            this.ObjectDescriptionTextBox.Multiline = true;
            this.ObjectDescriptionTextBox.Name = "ObjectDescriptionTextBox";
            this.ObjectDescriptionTextBox.Size = new System.Drawing.Size(493, 86);
            this.ObjectDescriptionTextBox.TabIndex = 1;
            this.ObjectDescriptionTextBox.TextChanged += new System.EventHandler(this.ObjectDescriptionTextBox_TextChanged);
            // 
            // SelectObjectButton
            // 
            this.SelectObjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectObjectButton.Location = new System.Drawing.Point(10, 6);
            this.SelectObjectButton.Name = "SelectObjectButton";
            this.SelectObjectButton.Size = new System.Drawing.Size(627, 30);
            this.SelectObjectButton.TabIndex = 2;
            this.SelectObjectButton.Text = "Select";
            this.SelectObjectButton.UseVisualStyleBackColor = true;
            this.SelectObjectButton.Click += new System.EventHandler(this.SelectObjectButton_Click);
            // 
            // ObjectTypeTabControl
            // 
            this.ObjectTypeTabControl.Controls.Add(this.ObjectFromDBTab);
            this.ObjectTypeTabControl.Controls.Add(this.RailTab);
            this.ObjectTypeTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectTypeTabControl.Location = new System.Drawing.Point(0, 0);
            this.ObjectTypeTabControl.Name = "ObjectTypeTabControl";
            this.ObjectTypeTabControl.SelectedIndex = 0;
            this.ObjectTypeTabControl.Size = new System.Drawing.Size(784, 472);
            this.ObjectTypeTabControl.TabIndex = 4;
            // 
            // ObjectFromDBTab
            // 
            this.ObjectFromDBTab.Controls.Add(this.MainSplitContainer);
            this.ObjectFromDBTab.Location = new System.Drawing.Point(4, 22);
            this.ObjectFromDBTab.Name = "ObjectFromDBTab";
            this.ObjectFromDBTab.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectFromDBTab.Size = new System.Drawing.Size(776, 446);
            this.ObjectFromDBTab.TabIndex = 0;
            this.ObjectFromDBTab.Text = "Objects";
            this.ObjectFromDBTab.UseVisualStyleBackColor = true;
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.DBEntryListView);
            this.MainSplitContainer.Panel1.Controls.Add(this.SearchPanel);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.splitContainer1);
            this.MainSplitContainer.Size = new System.Drawing.Size(770, 440);
            this.MainSplitContainer.SplitterDistance = 273;
            this.MainSplitContainer.TabIndex = 5;
            // 
            // SearchPanel
            // 
            this.SearchPanel.Controls.Add(this.SearchTextBox);
            this.SearchPanel.Controls.Add(this.SearchLabel);
            this.SearchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SearchPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(273, 22);
            this.SearchPanel.TabIndex = 3;
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchTextBox.Location = new System.Drawing.Point(44, 0);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(229, 20);
            this.SearchTextBox.TabIndex = 1;
            this.SearchTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchTextBox_KeyUp);
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SearchLabel.Location = new System.Drawing.Point(0, 0);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(44, 13);
            this.SearchLabel.TabIndex = 2;
            this.SearchLabel.Text = "Search:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.AddSettingsPanel);
            this.splitContainer1.Panel1.Controls.Add(this.ObjectDescriptionTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.ClassNameLabel);
            this.splitContainer1.Panel1.Controls.Add(this.EnglishNameTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PropertyNotesListView);
            this.splitContainer1.Panel2.Controls.Add(this.PropertyHintTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.PropertyLabel);
            this.splitContainer1.Size = new System.Drawing.Size(493, 440);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 8;
            // 
            // AddSettingsPanel
            // 
            this.AddSettingsPanel.Controls.Add(this.AddSettingsSplitContainer);
            this.AddSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddSettingsPanel.Location = new System.Drawing.Point(0, 139);
            this.AddSettingsPanel.Name = "AddSettingsPanel";
            this.AddSettingsPanel.Size = new System.Drawing.Size(493, 119);
            this.AddSettingsPanel.TabIndex = 3;
            // 
            // AddSettingsSplitContainer
            // 
            this.AddSettingsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddSettingsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.AddSettingsSplitContainer.Name = "AddSettingsSplitContainer";
            // 
            // AddSettingsSplitContainer.Panel1
            // 
            this.AddSettingsSplitContainer.Panel1.Controls.Add(this.SelectObjectListView);
            this.AddSettingsSplitContainer.Panel1.Controls.Add(this.SelectObjectLabel);
            // 
            // AddSettingsSplitContainer.Panel2
            // 
            this.AddSettingsSplitContainer.Panel2.Controls.Add(this.DeselectModelButton);
            this.AddSettingsSplitContainer.Panel2.Controls.Add(this.SelectModelListView);
            this.AddSettingsSplitContainer.Panel2.Controls.Add(this.SelectModelLabel);
            this.AddSettingsSplitContainer.Size = new System.Drawing.Size(493, 119);
            this.AddSettingsSplitContainer.SplitterDistance = 235;
            this.AddSettingsSplitContainer.TabIndex = 4;
            // 
            // SelectObjectListView
            // 
            this.SelectObjectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ObjectNameColumnHeader});
            this.SelectObjectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectObjectListView.FullRowSelect = true;
            this.SelectObjectListView.HideSelection = false;
            this.SelectObjectListView.Location = new System.Drawing.Point(0, 17);
            this.SelectObjectListView.MultiSelect = false;
            this.SelectObjectListView.Name = "SelectObjectListView";
            this.SelectObjectListView.Size = new System.Drawing.Size(235, 102);
            this.SelectObjectListView.TabIndex = 4;
            this.SelectObjectListView.UseCompatibleStateImageBehavior = false;
            this.SelectObjectListView.View = System.Windows.Forms.View.Details;
            // 
            // ObjectNameColumnHeader
            // 
            this.ObjectNameColumnHeader.Text = "Name";
            this.ObjectNameColumnHeader.Width = 217;
            // 
            // SelectObjectLabel
            // 
            this.SelectObjectLabel.AutoSize = true;
            this.SelectObjectLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SelectObjectLabel.Font = new System.Drawing.Font("Arial Black", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectObjectLabel.Location = new System.Drawing.Point(0, 0);
            this.SelectObjectLabel.Name = "SelectObjectLabel";
            this.SelectObjectLabel.Size = new System.Drawing.Size(101, 17);
            this.SelectObjectLabel.TabIndex = 3;
            this.SelectObjectLabel.Text = "Select Object:";
            // 
            // SelectModelListView
            // 
            this.SelectModelListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ModelNameColumnHeader});
            this.SelectModelListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectModelListView.FullRowSelect = true;
            this.SelectModelListView.HideSelection = false;
            this.SelectModelListView.Location = new System.Drawing.Point(0, 17);
            this.SelectModelListView.MultiSelect = false;
            this.SelectModelListView.Name = "SelectModelListView";
            this.SelectModelListView.Size = new System.Drawing.Size(254, 102);
            this.SelectModelListView.TabIndex = 6;
            this.SelectModelListView.UseCompatibleStateImageBehavior = false;
            this.SelectModelListView.View = System.Windows.Forms.View.Details;
            // 
            // ModelNameColumnHeader
            // 
            this.ModelNameColumnHeader.Text = "Name";
            this.ModelNameColumnHeader.Width = 235;
            // 
            // SelectModelLabel
            // 
            this.SelectModelLabel.AutoSize = true;
            this.SelectModelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SelectModelLabel.Font = new System.Drawing.Font("Arial Black", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectModelLabel.Location = new System.Drawing.Point(0, 0);
            this.SelectModelLabel.Name = "SelectModelLabel";
            this.SelectModelLabel.Size = new System.Drawing.Size(97, 17);
            this.SelectModelLabel.TabIndex = 5;
            this.SelectModelLabel.Text = "Select Model:";
            // 
            // ClassNameLabel
            // 
            this.ClassNameLabel.AutoSize = true;
            this.ClassNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ClassNameLabel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClassNameLabel.Location = new System.Drawing.Point(0, 30);
            this.ClassNameLabel.Name = "ClassNameLabel";
            this.ClassNameLabel.Size = new System.Drawing.Size(163, 23);
            this.ClassNameLabel.TabIndex = 2;
            this.ClassNameLabel.Text = "Nothing Selected";
            // 
            // EnglishNameTextBox
            // 
            this.EnglishNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.EnglishNameTextBox.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnglishNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.EnglishNameTextBox.Name = "EnglishNameTextBox";
            this.EnglishNameTextBox.Size = new System.Drawing.Size(493, 30);
            this.EnglishNameTextBox.TabIndex = 7;
            this.EnglishNameTextBox.Text = "Nothing Selected";
            this.EnglishNameTextBox.TextChanged += new System.EventHandler(this.EnglishNameTextBox_TextChanged);
            // 
            // PropertyNotesListView
            // 
            this.PropertyNotesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader,
            this.TypeColumnHeader,
            this.DescriptionColumnHeader});
            this.PropertyNotesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyNotesListView.FullRowSelect = true;
            this.PropertyNotesListView.GridLines = true;
            this.PropertyNotesListView.HideSelection = false;
            this.PropertyNotesListView.Location = new System.Drawing.Point(0, 23);
            this.PropertyNotesListView.MultiSelect = false;
            this.PropertyNotesListView.Name = "PropertyNotesListView";
            this.PropertyNotesListView.Size = new System.Drawing.Size(493, 100);
            this.PropertyNotesListView.TabIndex = 4;
            this.PropertyNotesListView.UseCompatibleStateImageBehavior = false;
            this.PropertyNotesListView.View = System.Windows.Forms.View.Details;
            this.PropertyNotesListView.SelectedIndexChanged += new System.EventHandler(this.PropertyNotesListView_SelectedIndexChanged);
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Property Name";
            this.NameColumnHeader.Width = 133;
            // 
            // TypeColumnHeader
            // 
            this.TypeColumnHeader.Text = "Type";
            this.TypeColumnHeader.Width = 74;
            // 
            // DescriptionColumnHeader
            // 
            this.DescriptionColumnHeader.Text = "Description";
            this.DescriptionColumnHeader.Width = 285;
            // 
            // PropertyHintTextBox
            // 
            this.PropertyHintTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PropertyHintTextBox.Location = new System.Drawing.Point(0, 123);
            this.PropertyHintTextBox.Multiline = true;
            this.PropertyHintTextBox.Name = "PropertyHintTextBox";
            this.PropertyHintTextBox.Size = new System.Drawing.Size(493, 55);
            this.PropertyHintTextBox.TabIndex = 6;
            this.PropertyHintTextBox.TextChanged += new System.EventHandler(this.PropertyHintTextBox_TextChanged);
            // 
            // PropertyLabel
            // 
            this.PropertyLabel.AutoSize = true;
            this.PropertyLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PropertyLabel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PropertyLabel.Location = new System.Drawing.Point(0, 0);
            this.PropertyLabel.Name = "PropertyLabel";
            this.PropertyLabel.Size = new System.Drawing.Size(132, 23);
            this.PropertyLabel.TabIndex = 5;
            this.PropertyLabel.Text = "No Properties";
            // 
            // RailTab
            // 
            this.RailTab.Controls.Add(this.RailFormationListView);
            this.RailTab.Controls.Add(this.RailBooleanPanel);
            this.RailTab.Controls.Add(this.RailTypePanel);
            this.RailTab.Location = new System.Drawing.Point(4, 22);
            this.RailTab.Name = "RailTab";
            this.RailTab.Padding = new System.Windows.Forms.Padding(3);
            this.RailTab.Size = new System.Drawing.Size(776, 446);
            this.RailTab.TabIndex = 1;
            this.RailTab.Text = "Rails";
            this.RailTab.UseVisualStyleBackColor = true;
            // 
            // RailFormationListView
            // 
            this.RailFormationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.RailNameColumnHeader,
            this.RailDescriptionColumnHeader});
            this.RailFormationListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RailFormationListView.FullRowSelect = true;
            this.RailFormationListView.GridLines = true;
            this.RailFormationListView.HideSelection = false;
            this.RailFormationListView.Location = new System.Drawing.Point(3, 54);
            this.RailFormationListView.MultiSelect = false;
            this.RailFormationListView.Name = "RailFormationListView";
            this.RailFormationListView.Size = new System.Drawing.Size(770, 389);
            this.RailFormationListView.TabIndex = 5;
            this.RailFormationListView.UseCompatibleStateImageBehavior = false;
            this.RailFormationListView.View = System.Windows.Forms.View.Details;
            // 
            // RailNameColumnHeader
            // 
            this.RailNameColumnHeader.Text = "Rail Name";
            this.RailNameColumnHeader.Width = 114;
            // 
            // RailDescriptionColumnHeader
            // 
            this.RailDescriptionColumnHeader.Text = "Description";
            this.RailDescriptionColumnHeader.Width = 517;
            // 
            // RailBooleanPanel
            // 
            this.RailBooleanPanel.Controls.Add(this.ReverseRailCheckBox);
            this.RailBooleanPanel.Controls.Add(this.CloseRailCheckBox);
            this.RailBooleanPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.RailBooleanPanel.Location = new System.Drawing.Point(3, 26);
            this.RailBooleanPanel.Name = "RailBooleanPanel";
            this.RailBooleanPanel.Size = new System.Drawing.Size(770, 28);
            this.RailBooleanPanel.TabIndex = 6;
            // 
            // ReverseRailCheckBox
            // 
            this.ReverseRailCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ReverseRailCheckBox.AutoSize = true;
            this.ReverseRailCheckBox.Location = new System.Drawing.Point(360, 3);
            this.ReverseRailCheckBox.Name = "ReverseRailCheckBox";
            this.ReverseRailCheckBox.Size = new System.Drawing.Size(72, 17);
            this.ReverseRailCheckBox.TabIndex = 5;
            this.ReverseRailCheckBox.Text = "Reverse?";
            this.ReverseRailCheckBox.UseVisualStyleBackColor = true;
            // 
            // CloseRailCheckBox
            // 
            this.CloseRailCheckBox.AutoSize = true;
            this.CloseRailCheckBox.Location = new System.Drawing.Point(0, 3);
            this.CloseRailCheckBox.Name = "CloseRailCheckBox";
            this.CloseRailCheckBox.Size = new System.Drawing.Size(101, 17);
            this.CloseRailCheckBox.TabIndex = 4;
            this.CloseRailCheckBox.Text = "Close the Path?";
            this.CloseRailCheckBox.UseVisualStyleBackColor = true;
            // 
            // RailTypePanel
            // 
            this.RailTypePanel.Controls.Add(this.RailTypeComboBox);
            this.RailTypePanel.Controls.Add(this.RailTypeLabel);
            this.RailTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.RailTypePanel.Location = new System.Drawing.Point(3, 3);
            this.RailTypePanel.Name = "RailTypePanel";
            this.RailTypePanel.Size = new System.Drawing.Size(770, 23);
            this.RailTypePanel.TabIndex = 7;
            // 
            // RailTypeComboBox
            // 
            this.RailTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RailTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RailTypeComboBox.FormattingEnabled = true;
            this.RailTypeComboBox.Items.AddRange(new object[] {
            "Rail",
            "RailWithMoveParameter",
            "RailWithEffect"});
            this.RailTypeComboBox.Location = new System.Drawing.Point(55, 0);
            this.RailTypeComboBox.Name = "RailTypeComboBox";
            this.RailTypeComboBox.Size = new System.Drawing.Size(715, 21);
            this.RailTypeComboBox.TabIndex = 0;
            // 
            // RailTypeLabel
            // 
            this.RailTypeLabel.AutoSize = true;
            this.RailTypeLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.RailTypeLabel.Location = new System.Drawing.Point(0, 0);
            this.RailTypeLabel.Name = "RailTypeLabel";
            this.RailTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.RailTypeLabel.TabIndex = 1;
            this.RailTypeLabel.Text = "Rail Type:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ToQuickFavoritesButton);
            this.panel1.Controls.Add(this.SelectObjectButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 472);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 39);
            this.panel1.TabIndex = 6;
            // 
            // ToQuickFavoritesButton
            // 
            this.ToQuickFavoritesButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ToQuickFavoritesButton.Location = new System.Drawing.Point(643, 6);
            this.ToQuickFavoritesButton.Name = "ToQuickFavoritesButton";
            this.ToQuickFavoritesButton.Size = new System.Drawing.Size(129, 30);
            this.ToQuickFavoritesButton.TabIndex = 3;
            this.ToQuickFavoritesButton.Text = "To QuickFavorites";
            this.ToQuickFavoritesButton.UseVisualStyleBackColor = true;
            this.ToQuickFavoritesButton.Click += new System.EventHandler(this.ToQuickFavoritesButton_Click);
            // 
            // DeselectModelButton
            // 
            this.DeselectModelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DeselectModelButton.Location = new System.Drawing.Point(236, -1);
            this.DeselectModelButton.Name = "DeselectModelButton";
            this.DeselectModelButton.Size = new System.Drawing.Size(18, 17);
            this.DeselectModelButton.TabIndex = 7;
            this.DeselectModelButton.Text = "X";
            this.DeselectModelButton.UseVisualStyleBackColor = true;
            this.DeselectModelButton.Click += new System.EventHandler(this.DeselectModelButton_Click);
            // 
            // AddObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 511);
            this.Controls.Add(this.ObjectTypeTabControl);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 550);
            this.Name = "AddObjectForm";
            this.ShowInTaskbar = false;
            this.Text = "Spotlight - Add Object";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddObjectForm_FormClosing);
            this.Load += new System.EventHandler(this.AddObjectForm_Load);
            this.ObjectTypeTabControl.ResumeLayout(false);
            this.ObjectFromDBTab.ResumeLayout(false);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.AddSettingsPanel.ResumeLayout(false);
            this.AddSettingsSplitContainer.Panel1.ResumeLayout(false);
            this.AddSettingsSplitContainer.Panel1.PerformLayout();
            this.AddSettingsSplitContainer.Panel2.ResumeLayout(false);
            this.AddSettingsSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AddSettingsSplitContainer)).EndInit();
            this.AddSettingsSplitContainer.ResumeLayout(false);
            this.RailTab.ResumeLayout(false);
            this.RailBooleanPanel.ResumeLayout(false);
            this.RailBooleanPanel.PerformLayout();
            this.RailTypePanel.ResumeLayout(false);
            this.RailTypePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView DBEntryListView;
        private System.Windows.Forms.ColumnHeader ClassNameColumnHeader;
        private System.Windows.Forms.TextBox ObjectDescriptionTextBox;
        private System.Windows.Forms.Button SelectObjectButton;
        private System.Windows.Forms.TabControl ObjectTypeTabControl;
        private System.Windows.Forms.TabPage ObjectFromDBTab;
        private System.Windows.Forms.TabPage RailTab;
        private System.Windows.Forms.Label RailTypeLabel;
        private System.Windows.Forms.ComboBox RailTypeComboBox;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.ColumnHeader ObjectCountColumnHeader;
        private System.Windows.Forms.Panel SearchPanel;
        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.ColumnHeader ModelCountColumnHeader;
        private System.Windows.Forms.Label ClassNameLabel;
        private System.Windows.Forms.ListView PropertyNotesListView;
        private System.Windows.Forms.ColumnHeader NameColumnHeader;
        private System.Windows.Forms.ColumnHeader TypeColumnHeader;
        private System.Windows.Forms.ColumnHeader DescriptionColumnHeader;
        private System.Windows.Forms.TextBox PropertyHintTextBox;
        private System.Windows.Forms.Label PropertyLabel;
        private System.Windows.Forms.Panel AddSettingsPanel;
        private System.Windows.Forms.SplitContainer AddSettingsSplitContainer;
        private System.Windows.Forms.ListView SelectObjectListView;
        private System.Windows.Forms.Label SelectObjectLabel;
        private System.Windows.Forms.ListView SelectModelListView;
        private System.Windows.Forms.Label SelectModelLabel;
        private System.Windows.Forms.ColumnHeader ObjectNameColumnHeader;
        private System.Windows.Forms.ColumnHeader ModelNameColumnHeader;
        private System.Windows.Forms.CheckBox CloseRailCheckBox;
        private System.Windows.Forms.ListView RailFormationListView;
        private System.Windows.Forms.Panel RailBooleanPanel;
        private System.Windows.Forms.CheckBox ReverseRailCheckBox;
        private System.Windows.Forms.Panel RailTypePanel;
        private System.Windows.Forms.ColumnHeader RailNameColumnHeader;
        private System.Windows.Forms.ColumnHeader RailDescriptionColumnHeader;
        private System.Windows.Forms.ColumnHeader EnglishNameColumnHeader;
        private System.Windows.Forms.TextBox EnglishNameTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ToQuickFavoritesButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button DeselectModelButton;
    }
}
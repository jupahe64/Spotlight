namespace Spotlight.GUI
{
    partial class LevelEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditorForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.LayersTabPage = new System.Windows.Forms.TabPage();
            this.LayerListControl = new Spotlight.GUI.LayerListControl();
            this.LayerListView = new System.Windows.Forms.ListView();
            this.ZonesTabPage = new System.Windows.Forms.TabPage();
            this.ZoneListBox = new System.Windows.Forms.ListBox();
            this.EditIndividualButton = new System.Windows.Forms.Button();
            this.ObjectsTabPage = new System.Windows.Forms.TabPage();
            this.MainSceneListView = new Spotlight.GUI.SceneListView3dWorld();
            this.ObjectUIControl = new GL_EditorFramework.ObjectUIControl();
            this.CurrentObjectLabel = new System.Windows.Forms.Label();
            this.ZoneDocumentTabControl = new GL_EditorFramework.DocumentTabControl();
            this.QuickFavoriteControl = new Spotlight.GUI.QuickFavoriteControl();
            this.LevelGLControlModern = new GL_EditorFramework.GL_Core.GL_ControlModern();
            this.SpotlightMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenExToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddZoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DuplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveSelectionToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToLinkedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToAppropriateListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToSpecificListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateViewGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LevelParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrowSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectAllLinkedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpotlightWikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpotlightStatusStrip = new System.Windows.Forms.StatusStrip();
            this.SpotlightToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.CancelAddObjectButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.LayersTabPage.SuspendLayout();
            this.ZonesTabPage.SuspendLayout();
            this.ObjectsTabPage.SuspendLayout();
            this.ZoneDocumentTabControl.SuspendLayout();
            this.SpotlightMenuStrip.SuspendLayout();
            this.SpotlightStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ZoneDocumentTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(1045, 639);
            this.splitContainer1.SplitterDistance = 346;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.MainTabControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ObjectUIControl);
            this.splitContainer2.Panel2.Controls.Add(this.CurrentObjectLabel);
            this.splitContainer2.Panel2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Panel2.Click += new System.EventHandler(this.SplitContainer2_Panel2_Click);
            this.splitContainer2.Size = new System.Drawing.Size(346, 639);
            this.splitContainer2.SplitterDistance = 300;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 1;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.LayersTabPage);
            this.MainTabControl.Controls.Add(this.ZonesTabPage);
            this.MainTabControl.Controls.Add(this.ObjectsTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(346, 300);
            this.MainTabControl.TabIndex = 1;
            // 
            // LayersTabPage
            // 
            this.LayersTabPage.Controls.Add(this.LayerListControl);
            this.LayersTabPage.Controls.Add(this.LayerListView);
            this.LayersTabPage.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LayersTabPage.Location = new System.Drawing.Point(4, 26);
            this.LayersTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.LayersTabPage.Name = "LayersTabPage";
            this.LayersTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.LayersTabPage.Size = new System.Drawing.Size(338, 270);
            this.LayersTabPage.TabIndex = 1;
            this.LayersTabPage.Text = "Layers";
            this.LayersTabPage.UseVisualStyleBackColor = true;
            // 
            // LayerListControl
            // 
            this.LayerListControl.BackColor = System.Drawing.SystemColors.Control;
            this.LayerListControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LayerListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerListControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LayerListControl.Location = new System.Drawing.Point(4, 4);
            this.LayerListControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LayerListControl.Name = "LayerListControl";
            this.LayerListControl.Size = new System.Drawing.Size(330, 262);
            this.LayerListControl.TabIndex = 2;
            // 
            // LayerListView
            // 
            this.LayerListView.CheckBoxes = true;
            this.LayerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerListView.Enabled = false;
            this.LayerListView.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LayerListView.HideSelection = false;
            this.LayerListView.Location = new System.Drawing.Point(4, 4);
            this.LayerListView.Margin = new System.Windows.Forms.Padding(4);
            this.LayerListView.Name = "LayerListView";
            this.LayerListView.Size = new System.Drawing.Size(330, 262);
            this.LayerListView.TabIndex = 1;
            this.LayerListView.UseCompatibleStateImageBehavior = false;
            this.LayerListView.View = System.Windows.Forms.View.List;
            // 
            // ZonesTabPage
            // 
            this.ZonesTabPage.Controls.Add(this.ZoneListBox);
            this.ZonesTabPage.Controls.Add(this.EditIndividualButton);
            this.ZonesTabPage.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZonesTabPage.Location = new System.Drawing.Point(4, 26);
            this.ZonesTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ZonesTabPage.Name = "ZonesTabPage";
            this.ZonesTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ZonesTabPage.Size = new System.Drawing.Size(338, 270);
            this.ZonesTabPage.TabIndex = 0;
            this.ZonesTabPage.Text = "Zones";
            this.ZonesTabPage.UseVisualStyleBackColor = true;
            // 
            // ZoneListBox
            // 
            this.ZoneListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneListBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoneListBox.FormattingEnabled = true;
            this.ZoneListBox.ItemHeight = 17;
            this.ZoneListBox.Location = new System.Drawing.Point(4, 4);
            this.ZoneListBox.Margin = new System.Windows.Forms.Padding(4);
            this.ZoneListBox.Name = "ZoneListBox";
            this.ZoneListBox.Size = new System.Drawing.Size(330, 234);
            this.ZoneListBox.TabIndex = 3;
            this.ZoneListBox.SelectedIndexChanged += new System.EventHandler(this.ZoneListBox_SelectedIndexChanged);
            // 
            // EditIndividualButton
            // 
            this.EditIndividualButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.EditIndividualButton.Enabled = false;
            this.EditIndividualButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditIndividualButton.Location = new System.Drawing.Point(4, 238);
            this.EditIndividualButton.Margin = new System.Windows.Forms.Padding(4);
            this.EditIndividualButton.Name = "EditIndividualButton";
            this.EditIndividualButton.Size = new System.Drawing.Size(330, 28);
            this.EditIndividualButton.TabIndex = 1;
            this.EditIndividualButton.Text = "Edit Individual";
            this.EditIndividualButton.UseVisualStyleBackColor = true;
            this.EditIndividualButton.Click += new System.EventHandler(this.EditIndividualButton_Click);
            // 
            // ObjectsTabPage
            // 
            this.ObjectsTabPage.Controls.Add(this.MainSceneListView);
            this.ObjectsTabPage.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ObjectsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ObjectsTabPage.Name = "ObjectsTabPage";
            this.ObjectsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ObjectsTabPage.Size = new System.Drawing.Size(338, 270);
            this.ObjectsTabPage.TabIndex = 1;
            this.ObjectsTabPage.Text = "Objects";
            this.ObjectsTabPage.UseVisualStyleBackColor = true;
            // 
            // MainSceneListView
            // 
            this.MainSceneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSceneListView.Enabled = false;
            this.MainSceneListView.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainSceneListView.Location = new System.Drawing.Point(4, 4);
            this.MainSceneListView.Margin = new System.Windows.Forms.Padding(4);
            this.MainSceneListView.Name = "MainSceneListView";
            this.MainSceneListView.Scene = null;
            this.MainSceneListView.Size = new System.Drawing.Size(330, 262);
            this.MainSceneListView.TabIndex = 1;
            this.MainSceneListView.ItemClicked += new GL_EditorFramework.ItemClickedEventHandler(this.MainSceneListView_ItemClicked);
            // 
            // ObjectUIControl
            // 
            this.ObjectUIControl.BackColor = System.Drawing.SystemColors.Control;
            this.ObjectUIControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectUIControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectUIControl.Location = new System.Drawing.Point(4, 21);
            this.ObjectUIControl.Margin = new System.Windows.Forms.Padding(5);
            this.ObjectUIControl.MinimumSize = new System.Drawing.Size(267, 246);
            this.ObjectUIControl.Name = "ObjectUIControl";
            this.ObjectUIControl.Size = new System.Drawing.Size(338, 309);
            this.ObjectUIControl.TabIndex = 1;
            // 
            // CurrentObjectLabel
            // 
            this.CurrentObjectLabel.AutoSize = true;
            this.CurrentObjectLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentObjectLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentObjectLabel.Location = new System.Drawing.Point(4, 4);
            this.CurrentObjectLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CurrentObjectLabel.Name = "CurrentObjectLabel";
            this.CurrentObjectLabel.Size = new System.Drawing.Size(107, 17);
            this.CurrentObjectLabel.TabIndex = 0;
            this.CurrentObjectLabel.Text = "Nothing selected";
            // 
            // ZoneDocumentTabControl
            // 
            this.ZoneDocumentTabControl.Controls.Add(this.QuickFavoriteControl);
            this.ZoneDocumentTabControl.Controls.Add(this.LevelGLControlModern);
            this.ZoneDocumentTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneDocumentTabControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoneDocumentTabControl.Location = new System.Drawing.Point(0, 0);
            this.ZoneDocumentTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.ZoneDocumentTabControl.Name = "ZoneDocumentTabControl";
            this.ZoneDocumentTabControl.Padding = new System.Windows.Forms.Padding(2, 23, 2, 2);
            this.ZoneDocumentTabControl.Size = new System.Drawing.Size(694, 639);
            this.ZoneDocumentTabControl.TabIndex = 1;
            this.ZoneDocumentTabControl.SelectedTabChanged += new System.EventHandler(this.ZoneDocumentTabControl_SelectedTabChanged);
            this.ZoneDocumentTabControl.TabClosing += new GL_EditorFramework.DocumentTabClosingEventHandler(this.ZoneDocumentTabControl_TabClosing);
            // 
            // QuickFavoriteControl
            // 
            this.QuickFavoriteControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.QuickFavoriteControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QuickFavoriteControl.Location = new System.Drawing.Point(2, 556);
            this.QuickFavoriteControl.Margin = new System.Windows.Forms.Padding(4);
            this.QuickFavoriteControl.Name = "QuickFavoriteControl";
            this.QuickFavoriteControl.Size = new System.Drawing.Size(690, 81);
            this.QuickFavoriteControl.TabIndex = 1;
            this.QuickFavoriteControl.SelectedFavoriteChanged += new System.EventHandler(this.QuickFavoriteControl_SelectedFavoriteChanged);
            // 
            // LevelGLControlModern
            // 
            this.LevelGLControlModern.AllowDrop = true;
            this.LevelGLControlModern.BackColor = System.Drawing.Color.Black;
            this.LevelGLControlModern.CameraDistance = 10F;
            this.LevelGLControlModern.CameraTarget = ((OpenTK.Vector3)(resources.GetObject("LevelGLControlModern.CameraTarget")));
            this.LevelGLControlModern.CamRotX = 0F;
            this.LevelGLControlModern.CamRotY = 0F;
            this.LevelGLControlModern.CurrentShader = null;
            this.LevelGLControlModern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LevelGLControlModern.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LevelGLControlModern.Fov = 0.7853982F;
            this.LevelGLControlModern.Location = new System.Drawing.Point(2, 23);
            this.LevelGLControlModern.Margin = new System.Windows.Forms.Padding(4, 49, 4, 4);
            this.LevelGLControlModern.Name = "LevelGLControlModern";
            this.LevelGLControlModern.NormPickingDepth = 0F;
            this.LevelGLControlModern.ShowOrientationCube = true;
            this.LevelGLControlModern.Size = new System.Drawing.Size(690, 614);
            this.LevelGLControlModern.Stereoscopy = GL_EditorFramework.GL_Core.GL_ControlBase.StereoscopyType.DISABLED;
            this.LevelGLControlModern.TabIndex = 0;
            this.LevelGLControlModern.VSync = false;
            this.LevelGLControlModern.ZFar = 32000F;
            this.LevelGLControlModern.ZNear = 1.5F;
            this.LevelGLControlModern.Load += new System.EventHandler(this.LevelGLControlModern_Load);
            this.LevelGLControlModern.DragDrop += new System.Windows.Forms.DragEventHandler(this.LevelGLControlModern_DragDrop);
            this.LevelGLControlModern.DragEnter += new System.Windows.Forms.DragEventHandler(this.LevelGLControlModern_DragEnter);
            // 
            // SpotlightMenuStrip
            // 
            this.SpotlightMenuStrip.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpotlightMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.SpotlightMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.SelectionToolStripMenuItem,
            this.ModeToolStripMenuItem,
            this.AboutToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.SpotlightMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.SpotlightMenuStrip.Name = "SpotlightMenuStrip";
            this.SpotlightMenuStrip.Size = new System.Drawing.Size(1045, 28);
            this.SpotlightMenuStrip.TabIndex = 3;
            this.SpotlightMenuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.OpenExToolStripMenuItem,
            this.SaveToolStripMenuItem,
            this.SaveAsToolStripMenuItem,
            this.OptionsToolStripMenuItem,
            this.restartToolStripMenuItem});
            this.FileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(41, 24);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.ToolTipText = "Open a Level file";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // OpenExToolStripMenuItem
            // 
            this.OpenExToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenExToolStripMenuItem.Name = "OpenExToolStripMenuItem";
            this.OpenExToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.OpenExToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.OpenExToolStripMenuItem.Text = "Open With Selector";
            this.OpenExToolStripMenuItem.Click += new System.EventHandler(this.OpenExToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Enabled = false;
            this.SaveAsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.SaveAsToolStripMenuItem.Text = "Save As";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.OptionsToolStripMenuItem.Text = "Options";
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.RestartToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UndoToolStripMenuItem,
            this.RedoToolStripMenuItem,
            this.AddObjectToolStripMenuItem,
            this.AddZoneToolStripMenuItem,
            this.CopyToolStripMenuItem,
            this.PasteToolStripMenuItem,
            this.DuplicateToolStripMenuItem,
            this.DeleteToolStripMenuItem,
            this.MoveSelectionToToolStripMenuItem,
            this.CreateViewGroupToolStripMenuItem,
            this.LevelParametersToolStripMenuItem,
            this.ChangeLayerToolStripMenuItem});
            this.EditToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.EditToolStripMenuItem.Text = "Edit";
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Enabled = false;
            this.UndoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.UndoToolStripMenuItem.Text = "Undo";
            this.UndoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Enabled = false;
            this.RedoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.RedoToolStripMenuItem.Text = "Redo";
            this.RedoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            // 
            // AddObjectToolStripMenuItem
            // 
            this.AddObjectToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddObjectToolStripMenuItem.Name = "AddObjectToolStripMenuItem";
            this.AddObjectToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.AddObjectToolStripMenuItem.Text = "Add Object";
            this.AddObjectToolStripMenuItem.Click += new System.EventHandler(this.AddObjectToolStripMenuItem_Click);
            // 
            // AddZoneToolStripMenuItem
            // 
            this.AddZoneToolStripMenuItem.Enabled = false;
            this.AddZoneToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddZoneToolStripMenuItem.Name = "AddZoneToolStripMenuItem";
            this.AddZoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.AddZoneToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.AddZoneToolStripMenuItem.Text = "Add Zone";
            this.AddZoneToolStripMenuItem.Click += new System.EventHandler(this.AddZoneToolStripMenuItem_Click);
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Enabled = false;
            this.CopyToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.CopyToolStripMenuItem.Text = "Copy";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // PasteToolStripMenuItem
            // 
            this.PasteToolStripMenuItem.Enabled = false;
            this.PasteToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem";
            this.PasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.PasteToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.PasteToolStripMenuItem.Text = "Paste";
            this.PasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // DuplicateToolStripMenuItem
            // 
            this.DuplicateToolStripMenuItem.Enabled = false;
            this.DuplicateToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DuplicateToolStripMenuItem.Name = "DuplicateToolStripMenuItem";
            this.DuplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DuplicateToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.DuplicateToolStripMenuItem.Text = "Duplicate";
            this.DuplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItem_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Enabled = false;
            this.DeleteToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.DeleteToolStripMenuItem.Text = "Delete";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // MoveSelectionToToolStripMenuItem
            // 
            this.MoveSelectionToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MoveToLinkedToolStripMenuItem,
            this.MoveToAppropriateListsToolStripMenuItem,
            this.MoveToSpecificListToolStripMenuItem});
            this.MoveSelectionToToolStripMenuItem.Enabled = false;
            this.MoveSelectionToToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveSelectionToToolStripMenuItem.Name = "MoveSelectionToToolStripMenuItem";
            this.MoveSelectionToToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.MoveSelectionToToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.MoveSelectionToToolStripMenuItem.Text = "Move Selection To";
            // 
            // MoveToLinkedToolStripMenuItem
            // 
            this.MoveToLinkedToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveToLinkedToolStripMenuItem.Name = "MoveToLinkedToolStripMenuItem";
            this.MoveToLinkedToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.MoveToLinkedToolStripMenuItem.Text = "Linked Objects";
            this.MoveToLinkedToolStripMenuItem.Click += new System.EventHandler(this.MoveToLinkedToolStripMenuItem_Click);
            // 
            // MoveToAppropriateListsToolStripMenuItem
            // 
            this.MoveToAppropriateListsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveToAppropriateListsToolStripMenuItem.Name = "MoveToAppropriateListsToolStripMenuItem";
            this.MoveToAppropriateListsToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.MoveToAppropriateListsToolStripMenuItem.Text = "Appropriate Lists";
            this.MoveToAppropriateListsToolStripMenuItem.Click += new System.EventHandler(this.MoveToAppropriateListsToolStripMenuItem_Click);
            // 
            // MoveToSpecificListToolStripMenuItem
            // 
            this.MoveToSpecificListToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveToSpecificListToolStripMenuItem.Name = "MoveToSpecificListToolStripMenuItem";
            this.MoveToSpecificListToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.MoveToSpecificListToolStripMenuItem.Text = "Specific List";
            // 
            // CreateViewGroupToolStripMenuItem
            // 
            this.CreateViewGroupToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateViewGroupToolStripMenuItem.Name = "CreateViewGroupToolStripMenuItem";
            this.CreateViewGroupToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.CreateViewGroupToolStripMenuItem.Text = "Create View Group";
            this.CreateViewGroupToolStripMenuItem.Click += new System.EventHandler(this.CreateViewGroupToolStripMenuItem_Click);
            // 
            // LevelParametersToolStripMenuItem
            // 
            this.LevelParametersToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LevelParametersToolStripMenuItem.Name = "LevelParametersToolStripMenuItem";
            this.LevelParametersToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.LevelParametersToolStripMenuItem.Text = "Level Parameters";
            this.LevelParametersToolStripMenuItem.Click += new System.EventHandler(this.LevelParametersToolStripMenuItem_Click);
            // 
            // ChangeLayerToolStripMenuItem
            // 
            this.ChangeLayerToolStripMenuItem.Name = "ChangeLayerToolStripMenuItem";
            this.ChangeLayerToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.ChangeLayerToolStripMenuItem.Text = "Change Layer";
            // 
            // SelectionToolStripMenuItem
            // 
            this.SelectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectAllToolStripMenuItem,
            this.DeselectAllToolStripMenuItem,
            this.GrowSelectionToolStripMenuItem,
            this.SelectAllLinkedToolStripMenuItem,
            this.invertSelectionToolStripMenuItem});
            this.SelectionToolStripMenuItem.Enabled = false;
            this.SelectionToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectionToolStripMenuItem.Name = "SelectionToolStripMenuItem";
            this.SelectionToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.SelectionToolStripMenuItem.Text = "Selection";
            // 
            // SelectAllToolStripMenuItem
            // 
            this.SelectAllToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            this.SelectAllToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.SelectAllToolStripMenuItem.Text = "Select All";
            this.SelectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // DeselectAllToolStripMenuItem
            // 
            this.DeselectAllToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeselectAllToolStripMenuItem.Name = "DeselectAllToolStripMenuItem";
            this.DeselectAllToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.DeselectAllToolStripMenuItem.Text = "Deselect All";
            this.DeselectAllToolStripMenuItem.Click += new System.EventHandler(this.DeselectAllToolStripMenuItem_Click);
            // 
            // GrowSelectionToolStripMenuItem
            // 
            this.GrowSelectionToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrowSelectionToolStripMenuItem.Name = "GrowSelectionToolStripMenuItem";
            this.GrowSelectionToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.GrowSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.GrowSelectionToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.GrowSelectionToolStripMenuItem.Text = "Select All Linked";
            this.GrowSelectionToolStripMenuItem.Click += new System.EventHandler(this.GrowSelectionToolStripMenuItem_Click);
            // 
            // SelectAllLinkedToolStripMenuItem
            // 
            this.SelectAllLinkedToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectAllLinkedToolStripMenuItem.Name = "SelectAllLinkedToolStripMenuItem";
            this.SelectAllLinkedToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.SelectAllLinkedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.SelectAllLinkedToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.SelectAllLinkedToolStripMenuItem.Text = "Select Selection Cluster";
            this.SelectAllLinkedToolStripMenuItem.Click += new System.EventHandler(this.SelectAllLinkedToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.invertSelectionToolStripMenuItem.Text = "Invert Selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.InvertSelectionToolStripMenuItem_Click);
            // 
            // ModeToolStripMenuItem
            // 
            this.ModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditObjectsToolStripMenuItem,
            this.EditLinksToolStripMenuItem});
            this.ModeToolStripMenuItem.Enabled = false;
            this.ModeToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModeToolStripMenuItem.Name = "ModeToolStripMenuItem";
            this.ModeToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.ModeToolStripMenuItem.Text = "Mode";
            // 
            // EditObjectsToolStripMenuItem
            // 
            this.EditObjectsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditObjectsToolStripMenuItem.Name = "EditObjectsToolStripMenuItem";
            this.EditObjectsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.O)));
            this.EditObjectsToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.EditObjectsToolStripMenuItem.Text = "Edit Objects";
            this.EditObjectsToolStripMenuItem.Click += new System.EventHandler(this.EditObjectsToolStripMenuItem_Click);
            // 
            // EditLinksToolStripMenuItem
            // 
            this.EditLinksToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditLinksToolStripMenuItem.Name = "EditLinksToolStripMenuItem";
            this.EditLinksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.L)));
            this.EditLinksToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.EditLinksToolStripMenuItem.Text = "Edit Links";
            this.EditLinksToolStripMenuItem.Click += new System.EventHandler(this.EditLinksToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpotlightWikiToolStripMenuItem,
            this.CheckForUpdatesToolStripMenuItem});
            this.AboutToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.AboutToolStripMenuItem.Text = "About";
            // 
            // SpotlightWikiToolStripMenuItem
            // 
            this.SpotlightWikiToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpotlightWikiToolStripMenuItem.Name = "SpotlightWikiToolStripMenuItem";
            this.SpotlightWikiToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.SpotlightWikiToolStripMenuItem.Text = "Spotlight Wiki";
            this.SpotlightWikiToolStripMenuItem.Click += new System.EventHandler(this.SpotlightWikiToolStripMenuItem_Click);
            // 
            // CheckForUpdatesToolStripMenuItem
            // 
            this.CheckForUpdatesToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckForUpdatesToolStripMenuItem.Name = "CheckForUpdatesToolStripMenuItem";
            this.CheckForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.CheckForUpdatesToolStripMenuItem.Text = "Check for Updates";
            this.CheckForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compareToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            this.compareToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.compareToolStripMenuItem.Text = "Compare";
            this.compareToolStripMenuItem.Click += new System.EventHandler(this.CompareToolStripMenuItem_Click);
            // 
            // SpotlightStatusStrip
            // 
            this.SpotlightStatusStrip.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpotlightStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.SpotlightStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpotlightToolStripStatusLabel});
            this.SpotlightStatusStrip.Location = new System.Drawing.Point(0, 667);
            this.SpotlightStatusStrip.Name = "SpotlightStatusStrip";
            this.SpotlightStatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.SpotlightStatusStrip.Size = new System.Drawing.Size(1045, 23);
            this.SpotlightStatusStrip.TabIndex = 4;
            // 
            // SpotlightToolStripStatusLabel
            // 
            this.SpotlightToolStripStatusLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpotlightToolStripStatusLabel.Name = "SpotlightToolStripStatusLabel";
            this.SpotlightToolStripStatusLabel.Size = new System.Drawing.Size(101, 17);
            this.SpotlightToolStripStatusLabel.Text = "Spotlight 0.8.0.0";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            // 
            // CancelAddObjectButton
            // 
            this.CancelAddObjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelAddObjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CancelAddObjectButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelAddObjectButton.Location = new System.Drawing.Point(955, 666);
            this.CancelAddObjectButton.Margin = new System.Windows.Forms.Padding(4);
            this.CancelAddObjectButton.Name = "CancelAddObjectButton";
            this.CancelAddObjectButton.Size = new System.Drawing.Size(88, 23);
            this.CancelAddObjectButton.TabIndex = 5;
            this.CancelAddObjectButton.Text = "Cancel";
            this.CancelAddObjectButton.UseVisualStyleBackColor = true;
            this.CancelAddObjectButton.Visible = false;
            this.CancelAddObjectButton.Click += new System.EventHandler(this.CancelAddObjectButton_Click);
            // 
            // LevelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 690);
            this.Controls.Add(this.CancelAddObjectButton);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.SpotlightMenuStrip);
            this.Controls.Add(this.SpotlightStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.SpotlightMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1061, 728);
            this.Name = "LevelEditorForm";
            this.Text = "SpotLight";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelEditorForm_FormClosing);
            this.Shown += new System.EventHandler(this.LevelEditorForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.LayersTabPage.ResumeLayout(false);
            this.ZonesTabPage.ResumeLayout(false);
            this.ObjectsTabPage.ResumeLayout(false);
            this.ZoneDocumentTabControl.ResumeLayout(false);
            this.SpotlightMenuStrip.ResumeLayout(false);
            this.SpotlightMenuStrip.PerformLayout();
            this.SpotlightStatusStrip.ResumeLayout(false);
            this.SpotlightStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label CurrentObjectLabel;
        private System.Windows.Forms.MenuStrip SpotlightMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.StatusStrip SpotlightStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel SpotlightToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UndoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RedoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LevelParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DuplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenExToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem ModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddObjectToolStripMenuItem;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage ZonesTabPage;
        private System.Windows.Forms.TabPage ObjectsTabPage;
        private System.Windows.Forms.TabPage LayersTabPage;
        private System.Windows.Forms.Button EditIndividualButton;
        public GL_EditorFramework.GL_Core.GL_ControlModern LevelGLControlModern;
        public GL_EditorFramework.ObjectUIControl ObjectUIControl;
        public SceneListView3dWorld MainSceneListView;
        private GL_EditorFramework.DocumentTabControl ZoneDocumentTabControl;
        private System.Windows.Forms.ListBox ZoneListBox;
        private System.Windows.Forms.ToolStripMenuItem AddZoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveSelectionToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveToLinkedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveToAppropriateListsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SpotlightWikiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CheckForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GrowSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectAllLinkedToolStripMenuItem;
        private QuickFavoriteControl QuickFavoriteControl;
        private System.Windows.Forms.ToolStripMenuItem SelectionToolStripMenuItem;
        private System.Windows.Forms.Button CancelAddObjectButton;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveToSpecificListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateViewGroupToolStripMenuItem;
        private System.Windows.Forms.ListView LayerListView;
        private LayerListControl LayerListControl;
        private System.Windows.Forms.ToolStripMenuItem ChangeLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
    }
}


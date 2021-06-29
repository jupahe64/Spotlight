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
            this.ZonesTabPage = new System.Windows.Forms.TabPage();
            this.ZoneListBox = new System.Windows.Forms.ListBox();
            this.EditIndividualButton = new System.Windows.Forms.Button();
            this.ObjectsTabPage = new System.Windows.Forms.TabPage();
            this.LayersTabPage = new System.Windows.Forms.TabPage();
            this.MainSceneListView = new SceneListView3dWorld();
            this.CurrentObjectLabel = new System.Windows.Forms.Label();
            this.ObjectUIControl = new GL_EditorFramework.ObjectUIControl();
            this.ZoneDocumentTabControl = new GL_EditorFramework.DocumentTabControl();
            this.QuickFavoriteControl = new QuickFavoriteControl();
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
            this.SpotlightStatusStrip = new System.Windows.Forms.StatusStrip();
            this.SpotlightToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.CancelAddObjectButton = new System.Windows.Forms.Button();
            this.LayerListView = new System.Windows.Forms.ListView();
            this.DrawLayerComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.MainTabControl.SuspendLayout();
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ZoneDocumentTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(784, 515);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.MainTabControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.CurrentObjectLabel);
            this.splitContainer2.Panel2.Controls.Add(this.ObjectUIControl);
            this.splitContainer2.Panel2.Click += new System.EventHandler(this.SplitContainer2_Panel2_Click);
            this.splitContainer2.Size = new System.Drawing.Size(260, 515);
            this.splitContainer2.SplitterDistance = 245;
            this.splitContainer2.TabIndex = 1;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.LayersTabPage);
            this.MainTabControl.Controls.Add(this.ZonesTabPage);
            this.MainTabControl.Controls.Add(this.ObjectsTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(260, 245);
            this.MainTabControl.TabIndex = 1;
            // 
            // ZonesTabPage
            // 
            this.ZonesTabPage.Controls.Add(this.ZoneListBox);
            this.ZonesTabPage.Controls.Add(this.EditIndividualButton);
            this.ZonesTabPage.Location = new System.Drawing.Point(4, 22);
            this.ZonesTabPage.Name = "ZonesTabPage";
            this.ZonesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ZonesTabPage.Size = new System.Drawing.Size(252, 219);
            this.ZonesTabPage.TabIndex = 0;
            this.ZonesTabPage.Text = "Zones";
            this.ZonesTabPage.UseVisualStyleBackColor = true;
            // 
            // ZoneListBox
            // 
            this.ZoneListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneListBox.FormattingEnabled = true;
            this.ZoneListBox.Location = new System.Drawing.Point(3, 3);
            this.ZoneListBox.Name = "ZoneListBox";
            this.ZoneListBox.Size = new System.Drawing.Size(246, 190);
            this.ZoneListBox.TabIndex = 3;
            this.ZoneListBox.SelectedIndexChanged += new System.EventHandler(this.ZoneListBox_SelectedIndexChanged);
            // 
            // EditIndividualButton
            // 
            this.EditIndividualButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.EditIndividualButton.Enabled = false;
            this.EditIndividualButton.Location = new System.Drawing.Point(3, 193);
            this.EditIndividualButton.Name = "EditIndividualButton";
            this.EditIndividualButton.Size = new System.Drawing.Size(246, 23);
            this.EditIndividualButton.TabIndex = 1;
            this.EditIndividualButton.Text = "Edit Individual";
            this.EditIndividualButton.UseVisualStyleBackColor = true;
            this.EditIndividualButton.Click += new System.EventHandler(this.EditIndividualButton_Click);
            // 
            // ObjectsTabPage
            // 
            this.ObjectsTabPage.Controls.Add(this.MainSceneListView);
            this.ObjectsTabPage.Location = new System.Drawing.Point(4, 22);
            this.ObjectsTabPage.Name = "ObjectsTabPage";
            this.ObjectsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectsTabPage.Size = new System.Drawing.Size(252, 219);
            this.ObjectsTabPage.TabIndex = 1;
            this.ObjectsTabPage.Text = "Objects";
            this.ObjectsTabPage.UseVisualStyleBackColor = true;
            // 
            // LayersTabPage
            // 
            this.LayersTabPage.Controls.AddRange(new System.Windows.Forms.Control[]{
                this.LayerListView,
                this.DrawLayerComboBox
            });
            this.LayersTabPage.Location = new System.Drawing.Point(4, 22);
            this.LayersTabPage.Name = "LayersTabPage";
            this.LayersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.LayersTabPage.Size = new System.Drawing.Size(252, 219);
            this.LayersTabPage.TabIndex = 1;
            this.LayersTabPage.Text = "Layers";
            this.LayersTabPage.UseVisualStyleBackColor = true;
            // 
            // MainSceneListView
            // 
            this.MainSceneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSceneListView.Enabled = false;
            this.MainSceneListView.Location = new System.Drawing.Point(3, 3);
            this.MainSceneListView.Name = "MainSceneListView";
            this.MainSceneListView.Scene = null;
            this.MainSceneListView.Size = new System.Drawing.Size(246, 213);
            this.MainSceneListView.TabIndex = 1;
            this.MainSceneListView.ItemClicked += new GL_EditorFramework.ItemClickedEventHandler(this.MainSceneListView_ItemClicked);
            // 
            // LayerListView
            // 
            this.LayerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerListView.Enabled = false;
            this.LayerListView.Location = new System.Drawing.Point(3, 3);
            this.LayerListView.Name = "LayerListView";
            this.LayerListView.Size = new System.Drawing.Size(246, 213);
            this.LayerListView.TabIndex = 1;
            this.LayerListView.CheckBoxes = true;
            this.LayerListView.View = System.Windows.Forms.View.List;
            this.LayerListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LayerList_ItemCheck);
            this.LayerListView.SelectedIndexChanged += new System.EventHandler(LayerListView_SelectedIndexChanged);
            // 
            // DrawLayerComboBox
            // 
            this.DrawLayerComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DrawLayerComboBox.Enabled = false;
            this.DrawLayerComboBox.Location = new System.Drawing.Point(3, 3);
            this.DrawLayerComboBox.Name = "DrawLayerComboBox";
            this.DrawLayerComboBox.Size = new System.Drawing.Size(246, 213);
            this.DrawLayerComboBox.TabIndex = 1;
            this.DrawLayerComboBox.TextChanged += new System.EventHandler(DrawLayerComboBox_TextChanged);
            // 
            // CurrentObjectLabel
            // 
            this.CurrentObjectLabel.AutoSize = true;
            this.CurrentObjectLabel.Location = new System.Drawing.Point(12, 0);
            this.CurrentObjectLabel.Name = "CurrentObjectLabel";
            this.CurrentObjectLabel.Size = new System.Drawing.Size(87, 13);
            this.CurrentObjectLabel.TabIndex = 0;
            this.CurrentObjectLabel.Text = "Nothing selected";
            // 
            // ObjectUIControl
            // 
            this.ObjectUIControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectUIControl.BackColor = System.Drawing.SystemColors.Control;
            this.ObjectUIControl.Location = new System.Drawing.Point(3, 16);
            this.ObjectUIControl.MinimumSize = new System.Drawing.Size(200, 200);
            this.ObjectUIControl.Name = "ObjectUIControl";
            this.ObjectUIControl.Size = new System.Drawing.Size(254, 245);
            this.ObjectUIControl.TabIndex = 1;
            // 
            // ZoneDocumentTabControl
            // 
            this.ZoneDocumentTabControl.Controls.Add(this.QuickFavoriteControl);
            this.ZoneDocumentTabControl.Controls.Add(this.LevelGLControlModern);
            this.ZoneDocumentTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneDocumentTabControl.Location = new System.Drawing.Point(0, 0);
            this.ZoneDocumentTabControl.Name = "ZoneDocumentTabControl";
            this.ZoneDocumentTabControl.Size = new System.Drawing.Size(520, 515);
            this.ZoneDocumentTabControl.TabIndex = 1;
            this.ZoneDocumentTabControl.SelectedTabChanged += new System.EventHandler(this.ZoneDocumentTabControl_SelectedTabChanged);
            this.ZoneDocumentTabControl.TabClosing += new GL_EditorFramework.DocumentTabClosingEventHandler(this.ZoneDocumentTabControl_TabClosing);
            // 
            // QuickFavoriteControl
            // 
            this.QuickFavoriteControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QuickFavoriteControl.Location = new System.Drawing.Point(3, 449);
            this.QuickFavoriteControl.Name = "QuickFavoriteControl";
            this.QuickFavoriteControl.Size = new System.Drawing.Size(515, 66);
            this.QuickFavoriteControl.TabIndex = 1;
            this.QuickFavoriteControl.SelectedFavoriteChanged += new System.EventHandler(this.QuickFavoriteControl_SelectedFavoriteChanged);
            // 
            // LevelGLControlModern
            // 
            this.LevelGLControlModern.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LevelGLControlModern.BackColor = System.Drawing.Color.Black;
            this.LevelGLControlModern.CameraDistance = 10F;
            this.LevelGLControlModern.CameraTarget = ((OpenTK.Vector3)(resources.GetObject("LevelGLControlModern.CameraTarget")));
            this.LevelGLControlModern.CamRotX = 0F;
            this.LevelGLControlModern.CamRotY = 0F;
            this.LevelGLControlModern.CurrentShader = null;
            this.LevelGLControlModern.Fov = 0.7853982F;
            this.LevelGLControlModern.Location = new System.Drawing.Point(3, 32);
            this.LevelGLControlModern.Margin = new System.Windows.Forms.Padding(3, 32, 3, 3);
            this.LevelGLControlModern.Name = "LevelGLControlModern";
            this.LevelGLControlModern.NormPickingDepth = 0F;
            this.LevelGLControlModern.ShowOrientationCube = true;
            this.LevelGLControlModern.Size = new System.Drawing.Size(515, 419);
            this.LevelGLControlModern.Stereoscopy = GL_EditorFramework.GL_Core.GL_ControlBase.StereoscopyType.DISABLED;
            this.LevelGLControlModern.TabIndex = 0;
            this.LevelGLControlModern.VSync = false;
            this.LevelGLControlModern.ZFar = 32000F;
            this.LevelGLControlModern.ZNear = 1.5F;
            this.LevelGLControlModern.Load += new System.EventHandler(this.LevelGLControlModern_Load);
            // 
            // SpotlightMenuStrip
            // 
            this.SpotlightMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.SelectionToolStripMenuItem,
            this.ModeToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.SpotlightMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.SpotlightMenuStrip.Name = "SpotlightMenuStrip";
            this.SpotlightMenuStrip.Size = new System.Drawing.Size(784, 24);
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
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.ToolTipText = "Open a Level file";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // OpenExToolStripMenuItem
            // 
            this.OpenExToolStripMenuItem.Name = "OpenExToolStripMenuItem";
            this.OpenExToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.OpenExToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.OpenExToolStripMenuItem.Text = "Open With Selector";
            this.OpenExToolStripMenuItem.Click += new System.EventHandler(this.OpenExToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Enabled = false;
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.S)));
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.SaveAsToolStripMenuItem.Text = "Save As";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.O)));
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.OptionsToolStripMenuItem.Text = "Options";
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
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
            this.LevelParametersToolStripMenuItem});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.EditToolStripMenuItem.Text = "Edit";
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Enabled = false;
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.UndoToolStripMenuItem.Text = "Undo";
            this.UndoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Enabled = false;
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.RedoToolStripMenuItem.Text = "Redo";
            this.RedoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            // 
            // AddObjectToolStripMenuItem
            // 
            this.AddObjectToolStripMenuItem.Name = "AddObjectToolStripMenuItem";
            this.AddObjectToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.AddObjectToolStripMenuItem.Text = "Add Object";
            this.AddObjectToolStripMenuItem.Click += new System.EventHandler(this.AddObjectToolStripMenuItem_Click);
            // 
            // AddZoneToolStripMenuItem
            // 
            this.AddZoneToolStripMenuItem.Enabled = false;
            this.AddZoneToolStripMenuItem.Name = "AddZoneToolStripMenuItem";
            this.AddZoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.AddZoneToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.AddZoneToolStripMenuItem.Text = "Add Zone";
            this.AddZoneToolStripMenuItem.Click += new System.EventHandler(this.AddZoneToolStripMenuItem_Click);
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Enabled = false;
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.CopyToolStripMenuItem.Text = "Copy";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // PasteToolStripMenuItem
            // 
            this.PasteToolStripMenuItem.Enabled = false;
            this.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem";
            this.PasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.PasteToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.PasteToolStripMenuItem.Text = "Paste";
            this.PasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // DuplicateToolStripMenuItem
            // 
            this.DuplicateToolStripMenuItem.Enabled = false;
            this.DuplicateToolStripMenuItem.Name = "DuplicateToolStripMenuItem";
            this.DuplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DuplicateToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.DuplicateToolStripMenuItem.Text = "Duplicate";
            this.DuplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItem_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Enabled = false;
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
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
            this.MoveSelectionToToolStripMenuItem.Name = "MoveSelectionToToolStripMenuItem";
            this.MoveSelectionToToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.MoveSelectionToToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.MoveSelectionToToolStripMenuItem.Text = "Move Selection To";
            // 
            // MoveToLinkedToolStripMenuItem
            // 
            this.MoveToLinkedToolStripMenuItem.Name = "MoveToLinkedToolStripMenuItem";
            this.MoveToLinkedToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.MoveToLinkedToolStripMenuItem.Text = "Linked Objects";
            this.MoveToLinkedToolStripMenuItem.Click += new System.EventHandler(this.MoveToLinkedToolStripMenuItem_Click);
            // 
            // MoveToAppropriateListsToolStripMenuItem
            // 
            this.MoveToAppropriateListsToolStripMenuItem.Name = "MoveToAppropriateListsToolStripMenuItem";
            this.MoveToAppropriateListsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.MoveToAppropriateListsToolStripMenuItem.Text = "Appropriate Lists";
            this.MoveToAppropriateListsToolStripMenuItem.Click += new System.EventHandler(this.MoveToAppropriateListsToolStripMenuItem_Click);
            // 
            // MoveToSpecificListToolStripMenuItem
            // 
            this.MoveToSpecificListToolStripMenuItem.Name = "MoveToSpecificListToolStripMenuItem";
            this.MoveToSpecificListToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.MoveToSpecificListToolStripMenuItem.Text = "Specific List";
            // 
            // CreateViewGroupToolStripMenuItem
            // 
            this.CreateViewGroupToolStripMenuItem.Name = "LevelParametersToolStripMenuItem";
            this.CreateViewGroupToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.CreateViewGroupToolStripMenuItem.Text = "Create View Group";
            this.CreateViewGroupToolStripMenuItem.Click += new System.EventHandler(this.CreateViewGroupToolStripMenuItem_Click);
            // 
            // LevelParametersToolStripMenuItem
            // 
            this.LevelParametersToolStripMenuItem.Name = "LevelParametersToolStripMenuItem";
            this.LevelParametersToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.LevelParametersToolStripMenuItem.Text = "Level Parameters";
            this.LevelParametersToolStripMenuItem.Click += new System.EventHandler(this.LevelParametersToolStripMenuItem_Click);
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
            this.SelectionToolStripMenuItem.Name = "SelectionToolStripMenuItem";
            this.SelectionToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.SelectionToolStripMenuItem.Text = "Selection";
            // 
            // SelectAllToolStripMenuItem
            // 
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            this.SelectAllToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.SelectAllToolStripMenuItem.Text = "Select All";
            this.SelectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // DeselectAllToolStripMenuItem
            // 
            this.DeselectAllToolStripMenuItem.Name = "DeselectAllToolStripMenuItem";
            this.DeselectAllToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.DeselectAllToolStripMenuItem.Text = "Deselect All";
            this.DeselectAllToolStripMenuItem.Click += new System.EventHandler(this.DeselectAllToolStripMenuItem_Click);
            // 
            // GrowSelectionToolStripMenuItem
            // 
            this.GrowSelectionToolStripMenuItem.Name = "GrowSelectionToolStripMenuItem";
            this.GrowSelectionToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.GrowSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.GrowSelectionToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.GrowSelectionToolStripMenuItem.Text = "Select All Linked";
            this.GrowSelectionToolStripMenuItem.Click += new System.EventHandler(this.GrowSelectionToolStripMenuItem_Click);
            // 
            // SelectAllLinkedToolStripMenuItem
            // 
            this.SelectAllLinkedToolStripMenuItem.Name = "SelectAllLinkedToolStripMenuItem";
            this.SelectAllLinkedToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.SelectAllLinkedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.SelectAllLinkedToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.SelectAllLinkedToolStripMenuItem.Text = "Select Selection Cluster";
            this.SelectAllLinkedToolStripMenuItem.Click += new System.EventHandler(this.SelectAllLinkedToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert Selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.InvertSelectionToolStripMenuItem_Click);
            // 
            // ModeToolStripMenuItem
            // 
            this.ModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditObjectsToolStripMenuItem,
            this.EditLinksToolStripMenuItem});
            this.ModeToolStripMenuItem.Enabled = false;
            this.ModeToolStripMenuItem.Name = "ModeToolStripMenuItem";
            this.ModeToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.ModeToolStripMenuItem.Text = "Mode";
            // 
            // EditObjectsToolStripMenuItem
            // 
            this.EditObjectsToolStripMenuItem.Name = "EditObjectsToolStripMenuItem";
            this.EditObjectsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
            | System.Windows.Forms.Keys.O)));
            this.EditObjectsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.EditObjectsToolStripMenuItem.Text = "Edit Objects";
            this.EditObjectsToolStripMenuItem.Click += new System.EventHandler(this.EditObjectsToolStripMenuItem_Click);
            // 
            // EditLinksToolStripMenuItem
            // 
            this.EditLinksToolStripMenuItem.Name = "EditLinksToolStripMenuItem";
            this.EditLinksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
            | System.Windows.Forms.Keys.L)));
            this.EditLinksToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.EditLinksToolStripMenuItem.Text = "Edit Links";
            this.EditLinksToolStripMenuItem.Click += new System.EventHandler(this.EditLinksToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpotlightWikiToolStripMenuItem,
            this.CheckForUpdatesToolStripMenuItem});
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.AboutToolStripMenuItem.Text = "About";
            // 
            // SpotlightWikiToolStripMenuItem
            // 
            this.SpotlightWikiToolStripMenuItem.Name = "SpotlightWikiToolStripMenuItem";
            this.SpotlightWikiToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.SpotlightWikiToolStripMenuItem.Text = "Spotlight Wiki";
            this.SpotlightWikiToolStripMenuItem.Click += new System.EventHandler(this.SpotlightWikiToolStripMenuItem_Click);
            // 
            // CheckForUpdatesToolStripMenuItem
            // 
            this.CheckForUpdatesToolStripMenuItem.Name = "CheckForUpdatesToolStripMenuItem";
            this.CheckForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.CheckForUpdatesToolStripMenuItem.Text = "Check for Updates";
            this.CheckForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
            // 
            // SpotlightStatusStrip
            // 
            this.SpotlightStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpotlightToolStripStatusLabel});
            this.SpotlightStatusStrip.Location = new System.Drawing.Point(0, 539);
            this.SpotlightStatusStrip.Name = "SpotlightStatusStrip";
            this.SpotlightStatusStrip.Size = new System.Drawing.Size(784, 22);
            this.SpotlightStatusStrip.TabIndex = 4;
            // 
            // SpotlightToolStripStatusLabel
            // 
            this.SpotlightToolStripStatusLabel.Name = "SpotlightToolStripStatusLabel";
            this.SpotlightToolStripStatusLabel.Size = new System.Drawing.Size(91, 17);
            this.SpotlightToolStripStatusLabel.Text = "Spotlight 0.8.0.0";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // CancelAddObjectButton
            // 
            this.CancelAddObjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelAddObjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CancelAddObjectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.CancelAddObjectButton.Location = new System.Drawing.Point(716, 541);
            this.CancelAddObjectButton.Name = "CancelAddObjectButton";
            this.CancelAddObjectButton.Size = new System.Drawing.Size(66, 19);
            this.CancelAddObjectButton.TabIndex = 5;
            this.CancelAddObjectButton.Text = "Cancel";
            this.CancelAddObjectButton.UseVisualStyleBackColor = true;
            this.CancelAddObjectButton.Visible = false;
            this.CancelAddObjectButton.Click += new System.EventHandler(this.CancelAddObjectButton_Click);
            // 
            // LevelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.CancelAddObjectButton);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.SpotlightMenuStrip);
            this.Controls.Add(this.SpotlightStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.SpotlightMenuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
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
        public System.Windows.Forms.ComboBox DrawLayerComboBox;
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
    }
}


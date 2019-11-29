namespace SpotLight
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageZones = new System.Windows.Forms.TabPage();
            this.btnEditIndividual = new System.Windows.Forms.Button();
            this.LevelZoneTreeView = new System.Windows.Forms.TreeView();
            this.tabPageObjects = new System.Windows.Forms.TabPage();
            this.MainSceneListView = new GL_EditorFramework.SceneListView();
            this.lblCurrentObject = new System.Windows.Forms.Label();
            this.ObjectUIControl = new GL_EditorFramework.ObjectUIControl();
            this.closableTabControl1 = new GL_EditorFramework.ClosableTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LevelGLControlModern = new GL_EditorFramework.GL_Core.GL_ControlModern();
            this.SpotlightMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenExToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DuplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LevelParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpotlightStatusStrip = new System.Windows.Forms.StatusStrip();
            this.SpotlightToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.SpotlightToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageZones.SuspendLayout();
            this.tabPageObjects.SuspendLayout();
            this.closableTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.closableTabControl1);
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
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lblCurrentObject);
            this.splitContainer2.Panel2.Controls.Add(this.ObjectUIControl);
            this.splitContainer2.Panel2.Click += new System.EventHandler(this.SplitContainer2_Panel2_Click);
            this.splitContainer2.Size = new System.Drawing.Size(260, 515);
            this.splitContainer2.SplitterDistance = 245;
            this.splitContainer2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageZones);
            this.tabControl1.Controls.Add(this.tabPageObjects);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(260, 245);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageZones
            // 
            this.tabPageZones.Controls.Add(this.btnEditIndividual);
            this.tabPageZones.Controls.Add(this.LevelZoneTreeView);
            this.tabPageZones.Location = new System.Drawing.Point(4, 22);
            this.tabPageZones.Name = "tabPageZones";
            this.tabPageZones.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageZones.Size = new System.Drawing.Size(252, 219);
            this.tabPageZones.TabIndex = 0;
            this.tabPageZones.Text = "Zones";
            this.tabPageZones.UseVisualStyleBackColor = true;
            // 
            // btnEditIndividual
            // 
            this.btnEditIndividual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditIndividual.Location = new System.Drawing.Point(3, 190);
            this.btnEditIndividual.Name = "btnEditIndividual";
            this.btnEditIndividual.Size = new System.Drawing.Size(246, 23);
            this.btnEditIndividual.TabIndex = 1;
            this.btnEditIndividual.Text = "Edit Individual";
            this.btnEditIndividual.UseVisualStyleBackColor = true;
            this.btnEditIndividual.Click += new System.EventHandler(this.BtnEditIndividual_Click);
            // 
            // LevelZoneTreeView
            // 
            this.LevelZoneTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LevelZoneTreeView.Location = new System.Drawing.Point(3, 3);
            this.LevelZoneTreeView.Name = "LevelZoneTreeView";
            this.LevelZoneTreeView.Size = new System.Drawing.Size(246, 184);
            this.LevelZoneTreeView.TabIndex = 0;
            this.LevelZoneTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LevelZoneTreeView_AfterSelect);
            // 
            // tabPageObjects
            // 
            this.tabPageObjects.Controls.Add(this.MainSceneListView);
            this.tabPageObjects.Location = new System.Drawing.Point(4, 22);
            this.tabPageObjects.Name = "tabPageObjects";
            this.tabPageObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageObjects.Size = new System.Drawing.Size(252, 219);
            this.tabPageObjects.TabIndex = 1;
            this.tabPageObjects.Text = "Objects";
            this.tabPageObjects.UseVisualStyleBackColor = true;
            // 
            // MainSceneListView
            // 
            this.MainSceneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSceneListView.Enabled = false;
            this.MainSceneListView.Location = new System.Drawing.Point(3, 3);
            this.MainSceneListView.Name = "MainSceneListView";
            this.MainSceneListView.RootLists = ((System.Collections.Generic.Dictionary<string, System.Collections.IList>)(resources.GetObject("MainSceneListView.RootLists")));
            this.MainSceneListView.Size = new System.Drawing.Size(246, 213);
            this.MainSceneListView.TabIndex = 1;
            this.MainSceneListView.ItemDoubleClicked += new GL_EditorFramework.ItemDoubleClickedEventHandler(this.MainSceneListView_ItemDoubleClicked);
            // 
            // lblCurrentObject
            // 
            this.lblCurrentObject.AutoSize = true;
            this.lblCurrentObject.Location = new System.Drawing.Point(12, 0);
            this.lblCurrentObject.Name = "lblCurrentObject";
            this.lblCurrentObject.Size = new System.Drawing.Size(87, 13);
            this.lblCurrentObject.TabIndex = 0;
            this.lblCurrentObject.Text = "Nothing selected";
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
            // closableTabControl1
            // 
            this.closableTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.closableTabControl1.Controls.Add(this.tabPage1);
            this.closableTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.closableTabControl1.Location = new System.Drawing.Point(3, 3);
            this.closableTabControl1.Name = "closableTabControl1";
            this.closableTabControl1.Padding = new System.Drawing.Point(16, 4);
            this.closableTabControl1.SelectedIndex = 0;
            this.closableTabControl1.Size = new System.Drawing.Size(517, 512);
            this.closableTabControl1.TabIndex = 1;
            this.closableTabControl1.SelectedIndexChanged += new System.EventHandler(this.ClosableTabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.LevelGLControlModern);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(509, 484);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LevelGLControlModern
            // 
            this.LevelGLControlModern.BackColor = System.Drawing.Color.Black;
            this.LevelGLControlModern.CamRotX = 0F;
            this.LevelGLControlModern.CamRotY = 0F;
            this.LevelGLControlModern.CurrentShader = null;
            this.LevelGLControlModern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LevelGLControlModern.Fov = 0.7853982F;
            this.LevelGLControlModern.Location = new System.Drawing.Point(0, 0);
            this.LevelGLControlModern.Name = "LevelGLControlModern";
            this.LevelGLControlModern.NormPickingDepth = 0F;
            this.LevelGLControlModern.ShowOrientationCube = true;
            this.LevelGLControlModern.Size = new System.Drawing.Size(509, 484);
            this.LevelGLControlModern.Stereoscopy = false;
            this.LevelGLControlModern.TabIndex = 0;
            this.LevelGLControlModern.VSync = false;
            this.LevelGLControlModern.ZFar = 32000F;
            this.LevelGLControlModern.ZNear = 1.5F;
            // 
            // SpotlightMenuStrip
            // 
            this.SpotlightMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.modeToolStripMenuItem});
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
            this.OptionsToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.ToolTipText = "Open a Level file";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // OpenExToolStripMenuItem
            // 
            this.OpenExToolStripMenuItem.Name = "OpenExToolStripMenuItem";
            this.OpenExToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.OpenExToolStripMenuItem.Text = "Open With Selector";
            this.OpenExToolStripMenuItem.Click += new System.EventHandler(this.OpenExToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SaveAsToolStripMenuItem.Text = "Save As";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.OptionsToolStripMenuItem.Text = "Options";
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UndoToolStripMenuItem,
            this.RedoToolStripMenuItem,
            this.AddObjectToolStripMenuItem,
            this.DuplicateToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.LevelParametersToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.UndoToolStripMenuItem.Text = "Undo              Ctrl+Z";
            this.UndoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.RedoToolStripMenuItem.Text = "Redo               Ctrl+Y";
            this.RedoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            // 
            // AddObjectToolStripMenuItem
            // 
            this.AddObjectToolStripMenuItem.Name = "AddObjectToolStripMenuItem";
            this.AddObjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.AddObjectToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.AddObjectToolStripMenuItem.Text = "Add Object";
            this.AddObjectToolStripMenuItem.Click += new System.EventHandler(this.AddObjectToolStripMenuItem_Click);
            // 
            // DuplicateToolStripMenuItem
            // 
            this.DuplicateToolStripMenuItem.Name = "DuplicateToolStripMenuItem";
            this.DuplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DuplicateToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.DuplicateToolStripMenuItem.Text = "Duplicate";
            this.DuplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.DeselectAllToolStripMenuItem_Click);
            // 
            // LevelParametersToolStripMenuItem
            // 
            this.LevelParametersToolStripMenuItem.Name = "LevelParametersToolStripMenuItem";
            this.LevelParametersToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.LevelParametersToolStripMenuItem.Text = "Level Parameters";
            this.LevelParametersToolStripMenuItem.Click += new System.EventHandler(this.LevelParametersToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editObjectsToolStripMenuItem,
            this.editLinksToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // editObjectsToolStripMenuItem
            // 
            this.editObjectsToolStripMenuItem.Name = "editObjectsToolStripMenuItem";
            this.editObjectsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.O)));
            this.editObjectsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.editObjectsToolStripMenuItem.Text = "Edit Objects";
            this.editObjectsToolStripMenuItem.Click += new System.EventHandler(this.EditObjectsToolStripMenuItem_Click);
            // 
            // editLinksToolStripMenuItem
            // 
            this.editLinksToolStripMenuItem.Name = "editLinksToolStripMenuItem";
            this.editLinksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.L)));
            this.editLinksToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.editLinksToolStripMenuItem.Text = "Edit Links";
            this.editLinksToolStripMenuItem.Click += new System.EventHandler(this.EditLinksToolStripMenuItem_Click);
            // 
            // SpotlightStatusStrip
            // 
            this.SpotlightStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpotlightToolStripProgressBar,
            this.SpotlightToolStripStatusLabel});
            this.SpotlightStatusStrip.Location = new System.Drawing.Point(0, 539);
            this.SpotlightStatusStrip.Name = "SpotlightStatusStrip";
            this.SpotlightStatusStrip.Size = new System.Drawing.Size(784, 22);
            this.SpotlightStatusStrip.TabIndex = 4;
            // 
            // SpotlightToolStripProgressBar
            // 
            this.SpotlightToolStripProgressBar.Name = "SpotlightToolStripProgressBar";
            this.SpotlightToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // SpotlightToolStripStatusLabel
            // 
            this.SpotlightToolStripStatusLabel.Name = "SpotlightToolStripStatusLabel";
            this.SpotlightToolStripStatusLabel.Size = new System.Drawing.Size(91, 17);
            this.SpotlightToolStripStatusLabel.Text = "Spotlight 0.1.5.0";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // LevelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.SpotlightMenuStrip);
            this.Controls.Add(this.SpotlightStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.SpotlightMenuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "LevelEditorForm";
            this.Text = "SpotLight";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageZones.ResumeLayout(false);
            this.tabPageObjects.ResumeLayout(false);
            this.closableTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblCurrentObject;
        private System.Windows.Forms.MenuStrip SpotlightMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.StatusStrip SpotlightStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel SpotlightToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar SpotlightToolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UndoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RedoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LevelParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DuplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenExToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddObjectToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageZones;
        private System.Windows.Forms.TabPage tabPageObjects;
        private System.Windows.Forms.Button btnEditIndividual;
        public GL_EditorFramework.GL_Core.GL_ControlModern LevelGLControlModern;
        public GL_EditorFramework.ObjectUIControl ObjectUIControl;
        public System.Windows.Forms.TreeView LevelZoneTreeView;
        public GL_EditorFramework.SceneListView MainSceneListView;
        private GL_EditorFramework.ClosableTabControl closableTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
    }
}


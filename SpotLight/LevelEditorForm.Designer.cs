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
            this.lblCurrentObject = new System.Windows.Forms.Label();
            this.SpotlightMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LevelParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpotlightStatusStrip = new System.Windows.Forms.StatusStrip();
            this.SpotlightToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.SpotlightToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainSceneListView = new GL_EditorFramework.SceneListView();
            this.ObjectUIControl = new GL_EditorFramework.ObjectUIControl();
            this.LevelGLControlModern = new GL_EditorFramework.GL_Core.GL_ControlModern();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.LevelGLControlModern);
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
            this.splitContainer2.Panel1.Controls.Add(this.MainSceneListView);
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
            // lblCurrentObject
            // 
            this.lblCurrentObject.AutoSize = true;
            this.lblCurrentObject.Location = new System.Drawing.Point(12, 0);
            this.lblCurrentObject.Name = "lblCurrentObject";
            this.lblCurrentObject.Size = new System.Drawing.Size(87, 13);
            this.lblCurrentObject.TabIndex = 0;
            this.lblCurrentObject.Text = "Nothing selected";
            // 
            // SpotlightMenuStrip
            // 
            this.SpotlightMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.editToolStripMenuItem});
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
            this.LevelParametersToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.UndoToolStripMenuItem.Text = "Undo              Ctrl+Z";
            this.UndoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RedoToolStripMenuItem.Text = "Redo               Ctrl+Y";
            this.RedoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            // 
            // LevelParametersToolStripMenuItem
            // 
            this.LevelParametersToolStripMenuItem.Name = "LevelParametersToolStripMenuItem";
            this.LevelParametersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.LevelParametersToolStripMenuItem.Text = "Level Parameters";
            this.LevelParametersToolStripMenuItem.Click += new System.EventHandler(this.LevelParametersToolStripMenuItem_Click);
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
            this.SpotlightToolStripStatusLabel.Text = "Spotlight 0.0.1.0";
            // 
            // MainSceneListView
            // 
            this.MainSceneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSceneListView.Enabled = false;
            this.MainSceneListView.Location = new System.Drawing.Point(0, 0);
            this.MainSceneListView.Name = "MainSceneListView";
            this.MainSceneListView.RootLists = ((System.Collections.Generic.Dictionary<string, System.Collections.IList>)(resources.GetObject("MainSceneListView.RootLists")));
            this.MainSceneListView.Size = new System.Drawing.Size(260, 245);
            this.MainSceneListView.TabIndex = 0;
            // 
            // ObjectUIControl
            // 
            this.ObjectUIControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectUIControl.CurrentObjectUIProvider = null;
            this.ObjectUIControl.Location = new System.Drawing.Point(3, 16);
            this.ObjectUIControl.MinimumSize = new System.Drawing.Size(200, 200);
            this.ObjectUIControl.Name = "ObjectUIControl";
            this.ObjectUIControl.Size = new System.Drawing.Size(254, 247);
            this.ObjectUIControl.TabIndex = 1;
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
            this.LevelGLControlModern.Size = new System.Drawing.Size(520, 515);
            this.LevelGLControlModern.Stereoscopy = false;
            this.LevelGLControlModern.TabIndex = 0;
            this.LevelGLControlModern.VSync = false;
            this.LevelGLControlModern.ZFar = 32000F;
            this.LevelGLControlModern.ZNear = 1.5F;
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
            this.SpotlightMenuStrip.ResumeLayout(false);
            this.SpotlightMenuStrip.PerformLayout();
            this.SpotlightStatusStrip.ResumeLayout(false);
            this.SpotlightStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GL_EditorFramework.GL_Core.GL_ControlModern LevelGLControlModern;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private GL_EditorFramework.SceneListView MainSceneListView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label lblCurrentObject;
        private GL_EditorFramework.ObjectUIControl ObjectUIControl;
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
    }
}


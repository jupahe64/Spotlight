namespace SpotLight.GUI
{
    partial class SceneListView3dWorld
    {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ItemsListView = new GL_EditorFramework.FastListView();
            this.TreeView = new SceneTreeView3dWorld();
            this.CurrentZoneLabel = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CurrentZoneLabel
            // 
            this.CurrentZoneLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentZoneLabel.Location = new System.Drawing.Point(0, 2);
            this.CurrentZoneLabel.Name = "RootListComboBox";
            this.CurrentZoneLabel.Size = new System.Drawing.Size(300, 29);
            //this.CurrentZoneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //this.CurrentZoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CurrentZoneLabel.TabIndex = 3;
            // 
            // ItemsListView
            // 
            this.ItemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsListView.AutoScroll = true;
            this.ItemsListView.BackColor = System.Drawing.SystemColors.Window;
            this.ItemsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ItemsListView.Location = new System.Drawing.Point(1, 29);
            this.ItemsListView.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.ItemsListView.Name = "ItemsListView";
            this.ItemsListView.Size = new System.Drawing.Size(298, 270);
            this.ItemsListView.TabIndex = 0;
            // 
            // TreeView
            // 
            this.TreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeView.AutoScroll = true;
            this.TreeView.BackColor = System.Drawing.SystemColors.Window;
            this.TreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TreeView.Location = new System.Drawing.Point(1, 29);
            this.TreeView.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.TreeView.Name = "ItemsListView";
            this.TreeView.Size = new System.Drawing.Size(298, 270);
            this.TreeView.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Image = global::GL_EditorFramework.Properties.Resources.BackButtonIcon;
            this.btnBack.Location = new System.Drawing.Point(0, 2);
            this.btnBack.Margin = new System.Windows.Forms.Padding(1);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(29, 21);
            this.btnBack.TabIndex = 2;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // SceneListView
            // 
            this.Controls.Add(this.CurrentZoneLabel);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.TreeView);
            this.Controls.Add(this.ItemsListView);
            this.Name = "SceneListView";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private GL_EditorFramework.FastListView ItemsListView;
        private SceneTreeView3dWorld TreeView;
        private System.Windows.Forms.Label CurrentZoneLabel;
        private System.Windows.Forms.Button btnBack;
    }
}

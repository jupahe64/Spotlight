namespace Spotlight.GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneListView3dWorld));
            this.ItemsListView = new GL_EditorFramework.FastListView();
            this.BackButton = new System.Windows.Forms.Button();
            this.TreeView = new Spotlight.GUI.SceneTreeView3dWorld();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            // BackButton
            // 
            this.BackButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackButton.Image = ((System.Drawing.Image)(resources.GetObject("BackButton.Image")));
            this.BackButton.Location = new System.Drawing.Point(0, 2);
            this.BackButton.Margin = new System.Windows.Forms.Padding(1);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(29, 21);
            this.BackButton.TabIndex = 2;
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Visible = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // TreeView
            // 
            this.TreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeView.AutoScroll = true;
            this.TreeView.BackColor = System.Drawing.SystemColors.Window;
            this.TreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TreeView.FilterString = "";
            this.TreeView.Location = new System.Drawing.Point(0, 29);
            this.TreeView.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.TreeView.Name = "TreeView";
            this.TreeView.Size = new System.Drawing.Size(300, 271);
            this.TreeView.TabIndex = 0;
            // 
            // FilterTextBox
            // 
            this.FilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTextBox.Location = new System.Drawing.Point(0, 3);
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(300, 20);
            this.FilterTextBox.TabIndex = 3;
            this.FilterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
            // 
            // SceneListView3dWorld
            // 
            this.Controls.Add(this.FilterTextBox);
            this.Controls.Add(this.TreeView);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.ItemsListView);
            this.Name = "SceneListView3dWorld";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GL_EditorFramework.FastListView ItemsListView;
        private SceneTreeView3dWorld TreeView;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.TextBox FilterTextBox;
    }
}

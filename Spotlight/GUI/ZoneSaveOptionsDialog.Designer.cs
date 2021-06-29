
namespace Spotlight.GUI
{
    partial class ZoneSaveOptionsDialog
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
            this.ZonesDataGridView = new System.Windows.Forms.DataGridView();
            this.OriginalNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShouldSaveColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StageNameTextBox = new System.Windows.Forms.TextBox();
            this.PresetComboBox = new System.Windows.Forms.ComboBox();
            this.ZonesLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.GamePresetLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CancleButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ZonesDataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ZonesDataGridView
            // 
            this.ZonesDataGridView.AllowUserToAddRows = false;
            this.ZonesDataGridView.AllowUserToDeleteRows = false;
            this.ZonesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ZonesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ZonesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OriginalNameColumn,
            this.NewNameColumn,
            this.ShouldSaveColumn});
            this.ZonesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZonesDataGridView.Location = new System.Drawing.Point(0, 107);
            this.ZonesDataGridView.Name = "ZonesDataGridView";
            this.ZonesDataGridView.Size = new System.Drawing.Size(342, 292);
            this.ZonesDataGridView.TabIndex = 0;
            // 
            // OriginalNameColumn
            // 
            this.OriginalNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.OriginalNameColumn.HeaderText = "Original";
            this.OriginalNameColumn.Name = "OriginalNameColumn";
            this.OriginalNameColumn.ReadOnly = true;
            this.OriginalNameColumn.Width = 67;
            // 
            // NewNameColumn
            // 
            this.NewNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NewNameColumn.HeaderText = "New";
            this.NewNameColumn.Name = "NewNameColumn";
            // 
            // ShouldSaveColumn
            // 
            this.ShouldSaveColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ShouldSaveColumn.HeaderText = "Save";
            this.ShouldSaveColumn.Name = "ShouldSaveColumn";
            this.ShouldSaveColumn.Width = 38;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.StageNameTextBox);
            this.panel1.Controls.Add(this.PresetComboBox);
            this.panel1.Controls.Add(this.ZonesLabel);
            this.panel1.Controls.Add(this.NameLabel);
            this.panel1.Controls.Add(this.GamePresetLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(342, 107);
            this.panel1.TabIndex = 1;
            // 
            // StageNameTextBox
            // 
            this.StageNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StageNameTextBox.Location = new System.Drawing.Point(86, 6);
            this.StageNameTextBox.Name = "StageNameTextBox";
            this.StageNameTextBox.Size = new System.Drawing.Size(244, 20);
            this.StageNameTextBox.TabIndex = 4;
            // 
            // PresetComboBox
            // 
            this.PresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PresetComboBox.FormattingEnabled = true;
            this.PresetComboBox.Location = new System.Drawing.Point(86, 32);
            this.PresetComboBox.Name = "PresetComboBox";
            this.PresetComboBox.Size = new System.Drawing.Size(244, 21);
            this.PresetComboBox.TabIndex = 3;
            // 
            // ZonesLabel
            // 
            this.ZonesLabel.AutoSize = true;
            this.ZonesLabel.Location = new System.Drawing.Point(3, 91);
            this.ZonesLabel.Name = "ZonesLabel";
            this.ZonesLabel.Size = new System.Drawing.Size(40, 13);
            this.ZonesLabel.TabIndex = 2;
            this.ZonesLabel.Text = "Zones:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(12, 9);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(35, 13);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Name";
            // 
            // GamePresetLabel
            // 
            this.GamePresetLabel.AutoSize = true;
            this.GamePresetLabel.Location = new System.Drawing.Point(12, 35);
            this.GamePresetLabel.Name = "GamePresetLabel";
            this.GamePresetLabel.Size = new System.Drawing.Size(68, 13);
            this.GamePresetLabel.TabIndex = 0;
            this.GamePresetLabel.Text = "Game Preset";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CancleButton);
            this.panel2.Controls.Add(this.OKButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 360);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(342, 39);
            this.panel2.TabIndex = 2;
            // 
            // CancleButton
            // 
            this.CancleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancleButton.Location = new System.Drawing.Point(183, 13);
            this.CancleButton.Name = "CancleButton";
            this.CancleButton.Size = new System.Drawing.Size(75, 23);
            this.CancleButton.TabIndex = 1;
            this.CancleButton.Text = "Cancel";
            this.CancleButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(264, 13);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // ZoneSaveOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 399);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ZonesDataGridView);
            this.Controls.Add(this.panel1);
            this.Name = "ZoneSaveOptionsDialog";
            this.Text = "Almost done";
            ((System.ComponentModel.ISupportInitialize)(this.ZonesDataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ZonesDataGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox StageNameTextBox;
        private System.Windows.Forms.ComboBox PresetComboBox;
        private System.Windows.Forms.Label ZonesLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label GamePresetLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button CancleButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn OriginalNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewNameColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ShouldSaveColumn;
    }
}
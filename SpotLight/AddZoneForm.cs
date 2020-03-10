using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    class AddZoneForm : Form
    {
        #region generated code
        private ListBox ZonesListBox;
        private Button OKButton;
        private Button CancelSelectionButton;
        private CheckBox FilterZonesCheckbox;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddZoneForm));
            this.ZonesListBox = new System.Windows.Forms.ListBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelSelectionButton = new System.Windows.Forms.Button();
            this.FilterZonesCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ZonesListBox
            // 
            this.ZonesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ZonesListBox.FormattingEnabled = true;
            this.ZonesListBox.Location = new System.Drawing.Point(12, 12);
            this.ZonesListBox.Name = "ZonesListBox";
            this.ZonesListBox.Size = new System.Drawing.Size(348, 368);
            this.ZonesListBox.TabIndex = 0;
            this.ZonesListBox.SelectedIndexChanged += new System.EventHandler(this.ZoneListBox_SelectedIndexChanged);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(204, 387);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelSelectionButton
            // 
            this.CancelSelectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelSelectionButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelSelectionButton.Location = new System.Drawing.Point(285, 387);
            this.CancelSelectionButton.Name = "CancelSelectionButton";
            this.CancelSelectionButton.Size = new System.Drawing.Size(75, 23);
            this.CancelSelectionButton.TabIndex = 2;
            this.CancelSelectionButton.Text = "Cancel";
            this.CancelSelectionButton.UseVisualStyleBackColor = true;
            // 
            // FilterZonesCheckbox
            // 
            this.FilterZonesCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FilterZonesCheckbox.AutoSize = true;
            this.FilterZonesCheckbox.Location = new System.Drawing.Point(12, 387);
            this.FilterZonesCheckbox.Name = "FilterZonesCheckbox";
            this.FilterZonesCheckbox.Size = new System.Drawing.Size(81, 17);
            this.FilterZonesCheckbox.TabIndex = 3;
            this.FilterZonesCheckbox.Text = "Filter Zones";
            this.FilterZonesCheckbox.UseVisualStyleBackColor = true;
            this.FilterZonesCheckbox.CheckedChanged += new System.EventHandler(this.FilterZonesCheckbox_CheckedChanged);
            // 
            // AddZoneForm
            // 
            this.ClientSize = new System.Drawing.Size(372, 422);
            this.Controls.Add(this.FilterZonesCheckbox);
            this.Controls.Add(this.CancelSelectionButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.ZonesListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddZoneForm";
            this.Text = "Spotlight - Add Zone";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        public AddZoneForm()
        {
            InitializeComponent();
            CenterToParent();
            FilterZonesCheckbox_CheckedChanged(null, null);

            #region Localize()
            Text = Program.CurrentLanguage.GetTranslation("AddZonesTitle") ?? "Spotlight - Add Zone";
            FilterZonesCheckbox.Text = Program.CurrentLanguage.GetTranslation("FilterZonesCheckbox") ?? "Filter Zones";
            OKButton.Text = Program.CurrentLanguage.GetTranslation("OKButton") ?? "OK";
            CancelSelectionButton.Text = Program.CurrentLanguage.GetTranslation("CancelSelectionButton") ?? "Cancel";
            #endregion
        }

        public string SelectedFileName { get; private set; }

        private void FilterZonesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ZonesListBox.Items.Clear();

            foreach (string filePath in Directory.EnumerateFiles(Program.ProjectPath.Equals("") ? Program.BaseStageDataPath: Path.Combine(Program.ProjectPath, "StageData")))
            {
                if (!filePath.EndsWith("Map1.szs"))
                    continue;
                if (FilterZonesCheckbox.Checked && !filePath.Contains("Zone"))
                    continue;

                ZonesListBox.Items.Add(Path.GetFileName(filePath));
            }
        }

        private void ZoneListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFileName = (string)ZonesListBox.SelectedItem;
        }
    }
}

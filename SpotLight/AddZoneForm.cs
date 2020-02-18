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
        private ListBox listBox1;
        private Button OKBtn;
        private Button CancelBtn;
        private CheckBox FilterZonesCheckbox;

        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.FilterZonesCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(348, 368);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(204, 387);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 1;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(285, 387);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
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
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.listBox1);
            this.Name = "AddZoneForm";
            this.Text = "Add Zone";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        public AddZoneForm()
        {
            InitializeComponent();

            FilterZonesCheckbox_CheckedChanged(null, null);
        }

        public string SelectedFileName { get; private set; }

        private void FilterZonesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (string filePath in Directory.EnumerateFiles(Program.StageDataPath))
            {
                if (!filePath.EndsWith("Map1.szs"))
                    continue;
                if (FilterZonesCheckbox.Checked && !filePath.Contains("Zone"))
                    continue;

                listBox1.Items.Add(Path.GetFileName(filePath));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFileName = (string)listBox1.SelectedItem;
        }
    }
}

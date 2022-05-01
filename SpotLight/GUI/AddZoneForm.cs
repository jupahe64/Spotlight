using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight
{
    class AddZoneForm : Form
    {
        #region generated code
        private Button OKButton;
        private Button CancelSelectionButton;
        private BrightIdeasSoftware.FastObjectListView ZoneListView;
        private BrightIdeasSoftware.OLVColumn DirectoryColumn;
        private BrightIdeasSoftware.OLVColumn StageNameColumn;
        private Label label1;
        private TextBox SearchTextBox;
        private Panel panel1;
        private Panel panel2;
        private CheckBox FilterZonesCheckbox;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddZoneForm));
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelSelectionButton = new System.Windows.Forms.Button();
            this.FilterZonesCheckbox = new System.Windows.Forms.CheckBox();
            this.ZoneListView = new BrightIdeasSoftware.FastObjectListView();
            this.StageNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.DirectoryColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.ZoneListView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OKButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(216, 3);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 29);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelSelectionButton
            // 
            this.CancelSelectionButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelSelectionButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.CancelSelectionButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelSelectionButton.Location = new System.Drawing.Point(291, 3);
            this.CancelSelectionButton.Name = "CancelSelectionButton";
            this.CancelSelectionButton.Size = new System.Drawing.Size(75, 29);
            this.CancelSelectionButton.TabIndex = 2;
            this.CancelSelectionButton.Text = "Cancel";
            this.CancelSelectionButton.UseVisualStyleBackColor = true;
            // 
            // FilterZonesCheckbox
            // 
            this.FilterZonesCheckbox.AutoSize = true;
            this.FilterZonesCheckbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.FilterZonesCheckbox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilterZonesCheckbox.Location = new System.Drawing.Point(0, 3);
            this.FilterZonesCheckbox.Name = "FilterZonesCheckbox";
            this.FilterZonesCheckbox.Size = new System.Drawing.Size(97, 29);
            this.FilterZonesCheckbox.TabIndex = 3;
            this.FilterZonesCheckbox.Text = "Filter Zones";
            this.FilterZonesCheckbox.UseVisualStyleBackColor = true;
            this.FilterZonesCheckbox.CheckedChanged += new System.EventHandler(this.FilterZonesCheckbox_CheckedChanged);
            // 
            // ZoneListView
            // 
            this.ZoneListView.AllColumns.Add(this.StageNameColumn);
            this.ZoneListView.AllColumns.Add(this.DirectoryColumn);
            this.ZoneListView.CellEditUseWholeCell = false;
            this.ZoneListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.StageNameColumn,
            this.DirectoryColumn});
            this.ZoneListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.ZoneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZoneListView.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoneListView.FullRowSelect = true;
            this.ZoneListView.HideSelection = false;
            this.ZoneListView.Location = new System.Drawing.Point(3, 37);
            this.ZoneListView.MultiSelect = false;
            this.ZoneListView.Name = "ZoneListView";
            this.ZoneListView.SelectAllOnControlA = false;
            this.ZoneListView.ShowGroups = false;
            this.ZoneListView.Size = new System.Drawing.Size(366, 347);
            this.ZoneListView.TabIndex = 4;
            this.ZoneListView.UseCompatibleStateImageBehavior = false;
            this.ZoneListView.View = System.Windows.Forms.View.Details;
            this.ZoneListView.VirtualMode = true;
            this.ZoneListView.SelectedIndexChanged += new System.EventHandler(this.ZoneListView_SelectedIndexChanged);
            // 
            // StageNameColumn
            // 
            this.StageNameColumn.CellEditUseWholeCell = true;
            this.StageNameColumn.FillsFreeSpace = true;
            this.StageNameColumn.IsEditable = false;
            this.StageNameColumn.Text = "Stage Name";
            // 
            // DirectoryColumn
            // 
            this.DirectoryColumn.CellEditUseWholeCell = true;
            this.DirectoryColumn.IsEditable = false;
            this.DirectoryColumn.Searchable = false;
            this.DirectoryColumn.Text = "Directory";
            this.DirectoryColumn.Width = 105;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Search";
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchTextBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchTextBox.Location = new System.Drawing.Point(47, 0);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(319, 25);
            this.SearchTextBox.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SearchTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 34);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.OKButton);
            this.panel2.Controls.Add(this.CancelSelectionButton);
            this.panel2.Controls.Add(this.FilterZonesCheckbox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 384);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel2.Size = new System.Drawing.Size(366, 35);
            this.panel2.TabIndex = 8;
            // 
            // AddZoneForm
            // 
            this.ClientSize = new System.Drawing.Size(372, 422);
            this.Controls.Add(this.ZoneListView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddZoneForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Spotlight - Add Zone";
            ((System.ComponentModel.ISupportInitialize)(this.ZoneListView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        string currentDirectory;

        HashSet<StageInfo> zoneInfos = new HashSet<StageInfo>();

        public AddZoneForm(string currentDirectory)
        {
            this.currentDirectory = currentDirectory;

            InitializeComponent();
            CenterToParent();

            DirectoryColumn.AspectName = nameof(StageInfo.Directory);
            StageNameColumn.AspectName = nameof(StageInfo.StageName);

            DirectoryColumn.AspectToStringConverter = x => (string)x == Program.BaseStageDataPath ? "GamePath" : "Current Directory";

            #region search engine
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            #endregion

            FilterZonesCheckbox_CheckedChanged(null, null);

            #region Localize()

            this.Localize(FilterZonesCheckbox, DirectoryColumn, StageNameColumn);

            OKButton.Text = Program.CurrentLanguage.GetTranslation("OKButton") ?? "OK";
            CancelSelectionButton.Text = Program.CurrentLanguage.GetTranslation("CancelButton") ?? "Cancel";
            #endregion
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            bool hasSearchTerm = !string.IsNullOrEmpty(SearchTextBox.Text);

            object selected = ZoneListView.SelectedObject;
            if (hasSearchTerm)
            {
                List<StageInfo> beginsWithTerm = new List<StageInfo>();
                List<StageInfo> containsTerm = new List<StageInfo>();

                foreach (StageInfo stageInfo in zoneInfos)
                {
                    int matchIndex = stageInfo.StageName.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase);

                    if (matchIndex == 0)
                        beginsWithTerm.Add(stageInfo);
                    else if (matchIndex > -1)
                        containsTerm.Add(stageInfo);
                }

                beginsWithTerm.AddRange(containsTerm);

                ZoneListView.SetObjects(beginsWithTerm);
            }
            else
            {
                ZoneListView.SetObjects(zoneInfos);
            }

            ZoneListView.SelectedObject = selected;
            ZoneListView.FocusedObject = selected;
        }

        public StageInfo SelectedStageInfo { get; private set; }

        private void FilterZonesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            zoneInfos.Clear();

            foreach (string filePath in Directory.EnumerateFiles(Program.BaseStageDataPath))
            {
                if (SM3DWorldZone.TryGetStageInfo(filePath, out StageInfo? _stageInfo))
                {
                    StageInfo stageInfo = _stageInfo.Value;

                    if (FilterZonesCheckbox.Checked && !stageInfo.StageName.EndsWith("Zone"))
                        continue;

                    stageInfo.StageArcType = StageArcType.NotSpecified;

                    zoneInfos.Add(stageInfo);
                }
            }

            if(currentDirectory!= Program.BaseStageDataPath)
            {
                foreach (string filePath in Directory.EnumerateFiles(currentDirectory))
                {
                    if (SM3DWorldZone.TryGetStageInfo(filePath, out StageInfo? _stageInfo))
                    {
                        StageInfo stageInfo = _stageInfo.Value;

                        if (FilterZonesCheckbox.Checked && !stageInfo.StageName.EndsWith("Zone"))
                            continue;

                        zoneInfos.Remove(new StageInfo(Program.BaseStageDataPath, stageInfo.StageName));

                        stageInfo.StageArcType = StageArcType.NotSpecified;

                        zoneInfos.Add(stageInfo);
                    }
                }
            }

            SearchTextBox_TextChanged(null, null);
        }

        private void ZonesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OKButton.PerformClick();
        }

        private void ZoneListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedStageInfo = (StageInfo)ZoneListView.SelectedObject;
        }
    }
}

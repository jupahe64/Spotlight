using Spotlight.Level;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight.GUI
{
    public partial class ZoneSaveOptionsDialog : Form
    {
        public string StageName => StageNameTextBox.Text;
        public ByteOrder ByteOrder => ((GamePreset)PresetComboBox.SelectedValue).ByteOrder;
        public StageArcType StageArcType => ((GamePreset)PresetComboBox.SelectedValue).StageArcType;

        public class Entry
        {
            public Entry(string stageName, bool forceSaving = false)
            {
                OriginalName = stageName;
                NewName = stageName;

                ForceSaving = forceSaving;
                ShouldSave = forceSaving;
            }

            public bool ForceSaving { get; private set; }

            public string OriginalName { get; private set; }
            public string NewName { get;  set; }

            public bool ShouldSave { get; set; }
        }

        public Entry[] AdditionalZoneEntries { get; private set; }

        KeyValuePair<string, GamePreset>[] GamePresets = new KeyValuePair<string, GamePreset>[]
        {
#if ODYSSEY
            new ("Super Mario Odyssey", new GamePreset(ByteOrder.LittleEndian, StageArcType.Split))
#else
            new ("Super Mario 3D World",                          new GamePreset(ByteOrder.BigEndian,    StageArcType.Split)),
            new ("SM3DW + Bowsers Fury",                          new GamePreset(ByteOrder.LittleEndian, StageArcType.Combined)),
            new ("Captain Toad Treasure Tracker (Wii U)",         new GamePreset(ByteOrder.BigEndian,    StageArcType.Split)),
            new ("Captain Toad Treasure Tracker (Switch)",        new GamePreset(ByteOrder.LittleEndian, StageArcType.Split)),
            new ("Captain Toad Treasure Tracker (Switch Update)", new GamePreset(ByteOrder.LittleEndian, StageArcType.Combined)),
#endif
        };

        public ZoneSaveOptionsDialog(StageInfo stageInfo, ByteOrder byteOrder, List<SM3DWorldZone> additionalZones)
        {
            InitializeComponent();

            StageNameTextBox.Text = stageInfo.StageName;

            PresetComboBox.DataSource = GamePresets;
            PresetComboBox.DisplayMember = "Key";
            PresetComboBox.ValueMember = "Value";

            PresetComboBox.SelectedValue = new GamePreset(byteOrder, stageInfo.StageArcType);

            if (PresetComboBox.SelectedIndex == -1)
                PresetComboBox.SelectedIndex = 0;

            AdditionalZoneEntries = additionalZones.Select(x => new Entry(x.StageInfo.StageName, !x.IsSaved)).ToArray();

            ZonesDataGridView.AutoGenerateColumns = false;

            ZonesDataGridView.DataSource = AdditionalZoneEntries;

            OriginalNameColumn.DataPropertyName = "OriginalName";
            NewNameColumn.DataPropertyName = "NewName";
            ShouldSaveColumn.DataPropertyName = "ShouldSave";

            this.Localize(NameLabel, GamePresetLabel, ZonesLabel, OriginalNameColumn, NewNameColumn, ShouldSaveColumn);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            for (int i = 0; i < AdditionalZoneEntries.Length; i++)
            {
                if (AdditionalZoneEntries[i].ForceSaving)
                {
                    ZonesDataGridView.Rows[i].Cells[2].ReadOnly = true;
                    ZonesDataGridView.Rows[i].Cells[2].Style.BackColor = SystemColors.ControlLight;
                    ZonesDataGridView.Rows[i].Cells[2].Style.SelectionBackColor = SystemColors.ControlLight;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (string.IsNullOrEmpty(StageNameTextBox.Text))
            {
                e.Cancel = true;
                SystemSounds.Beep.Play();
                StageNameTextBox.Focus();
            }
        }
    }

    internal struct GamePreset
    {
        public ByteOrder ByteOrder;
        public StageArcType StageArcType;

        public GamePreset(ByteOrder byteOrder, StageArcType arcType)
        {
            ByteOrder = byteOrder;
            StageArcType = arcType;
        }

        public override bool Equals(object obj)
        {
            return obj is GamePreset other &&
                   ByteOrder == other.ByteOrder &&
                   StageArcType == other.StageArcType;
        }

        public override int GetHashCode()
        {
            int hashCode = 257539868;
            hashCode = hashCode * -1521134295 + ByteOrder.GetHashCode();
            hashCode = hashCode * -1521134295 + StageArcType.GetHashCode();
            return hashCode;
        }
    }
}

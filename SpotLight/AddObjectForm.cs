using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpotLight.ObjectParamDatabase;
using Spotlight.ObjectDescDatabase;
using SpotLight.EditorDrawables;

namespace SpotLight
{
    public partial class AddObjectForm : Form
    {
        public AddObjectForm(ObjectParameterDatabase Database)
        {
            InitializeComponent();
            CenterToParent();
            ObjectSelectListView.ShowGroups = true;
            for (int i = 0; i < Database.ObjectParameters.Count; i++)
            {
                ListViewGroup LVG = null;
                if (Database.ObjectParameters[i].CategoryID >= 0 && Database.ObjectParameters[i].CategoryID <= 7)
                {
                    LVG = ObjectSelectListView.Groups[Database.ObjectParameters[i].CategoryID];
                }
                else if (Database.ObjectParameters[i].CategoryID == 8)
                {

                }

                ListViewItem LVI = new ListViewItem(new string[] { Database.ObjectParameters[i].ClassName }) { Group = LVG, Tag = Database.ObjectParameters[i] };
                ObjectSelectListView.Items.Add(LVI);
            }
        }
        public int SelectedIndex => ObjectSelectListView.SelectedIndices[0];
        bool Loading = false;
        bool Edited = false;
        bool YesObjectIsChosen = false;
        ObjectDescriptionDatabase ODD = new ObjectDescriptionDatabase();
        private void ObjectSelectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ObjectSelectListView.SelectedItems.Count == 0)
                return;
            Loading = true;
            Description tmp = ODD.GetDescription(ObjectSelectListView.SelectedItems[0].SubItems[0].Text);
            ObjectDescriptionTextBox.Text = tmp.Text;
            ObjectNameGroupBox.Text = tmp.ObjectName;
            Loading = false;
        }

        private void ObjectDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading || ObjectSelectListView.SelectedItems.Count == 0)
                return;
            ODD.SetDescription(ObjectSelectListView.SelectedItems[0].SubItems[0].Text, ObjectDescriptionTextBox.Text);
            Edited = true;
        }

        private void AddObjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Edited)
            {
                DialogResult DR = MessageBox.Show("You edited one or more object descriptions\nWould you like to save?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (DR == DialogResult.Yes)
                {
                    //Save Database
                }
                else if (DR == DialogResult.Cancel)
                    e.Cancel = true;
            }
            if (YesObjectIsChosen)
            {
                DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.Cancel;
        }
        private void SelectObjectButton_Click(object sender, EventArgs e)
        {
            if (Loading || ObjectSelectListView.SelectedItems.Count == 0)
                return;

            YesObjectIsChosen = true;
            Close();
        }
    }
}

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
using OpenTK;
using SpotLight.Level;
using GL_EditorFramework.GL_Core;

namespace SpotLight
{
    public partial class AddObjectForm : Form
    {
        public AddObjectForm(ObjectParameterDatabase database, SM3DWorldScene scene, GL_ControlModern control)
        {
            InitializeComponent();
            CenterToParent();
            ObjectSelectListView.ShowGroups = true;
            for (int i = 0; i < database.ObjectParameters.Count; i++)
            {
                ListViewGroup LVG = null;
                if (database.ObjectParameters[i].CategoryID >= 0 && database.ObjectParameters[i].CategoryID <= 7)
                {
                    LVG = ObjectSelectListView.Groups[database.ObjectParameters[i].CategoryID];
                }
                else if (database.ObjectParameters[i].CategoryID == 8)
                {

                }

                ListViewItem LVI = new ListViewItem(new string[] { database.ObjectParameters[i].ClassName }) { Group = LVG, Tag = database.ObjectParameters[i] };
                ObjectSelectListView.Items.Add(LVI);
            }
            if (System.IO.File.Exists(Program.SODDPath))
                ODD = new ObjectDescriptionDatabase(Program.SODDPath);

            this.scene = scene;
            this.control = control;
            this.database = database;
        }

        SM3DWorldScene scene;
        GL_ControlModern control;
        ObjectParameterDatabase database;

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
                    ODD.Save(Program.SODDPath);
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

            scene.ObjectPlaceDelegate = PlaceObject;

            Close();
        }

        enum Category : byte
        {
            Area,
            CheckPoint,
            Demo,
            Goal,
            Object,
            Player,
            Sky,
            Link
        }

        private (I3dWorldObject, ObjectList)[] PlaceObject(Vector3 Position, SM3DWorldZone Zone)
        {
            Parameter entry = database.ObjectParameters[SelectedIndex];
            Category category = (Category)entry.CategoryID;

            ObjectList objList;
            if (category == Category.Link)
                objList = Zone.LinkedObjects;
            else
                objList = Zone.ObjLists[SM3DWorldZone.MAP_PREFIX + category.ToString() + "List"];

            General3dWorldObject currentobject = entry.ToGeneral3DWorldObject(Zone.NextObjID(), -Position);
            currentobject.DoModelLoad(control);
            return new (I3dWorldObject, ObjectList)[] { 
                (currentobject, objList)
            };
        }
    }
}

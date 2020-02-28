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
using Spotlight.ObjectInformationDatabase;
using SpotLight.EditorDrawables;
using OpenTK;
using SpotLight.Level;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.EditorDrawables;
using System.Reflection;

namespace SpotLight
{
    public partial class AddObjectForm : Form
    {
        public AddObjectForm(ObjectParameterDatabase database, SM3DWorldScene scene, GL_ControlModern control)
        {
            InitializeComponent();
            CenterToParent();
            ObjectSelectListView.ShowGroups = true;
            ObjectSelectListView.DoubleBuffering(true);
            FullItems = new ListViewItem[database.ObjectParameters.Count];
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

                ListViewItem LVI = new ListViewItem(new string[] { database.ObjectParameters[i].ClassName, database.ObjectParameters[i].ObjectNames.Count.ToString().PadLeft(3, '0'), database.ObjectParameters[i].ModelNames.Count.ToString().PadLeft(3, '0') }) { Group = LVG, Tag = database.ObjectParameters[i] };
                FullItems[i] = LVI;
            }
            ObjectSelectListView.Items.AddRange(FullItems);
            if (System.IO.File.Exists(Program.SODDPath))
                ODD = new ObjectInformationDatabase(Program.SODDPath);

            this.scene = scene;
            this.control = control;
            this.database = database;
        }

        readonly SM3DWorldScene scene;
        readonly GL_ControlModern control;
        readonly ObjectParameterDatabase database;
        ListViewItem[] FullItems;

        public int SelectedIndex => ObjectSelectListView.SelectedIndices[0];
        bool Loading = false;
        bool Edited = false;
        ObjectInformationDatabase ODD = new ObjectInformationDatabase();
        private void ObjectSelectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ObjectSelectListView.SelectedItems.Count == 0)
            {
                ClassNameLabel.Text = "Nothing Selected";
                SelectObjectListView.Items.Clear();
                SelectModelListView.Items.Clear();
                PropertyNotesListView.Items.Clear();
                SelectObjectListView.Enabled = false;
                SelectModelListView.Enabled = false;
                PropertyNotesListView.Enabled = false;
                return;
            }
            Loading = true;
            Information tmp = ODD.GetInformation(ObjectSelectListView.SelectedItems[0].SubItems[0].Text);
            ClassNameLabel.Text = tmp.ClassName;
            ObjectDescriptionTextBox.Text = tmp.Description;
            Parameter Param = (Parameter)ObjectSelectListView.SelectedItems[0].Tag;
            for (int i = 0; i < Param.ObjectNames.Count; i++)
            {
                SelectObjectListView.Items.Add(new ListViewItem(new string[] { Param.ObjectNames[i] }));
                SelectObjectListView.Items[0].Selected = true;
                SelectObjectListView.Enabled = true;
            }
            for (int i = 0; i < Param.ModelNames.Count; i++)
            {
                SelectModelListView.Items.Add(new ListViewItem(new string[] { Param.ModelNames[i] }));
                SelectModelListView.Items[0].Selected = true;
                SelectModelListView.Enabled = true;
            }
            for (int i = 0; i < Param.Properties.Count; i++)
            {
                PropertyNotesListView.Items.Add(new ListViewItem(new string[] { Param.Properties[i].Key, Param.Properties[i].Value, tmp.GetNoteForProperty(Param.Properties[i].Key) }));
                PropertyNotesListView.Items[0].Selected = true;
                PropertyNotesListView.Enabled = true;
                PropertyHintTextBox.Text = PropertyNotesListView.SelectedItems[0].SubItems[2].Text;
            }

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
                DialogResult result = MessageBox.Show("You edited one or more object descriptions\nWould you like to save?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    ODD.Save(Program.SODDPath);
                }
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }
        private void SelectObjectButton_Click(object sender, EventArgs e)
        {
            if (ObjectTypeTabControl.SelectedTab == ObjectFromDBTab)
            {
                if (Loading || ObjectSelectListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("You need to select an object from the Database", "No object selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                scene.ObjectPlaceDelegate = PlaceObjectFromDB;
                Close();
                return;

            }

            if (ObjectTypeTabControl.SelectedTab == RailTab)
            {
                scene.ObjectPlaceDelegate = PlaceRail;
                Close();
                return;

            }



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

        private (I3dWorldObject, ObjectList)[] PlaceObjectFromDB(Vector3 pos, SM3DWorldZone zone)
        {
            Parameter entry = database.ObjectParameters[SelectedIndex];
            Category category = (Category)entry.CategoryID;

            ObjectList objList;
            if (category == Category.Link)
                objList = zone.LinkedObjects;
            else
                objList = zone.ObjLists[SM3DWorldZone.MAP_PREFIX + category.ToString() + "List"];

            General3dWorldObject obj = entry.ToGeneral3DWorldObject(zone.NextObjID(), pos);
            obj.Prepare(control);
            return new (I3dWorldObject, ObjectList)[] { 
                (obj, objList)
            };
        }

        private (I3dWorldObject, ObjectList)[] PlaceRail(Vector3 pos, SM3DWorldZone zone)
        {
            List<PathPoint> pathPoints = new List<PathPoint>()
            {
                new RailPoint(pos+new Vector3(-3,0,0), Vector3.Zero, Vector3.Zero),
                new RailPoint(pos+new Vector3(3,0,0), Vector3.Zero, Vector3.Zero),
            };

            Rail rail = new Rail(pathPoints, zone.NextObjID(), false, false, false, (Rail.RailObjType)RailTypeComboBox.SelectedIndex);


            rail.Prepare(control);
            return new (I3dWorldObject, ObjectList)[] {
                (rail, zone.ObjLists["Map_Rails"])
            };
        }

        private void AddObjectForm_Load(object sender, EventArgs e)
        {
            RailTypeComboBox.SelectedIndex = 0;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string Search = SearchTextBox.Text.ToLower();
            ObjectSelectListView.Groups[10].Header = "Search Results: {RESULT}".Replace("{RESULT}", SearchTextBox.Text);
            if (Search.Equals(""))
            {
                ObjectSelectListView.Items.Clear();
                ObjectSelectListView.Items.AddRange(FullItems);
            }
            else
            {
                ObjectSelectListView.Items.Clear();
                for (int i = 0; i < database.ObjectParameters.Count; i++)
                {
                    Parameter Param = (Parameter)FullItems[i].Tag;
                    if (!Param.ClassName.ToLower().Contains(Search))
                        continue;



                    ObjectSelectListView.Items.Add(new ListViewItem(new string[] { Param.ClassName, Param.ObjectNames.Count.ToString().PadLeft(3,'0'), Param.ModelNames.Count.ToString().PadLeft(3, '0') }) { Group = ObjectSelectListView.Groups[10], Tag = Param });
                }
                if (ObjectSelectListView.Items.Count == 0)
                    ObjectSelectListView.Items.Add(new ListViewItem(new string[] { "No Results for "+SearchTextBox.Text, "---", "---" }) { Group = ObjectSelectListView.Groups[10] });
            }
        }

        private void PropertyNotesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || ObjectSelectListView.SelectedItems.Count == 0 || PropertyNotesListView.SelectedItems.Count == 0)
                return;

            Loading = true;
            PropertyHintTextBox.Text = PropertyNotesListView.SelectedItems[0].SubItems[2].Text;
            Loading = false;
        }

        private void PropertyHintTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading || ObjectSelectListView.SelectedItems.Count == 0 || PropertyNotesListView.SelectedItems.Count == 0)
                return;
            Information temp = ODD.GetInformation(ObjectSelectListView.SelectedItems[0].SubItems[0].Text);
            temp.SetNoteForProperty(PropertyNotesListView.SelectedItems[0].SubItems[0].Text, PropertyHintTextBox.Text);
            ODD.SetInformation(ObjectSelectListView.SelectedItems[0].SubItems[0].Text, temp);
            PropertyNotesListView.SelectedItems[0].SubItems[2].Text = PropertyHintTextBox.Text;
            Edited = true;
        }
    }
    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enable });
        }
    }
}

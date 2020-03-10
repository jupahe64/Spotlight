using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using OpenTK;
using Spotlight.ObjectInformationDatabase;
using SpotLight.EditorDrawables;
using SpotLight.Level;
using SpotLight.ObjectParamDatabase;


namespace SpotLight
{
    public partial class AddObjectForm : Form
    {
        public AddObjectForm(SM3DWorldScene scene, GL_ControlModern control)
        {
            InitializeComponent();
            CenterToParent();
            Localize();
            DBEntryListView.ShowGroups = true;
            DBEntryListView.DoubleBuffering(true);
            if (System.IO.File.Exists(Program.SODDPath))
                OID = new ObjectInformationDatabase(Program.SODDPath);
            FullItems = new ListViewItem[Program.ParameterDB.ObjectParameters.Count];
            int i = 0;
            foreach(var parameter in Program.ParameterDB.ObjectParameters.Values)
            {
                ListViewGroup LVG = null;
                if (parameter.ObjList >= 0 && parameter.ObjList <= ObjList.Linked)
                {
                    LVG = DBEntryListView.Groups[(byte)parameter.ObjList];
                }

                ListViewItem LVI = new ListViewItem(new string[] 
                { 
                    parameter.ClassName, 
                    OID.GetInformation(parameter.ClassName).EnglishName ?? parameter.ClassName, 
                    parameter.ObjectNames.Count.ToString().PadLeft(3, '0'),
                    parameter.ModelNames.Count.ToString().PadLeft(3, '0') }) { Group = LVG, Tag = parameter };
                FullItems[i++] = LVI;
            }
            DBEntryListView.Items.AddRange(FullItems);

            this.scene = scene;
            this.control = control;
        }

        private void AddObjectForm_Load(object sender, EventArgs e)
        {
            RailTypeComboBox.SelectedIndex = 0;
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Line [2-Points]", "A Basic Line consisting of 2 path points." }) { Tag = PathPointFormations.BasicPath });
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Circle [4-Points]", "A Basic Circle consisting of 4 path points. Recommended to be closed." }) { Tag = PathPointFormations.CirclePath });
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Square [4-Points]", "A Basic Square consisting of 4 path points. Recommended to be closed." }) { Tag = PathPointFormations.SquarePath });
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Rectangle [4-Points]", "A Basic Rectangle consisting of 4 path points. Recommended to be closed." }) { Tag = PathPointFormations.RectanglePath });
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Rounded Rectangle [6-Points]", "A Rounded Rectangle consisting of 6 path points. Recommended to be closed." }) { Tag = PathPointFormations.RoundedRectanglePath });
            RailFormationListView.Items.Add(new ListViewItem(new string[] { "Rounded Square [8-Points]", "A Rounded Square consisting of 8 path points. Recommended to be closed." }) { Tag = PathPointFormations.RoundedSquarePath });
            RailFormationListView.Items[0].Selected = true;
        }

        readonly SM3DWorldScene scene;
        readonly GL_ControlModern control;
        readonly ListViewItem[] FullItems;
        private Information ObjectInformation;

        public string SelectedClassName => DBEntryListView.SelectedItems[0].SubItems[0].Text;
        bool Loading = false;
        bool Edited = false;
        readonly ObjectInformationDatabase OID = new ObjectInformationDatabase();
        private void ObjectSelectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectObjectListView.Items.Clear();
            SelectModelListView.Items.Clear();
            PropertyNotesListView.Items.Clear();

            if (DBEntryListView.SelectedItems.Count == 0 || DBEntryListView.SelectedItems[0].Tag == null)
            {
                ClassNameLabel.Text = "Nothing Selected";
                SelectObjectListView.Enabled = false;
                SelectModelListView.Enabled = false;
                PropertyNotesListView.Enabled = false;
                return;
            }
            Loading = true;
            ObjectInformation = OID.GetInformation(SelectedClassName);
            ClassNameLabel.Text = ObjectInformation.ClassName;
            EnglishNameTextBox.Text = (ObjectInformation.EnglishName == null || ObjectInformation.EnglishName.Length == 0) ? ObjectInformation.ClassName : ObjectInformation.EnglishName;
            ObjectDescriptionTextBox.Text = ObjectInformation.Description.Length == 0 ? "No Description Found": ObjectInformation.Description ;
            Parameter Param = (Parameter)DBEntryListView.SelectedItems[0].Tag;
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
                PropertyNotesListView.Items.Add(new ListViewItem(new string[] { Param.Properties[i].Key, Param.Properties[i].Value, ObjectInformation.GetNoteForProperty(Param.Properties[i].Key) }));
                PropertyNotesListView.Items[0].Selected = true;
                PropertyNotesListView.Enabled = true;
                PropertyHintTextBox.Text = PropertyNotesListView.SelectedItems[0].SubItems[2].Text;
            }
            PropertyLabel.Text = PropertyNotesListView.Items.Count == 0 ? "No Properties":(PropertyNotesListView.Items.Count > 1 ? $"{PropertyNotesListView.Items.Count} Properties":"1 Property");

            Loading = false;
        }

        private void ObjectDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading || DBEntryListView.SelectedItems.Count == 0)
                return;
            ObjectInformation.Description = ObjectDescriptionTextBox.Text;
            OID.SetInformation(ObjectInformation);
            Edited = true;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string Search = SearchTextBox.Text.ToLower();
            DBEntryListView.Groups[10].Header = "Search Results: {RESULT}".Replace("{RESULT}", SearchTextBox.Text);
            if (Search.Equals(""))
            {
                DBEntryListView.Items.Clear();
                DBEntryListView.Items.AddRange(FullItems);
            }
            else
            {
                DBEntryListView.Items.Clear();
                for (int i = 0; i < Program.ParameterDB.ObjectParameters.Count; i++)
                {
                    Parameter Param = (Parameter)FullItems[i].Tag;
                    Information tmp = OID.GetInformation(Param.ClassName);
                    bool a = !Param.ClassName.ToLower().Contains(Search), b = !(tmp.EnglishName ?? Param.ClassName).ToLower().Contains(Search);
                    if (a && b)
                        continue;

                    DBEntryListView.Items.Add(new ListViewItem(new string[] { Param.ClassName, OID.GetInformation(Param.ClassName).EnglishName ?? Param.ClassName, Param.ObjectNames.Count.ToString().PadLeft(3, '0'), Param.ModelNames.Count.ToString().PadLeft(3, '0') }) { Group = DBEntryListView.Groups[10], Tag = Param });
                }
                if (DBEntryListView.Items.Count == 0)
                    DBEntryListView.Items.Add(new ListViewItem(new string[] { "No Results for " + SearchTextBox.Text, "----------", "---", "---" }) { Group = DBEntryListView.Groups[10] });
            }
        }

        private void PropertyNotesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || DBEntryListView.SelectedItems.Count == 0 || PropertyNotesListView.SelectedItems.Count == 0)
                return;

            Loading = true;
            PropertyHintTextBox.Text = PropertyNotesListView.SelectedItems[0].SubItems[2].Text;
            Loading = false;
        }

        private void PropertyHintTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading || DBEntryListView.SelectedItems.Count == 0 || PropertyNotesListView.SelectedItems.Count == 0)
                return;
            ObjectInformation.SetNoteForProperty(PropertyNotesListView.SelectedItems[0].SubItems[0].Text, PropertyHintTextBox.Text);
            PropertyNotesListView.SelectedItems[0].SubItems[2].Text = PropertyHintTextBox.Text;
            OID.SetInformation(ObjectInformation);
            Edited = true;
        }

        private void ObjectTypeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool trig = ObjectTypeTabControl.SelectedIndex == 0;

            ClassNameLabel.Enabled = trig;
            ObjectDescriptionTextBox.Enabled = trig;
            SelectObjectLabel.Enabled = trig;
            SelectObjectListView.Enabled = trig;
            SelectModelLabel.Enabled = trig;
            SelectModelListView.Enabled = trig;
            PropertyLabel.Enabled = trig;
            PropertyNotesListView.Enabled = trig;
            PropertyHintTextBox.Enabled = trig;
            EnglishNameTextBox.Enabled = trig;
        }

        private void EnglishNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading || DBEntryListView.SelectedItems.Count == 0)
                return;
            ObjectInformation.EnglishName = EnglishNameTextBox.Text;
            OID.SetInformation(ObjectInformation);
            DBEntryListView.SelectedItems[0].SubItems[1].Text = EnglishNameTextBox.Text;
            Edited = true;
        }

        private void AddObjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Edited)
            {
                DialogResult result = MessageBox.Show("You edited one or more object descriptions\nWould you like to save?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    OID.Save(Program.SODDPath);
                }
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }
        private void SelectObjectButton_Click(object sender, EventArgs e)
        {
            if (ObjectTypeTabControl.SelectedTab == ObjectFromDBTab)
            {
                if (Loading || DBEntryListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("You need to select an object from the Database", "No object selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                scene.ObjectPlaceDelegate = PlaceObjectFromDB;

                selectedParameter = Program.ParameterDB.ObjectParameters[SelectedClassName];

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

        Parameter selectedParameter;
        /// <summary>
        /// Place an object from the Object Database with this.
        /// </summary>
        /// <param name="pos">Position to place the object at</param>
        /// <param name="zone">Zone to place the object into</param>
        /// <returns></returns>
        private (I3dWorldObject, ObjectList)[] PlaceObjectFromDB(Vector3 pos, SM3DWorldZone zone)
        {
            General3dWorldObject obj = selectedParameter.ToGeneral3DWorldObject(zone.NextObjID(), zone, pos,
                SelectObjectListView.SelectedIndices.Count == 1 ? SelectObjectListView.SelectedIndices[0] : -1,
                SelectModelListView.SelectedIndices.Count == 1 ? SelectModelListView.SelectedIndices[0] : -1);
            obj.Prepare(control);

            if (selectedParameter.TryGetObjectList(zone, out ObjectList objList))
            {
                return new (I3dWorldObject, ObjectList)[] {
                (obj, objList)
                };
            }
            else
            {
                return new (I3dWorldObject, ObjectList)[] {
                (obj, zone.LinkedObjects)
                };
            }
        }
        /// <summary>
        /// Place a rail with this.
        /// </summary>
        /// <param name="pos">Position to place the rail at</param>
        /// <param name="zone">Zone to place the rail into</param>
        /// <returns></returns>
        private (I3dWorldObject, ObjectList)[] PlaceRail(Vector3 pos, SM3DWorldZone zone)
        {
            List<PathPoint> pathPoints = PathPointFormations.GetPathFormation(pos, (List<PathPoint>)RailFormationListView.SelectedItems[0].Tag, ReverseRailCheckBox.Checked);

            Rail rail = new Rail(pathPoints, zone.NextObjID(), ClosePathCheckBox.Checked, LadderRailCheckBox.Checked, false, (Rail.RailObjType)RailTypeComboBox.SelectedIndex, zone);


            rail.Prepare(control);

            if (zone.ObjLists.ContainsKey("Map_Rails"))
            {
                return new (I3dWorldObject, ObjectList)[] {
                    (rail, zone.ObjLists["Map_Rails"])
                };
            }
            else
            {
                return new (I3dWorldObject, ObjectList)[] {
                (rail, zone.LinkedObjects)
                };
            }
        }




        private void Localize()
        {
            Text = Program.CurrentLanguage.GetTranslation("AddObjectsTitle") ?? "Spotlight - Add Object";
            ObjectFromDBTab.Text = Program.CurrentLanguage.GetTranslation("ObjectFromDBTab") ?? "Objects";
            RailTab.Text = Program.CurrentLanguage.GetTranslation("RailTab") ?? "Rails";
            SearchLabel.Text = Program.CurrentLanguage.GetTranslation("SearchText") ?? "Search";
            ClassNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("ClassNameColumnHeader") ?? "Class Name";
            EnglishNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("EnglishNameColumnHeader") ?? "Name";
            ObjectCountColumnHeader.Text = Program.CurrentLanguage.GetTranslation("ObjectCountColumnHeader") ?? "Objects";
            ModelCountColumnHeader.Text = Program.CurrentLanguage.GetTranslation("ModelCountColumnHeader") ?? "Models";

            RailTypeLabel.Text = Program.CurrentLanguage.GetTranslation("RailTypeLabel") ?? "Rail Type";
            ClosePathCheckBox.Text = Program.CurrentLanguage.GetTranslation("ClosePathCheckBox") ?? "Close the Path?";
            ReverseRailCheckBox.Text = Program.CurrentLanguage.GetTranslation("ReverseRailCheckBox") ?? "Reverse?";
            LadderRailCheckBox.Text = Program.CurrentLanguage.GetTranslation("LadderRailCheckBox") ?? "Is Ladder?";
            RailNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("RailNameColumnHeader") ?? "Rail Name";
            RailDescriptionColumnHeader.Text = Program.CurrentLanguage.GetTranslation("AddObjectDescriptionText") ?? "Description";
        }

        private string NothingSelectedText { get; set; }
        private string NoPropertiesText { get; set; }
        private string SinglePropertyText { get; set; }
        private string MultiplePropertiesText { get; set; }
        private string NoDescriptionFound { get; set; }
    }
    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enable });
        }
    }

    public static class PathPointFormations
    {
        /// <summary>
        /// Gets a List of Path Points as they are placed relative to the pos
        /// </summary>
        /// <param name="pos">Position to place at</param>
        /// <param name="Formation">Formation to place</param>
        /// <returns></returns>
        public static List<PathPoint> GetPathFormation(Vector3 pos, List<PathPoint> Formation, bool Reverse)
        {
            List<PathPoint> Final = new List<PathPoint>();
            for (int i = 0; i < Formation.Count; i++)
            {
                PathPoint pt = Formation[i];
                pt.Position += pos;
                Final.Add(pt);
            }
            if (Reverse)
                Final.Reverse();
            return Final;
        }
        /// <summary>
        /// 2-Point Path (Line)
        /// </summary>
        public static List<PathPoint> BasicPath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-3,0,0), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(3,0,0), Vector3.Zero, Vector3.Zero),
        };
        /// <summary>
        /// 4-Point Path (Circle, Recommended Closed)
        /// </summary>
        public static List<PathPoint> CirclePath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-3,0,0), new Vector3(0,0,-2), new Vector3(0,0,2)),
            new RailPoint(new Vector3(0,0,3), new Vector3(-2,0,0), new Vector3(2,0,0)),
            new RailPoint(new Vector3(3,0,0), new Vector3(0,0,2), new Vector3(0,0,-2)),
            new RailPoint(new Vector3(0,0,-3), new Vector3(2,0,0), new Vector3(-2,0,0)),
        };
        /// <summary>
        /// 4-Point Path (Square, Recommended Closed)
        /// </summary>
        public static List<PathPoint> SquarePath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-3,0,-3), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(3,0,-3), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(3,0,3), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(-3,0,3), Vector3.Zero, Vector3.Zero),
        };
        /// <summary>
        /// 4-Point Path (Rectangle, Recommended Closed)
        /// </summary>
        public static List<PathPoint> RectanglePath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-3,0,-6), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(3,0,-6), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(3,0,6), Vector3.Zero, Vector3.Zero),
            new RailPoint(new Vector3(-3,0,6), Vector3.Zero, Vector3.Zero),
        };
        /// <summary>
        /// 6-Point Path (Rounded Rectangle, Recommended Closed)
        /// </summary>
        public static List<PathPoint> RoundedRectanglePath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-3,0,-5), new Vector3(0,0,2), new Vector3(0,0,-2)),
            new RailPoint(new Vector3(0,0,-8), new Vector3(-2,0,0), new Vector3(2,0,0)),
            new RailPoint(new Vector3(3,0,-5), new Vector3(0,0,-2), new Vector3(0,0,2)),
            new RailPoint(new Vector3(3,0,5), new Vector3(0,0,-2), new Vector3(0,0,2)),
            new RailPoint(new Vector3(0,0,8), new Vector3(2,0,0), new Vector3(-2,0,0)),
            new RailPoint(new Vector3(-3,0,5), new Vector3(0,0,2), new Vector3(0,0,-2)),
        };
        /// <summary>
        /// 8-Point Path (Rounded Square, Recommended Closed)
        /// </summary>
        public static List<PathPoint> RoundedSquarePath => new List<PathPoint>()
        {
            new RailPoint(new Vector3(-6,0,-3), new Vector3(0,0,2), new Vector3(0,0,-2)),//good
            new RailPoint(new Vector3(-3,0,-6), new Vector3(-2,0,0), new Vector3(2,0,0)),//good
            new RailPoint(new Vector3(3,0,-6), new Vector3(-2,0,0), new Vector3(2,0,0)),//good
            new RailPoint(new Vector3(6,0,-3), new Vector3(0,0,-2), new Vector3(0,0,2)),//good
            new RailPoint(new Vector3(6,0,3), new Vector3(0,0,-2), new Vector3(0,0,2)),
            new RailPoint(new Vector3(3,0,6), new Vector3(2,0,0), new Vector3(-2,0,0)),
            new RailPoint(new Vector3(-3,0,6), new Vector3(2,0,0), new Vector3(-2,0,0)),
            new RailPoint(new Vector3(-6,0,3), new Vector3(0,0,2), new Vector3(0,0,-2)),
        };
    }
}

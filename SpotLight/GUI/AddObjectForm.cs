using BrightIdeasSoftware;
using OpenTK;
using Spotlight.Database;
using Spotlight.EditorDrawables;
using Spotlight.Level;
using Spotlight.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Spotlight.GUI.PathShapeSelector;
using static Spotlight.ObjectParameterForm;

namespace Spotlight.GUI
{

    public partial class AddObjectForm : Form
    {
        private struct PointFormation
        {
            public Func<List<RailPoint>> Func { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        [Program.Localized]
        string UnsavedDescriptionsHeader = "Unsafed Changes";
        [Program.Localized]
        string UnsavedDescriptionsFooter = "have unsafed changes, do you want to save them?";
        
        readonly Control focusControl = new Label() { Size = new Size() };

        private byte[] infoDB_backup;

        EditableInformation currentEditInfo;
        PropertyDef selectedProperty;

        bool currentInfoWasEdited = false;

        private readonly SM3DWorldScene scene;
        private readonly QuickFavoriteControl quickFavorites;

        private IParameter selectedParameter;

        public bool SomethingWasSelected { get; private set; }

        bool hasSearchTerm = false;

        private List<string> editedInformations = new List<string>();

        private bool ignoreTextEvents = false;

        (string searchTerm, byte[] listViewState, Point scrollPos, object selection, int selectedPropertyIndex)[] tabPageStates;

        public void OnInformationEdited()
        {
            currentInfoWasEdited = true;

            if (!editedInformations.Contains(currentEditInfo.ClassName))
            {
                editedInformations.Add(currentEditInfo.ClassName);

                Text = caption + $" ({editedInformations.Count} unsaved entries)";
            }
        }

        public AddObjectForm(SM3DWorldScene scene, QuickFavoriteControl quickFavorites)
        {
            //backup current database
            var stream = new MemoryStream();
            Program.InformationDB.Save(stream);
            infoDB_backup = stream.ToArray();



            InitializeComponent();

            ClearForm();

            CenterToParent();
            Localize();

            ObjDBListView.ShowGroups = true;


            #region Setup Columns
            ClassNameColumn.AspectGetter = (o) => (o as IParameter).ClassName;

            EnglishNameColumn.AspectGetter = (o) =>
            {
                var className = (o as IParameter).ClassName;
                return Program.InformationDB.GetInformation(className)?.EnglishName ?? className;
            };


            var categories = new string[]
            {
                "Areas",
                "CheckPoints",
                "Cutscene Objects",
                "Goals",
                "Objects",
                "Starting Points",
                "Skies",
                "Linkables",
                "Rails",

                "DemoObjList",
                "NatureList",
                "PlayerStartInfoList",
                "CapMessageList",
                "MapIconList",
                "SceneWatchObjList",
                "ScenarioStartCameraList",
                "PlayerAffectObjList",
                "RaceList"
            };

            ObjListColumn.AspectGetter = (o) => (int)(o as IParameter).ObjList;
            //CategoryColumn.AspectToStringConverter = (o) => categories[(int)o];
            ObjListColumn.AspectToStringConverter = (o) => ((ObjList)(int)o).ToString();
            #endregion

            #region search engine
            SearchTextBox.TextChanged += (x, y) =>
            {
                hasSearchTerm = !string.IsNullOrEmpty(SearchTextBox.Text);

                object selected = ObjDBListView.SelectedObject;
                if (hasSearchTerm)
                {
                    List<IParameter> beginsWithTerm = new List<IParameter>();
                    List<IParameter> containsTerm = new List<IParameter>();

                    foreach (IParameter parameter in GetParameterList())
                    {
                        var className = parameter.ClassName;
                        var englishName = Program.InformationDB.GetInformation(className).EnglishName ?? className;

                        int matchIndexA = className.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase);
                        int matchIndexB = englishName.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase);

                        if (matchIndexA == 0 || matchIndexB == 0)
                            beginsWithTerm.Add(parameter);
                        else if (matchIndexA > -1 || matchIndexB > -1)
                            containsTerm.Add(parameter);
                    }

                    beginsWithTerm.AddRange(containsTerm);

                    ObjDBListView.ShowGroups = false;
                    ObjListColumn.IsVisible = true;

                    ObjDBListView.SetObjects(beginsWithTerm);
                    ObjDBListView.RebuildColumns();
                }
                else
                {
                    ObjDBListView.ShowGroups = true;
                    ObjListColumn.IsVisible = false;

                    PopulateObjDBListView();

                    ObjDBListView.RebuildColumns();
                }

                ObjDBListView.SelectedObject = selected;
                ObjDBListView.FocusedObject = selected;
            };
            #endregion

            #region setup path formations
            PathShapeSelector.AddShape(new PathShape("Line", new PathPoint[]
            {
                new PathPoint(new Vector3(-3,0,0), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(3,0,0), Vector3.Zero, Vector3.Zero),
            },
            false));

            PathShapeSelector.AddShape(new PathShape("Circle", new PathPoint[]
            {
                new PathPoint(new Vector3(-3,0,0), new Vector3(0,0,-2), new Vector3(0,0,2)),
                new PathPoint(new Vector3(0,0,3), new Vector3(-2,0,0), new Vector3(2,0,0)),
                new PathPoint(new Vector3(3,0,0), new Vector3(0,0,2), new Vector3(0,0,-2)),
                new PathPoint(new Vector3(0,0,-3), new Vector3(2,0,0), new Vector3(-2,0,0)),
            },
            true));

            PathShapeSelector.AddShape(new PathShape("Square", new PathPoint[]
            {
                new PathPoint(new Vector3(-3,0,-3), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(3,0,-3), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(3,0,3), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(-3,0,3), Vector3.Zero, Vector3.Zero),
            },
            true));

            PathShapeSelector.AddShape(new PathShape("Rectangle", new PathPoint[]
            {
                new PathPoint(new Vector3(-3,0,-6), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(3,0,-6), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(3,0,6), Vector3.Zero, Vector3.Zero),
                new PathPoint(new Vector3(-3,0,6), Vector3.Zero, Vector3.Zero),
            },
            true));

            PathShapeSelector.AddShape(new PathShape("RoundedRect", new PathPoint[]
            {
                new PathPoint(new Vector3(-3,0,-5), new Vector3(0,0,2), new Vector3(0,0,-2)),
                new PathPoint(new Vector3(0,0,-8), new Vector3(-2,0,0), new Vector3(2,0,0)),
                new PathPoint(new Vector3(3,0,-5), new Vector3(0,0,-2), new Vector3(0,0,2)),
                new PathPoint(new Vector3(3,0,5), new Vector3(0,0,-2), new Vector3(0,0,2)),
                new PathPoint(new Vector3(0,0,8), new Vector3(2,0,0), new Vector3(-2,0,0)),
                new PathPoint(new Vector3(-3,0,5), new Vector3(0,0,2), new Vector3(0,0,-2)),
            },
            true));

            PathShapeSelector.AddShape(new PathShape("RoundedSquare", new PathPoint[]
            {
                new PathPoint(new Vector3(-6,0,-3), new Vector3(0,0,2), new Vector3(0,0,-2)),//good
                new PathPoint(new Vector3(-3,0,-6), new Vector3(-2,0,0), new Vector3(2,0,0)),//good
                new PathPoint(new Vector3(3,0,-6), new Vector3(-2,0,0), new Vector3(2,0,0)),//good
                new PathPoint(new Vector3(6,0,-3), new Vector3(0,0,-2), new Vector3(0,0,2)),//good
                new PathPoint(new Vector3(6,0,3), new Vector3(0,0,-2), new Vector3(0,0,2)),
                new PathPoint(new Vector3(3,0,6), new Vector3(2,0,0), new Vector3(-2,0,0)),
                new PathPoint(new Vector3(-3,0,6), new Vector3(2,0,0), new Vector3(-2,0,0)),
                new PathPoint(new Vector3(-6,0,3), new Vector3(0,0,2), new Vector3(0,0,-2)),
            },
            true));

            //PathShapeComboBox.SelectedIndex = 0;
            #endregion

            #region setup area shapes
            AreaShapeComboBox.Items.AddRange(LevelIO.AreaModelNames.ToArray());

            AreaShapeComboBox.SelectedIndex = 0;
            #endregion

            tabPageStates = new (string searchTerm, byte[] listViewState, Point scrollPos, object selection, int selectedPropertyIndex)[ObjectTypeTabControl.TabPages.Count];


            PopulateObjDBListView();


            this.scene = scene;
            this.quickFavorites = quickFavorites;
        }

        private void Localize()
        {
            this.Localize(
                ClassNameColumn,
                EnglishNameColumn,
                ObjListColumn,
                SearchLabel,
                PropertiesLabel,
                ToQuickFavoritesButton,
                SelectObjectButton,
                ObjectNameLabel,
                ModelNameLabel,
                ObjectsTab,
                RailsTab,
                AreasTab,
                AreaShapeLabel
                );
        }

        string caption;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            caption = Text;
        }

        private void SubmitInformation()
        {
            if (currentEditInfo != null && currentInfoWasEdited)
            {
                Program.InformationDB.SetInformation(currentEditInfo);
            }
        }

        private void EnglishNameTextBox_FocusChanged(object sender, EventArgs e)
        {
            splitContainer1.Refresh();
        }

        private void NewAddObjectForm_Load(object sender, EventArgs e)
        {
            ObjectNameTextBox.Enabled = true;
            ModelNameTextBox.Enabled = true;

            ClassNameLabel.Focus();
        }

        int? selectedPropertyIndexOverride = null;

        int lastselectedTabIndex = 0;

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SubmitInformation();

            tabPageStates[lastselectedTabIndex] = (SearchTextBox.Text, ObjDBListView.SaveState(), ObjDBListView.AutoScrollOffset, ObjDBListView.SelectedObject, PropertyListBox.SelectedIndex);

            if (tabPageStates[ObjectTypeTabControl.SelectedIndex].listViewState != null)
            {
                (var searchTerm, var listViewState, var scrollPos, var selectedObject, var selectedPropertyIndex) = tabPageStates[ObjectTypeTabControl.SelectedIndex];

                PopulateObjDBListView();

                ObjDBListView.RestoreState(listViewState);

                if (selectedObject != null)
                    ObjDBListView.SelectedObject = selectedObject;
                else
                {
                    ClearForm();
                    ObjDBListView.DeselectAll();
                }

                SearchTextBox.Text = searchTerm;
                ObjDBListView.AutoScrollOffset = scrollPos;

                selectedPropertyIndexOverride = selectedPropertyIndex;
            }
            else
            {
                PopulateObjDBListView();

                SearchTextBox.Text = string.Empty;
                ClearForm();
                ObjDBListView.DeselectAll();
            }

            lastselectedTabIndex = ObjectTypeTabControl.SelectedIndex;
        }

        private void PopulateObjDBListView()
        {
            ObjDBListView.SetObjects(GetParameterList());

            ObjDBListView.BuildGroups(ObjListColumn, SortOrder.None);
        }

        private IEnumerable<IParameter> GetParameterList()
        {
            if (ObjectTypeTabControl.SelectedTab == ObjectsTab)
            {
                return Program.ParameterDB.ObjectParameters.Values;
            }
            else if (ObjectTypeTabControl.SelectedTab == RailsTab)
            {
                return Program.ParameterDB.RailParameters.Values;
            }
            else if (ObjectTypeTabControl.SelectedTab == AreasTab)
            {
                return Program.ParameterDB.AreaParameters.Values;
            }
            else
                throw new Exception();
        }

        private void SplitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if(EnglishNameTextBox.Focused)
                e.Graphics.FillRectangle(SystemBrushes.ControlDark, Rectangle.Inflate(EnglishNameTextBox.Bounds, 1, 1));
            else
                e.Graphics.FillRectangle(SystemBrushes.ControlLight, Rectangle.Inflate(EnglishNameTextBox.Bounds, 1, 1));
        }

        private void PropertyListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            bool selected = e.State.HasFlag(DrawItemState.Selected);

            var item = (PropertyDef)PropertyListBox.Items[e.Index];

            if (selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                e.Graphics.DrawString(item.TypeDef.ActualName, Font, SystemBrushes.HighlightText, Point.Add(e.Bounds.Location, new Size(5, 3)));
                e.Graphics.DrawString(item.Name, Font, SystemBrushes.HighlightText, Point.Add(e.Bounds.Location, new Size(50, 3)));
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, e.Bounds);
                e.Graphics.DrawString(item.TypeDef.ActualName, Font, SystemBrushes.ControlText, Point.Add(e.Bounds.Location, new Size(5, 3)));
                e.Graphics.DrawString(item.Name, Font, SystemBrushes.GrayText, Point.Add(e.Bounds.Location, new Size(50, 3)));
            }

            if (currentEditInfo.Properties.ContainsKey(item.Name))
            {
                int infoX = e.Bounds.Right - 20;
                int infoY = e.Bounds.Y + 3;

                if (selected)
                    e.Graphics.DrawImage(Resources.InfoIndicatorHighlight, infoX, infoY);
                else
                    e.Graphics.DrawImage(Resources.InfoIndicator, infoX, infoY);
            }

            if (e.Index < PropertyListBox.Items.Count - 1)
            {
                int lineY = e.Bounds.Bottom - 1;
                e.Graphics.DrawLine(SystemPens.ControlDark, 0, lineY, e.Bounds.Width, lineY);
            }
        }

        private void ObjDBListView_SelectionChanged(object sender, EventArgs e)
        {
            var backup = ignoreTextEvents;
            ignoreTextEvents = true;

            selectedParameter = (IParameter)ObjDBListView.SelectedObject;

            if (selectedParameter == null)
                return;

            SubmitInformation();

            currentInfoWasEdited = false;
            currentEditInfo = Program.InformationDB.GetEditableInformation(selectedParameter.ClassName);

            ClassNameLabel.Text = selectedParameter.ClassName;

            EnglishNameTextBox.Text = currentEditInfo.EnglishName ?? selectedParameter.ClassName;

            ObjectDescriptionTextBox.Text = currentEditInfo.Description;

            List<object> items = new List<object>();

            if (ObjectTypeTabControl.SelectedTab == ObjectsTab && selectedParameter is ObjectParam param)
            {
                for (int i = 0; i < param.Properties.Count; i++)
                {
                    items.Add(param.Properties[i]);
                }

                ObjectNameTextBox.Enabled = true;
                ModelNameTextBox.Enabled = true;

                ObjectNameTextBox.PossibleSuggestions = param.ObjectNames.ToArray();
                ModelNameTextBox.PossibleSuggestions = param.ModelNames.ToArray();

                ObjectNameTextBox.Text = ObjectNameTextBox.PossibleSuggestions[0];
                ModelNameTextBox.Text = string.Empty;
            }
            else if (ObjectTypeTabControl.SelectedTab == RailsTab && selectedParameter is RailParam railParam)
            {
                for (int i = 0; i < railParam.Properties.Count; i++)
                {
                    items.Add(railParam.Properties[i]);
                }

                PathShapeSelector.Enabled = true;
            }
            else if (ObjectTypeTabControl.SelectedTab == AreasTab && selectedParameter is AreaParam areaParam)
            {
                for (int i = 0; i < areaParam.Properties.Count; i++)
                {
                    items.Add(areaParam.Properties[i]);
                }

                AreaShapeComboBox.Enabled = true;
            }

            if(scene != null)
                SelectObjectButton.Enabled = true;
            ToQuickFavoritesButton.Enabled = true;

            PropertyDescriptionTextBox.Enabled = false;
            PropertyDescriptionTextBox.Text = string.Empty;

            PropertyListBox.Items.Clear();
            PropertyListBox.Items.AddRange(items.ToArray());

            if (selectedPropertyIndexOverride.HasValue)
            {
                PropertyListBox.SelectedIndex = selectedPropertyIndexOverride.Value;
                if (selectedPropertyIndexOverride.Value == -1)
                    PropertyListBox_SelectedIndexChanged(null, null);

                selectedPropertyIndexOverride = null;
            }

            ignoreTextEvents = backup;
        }

        private void ClearForm()
        {
            var backup = ignoreTextEvents;
            ignoreTextEvents = true;

            EnglishNameTextBox.Text = string.Empty;
            ClassNameLabel.Text = string.Empty;
            ObjectDescriptionTextBox.Text = string.Empty;
            PropertyDescriptionTextBox.Text = string.Empty;

            PropertyListBox.Items.Clear();

            SelectObjectButton.Enabled = false;
            ToQuickFavoritesButton.Enabled = false;


            if (ObjectTypeTabControl.SelectedTab == ObjectsTab)
            {
                ObjectNameTextBox.Enabled = false;
                ModelNameTextBox.Enabled = false;
            }
            else if (ObjectTypeTabControl.SelectedTab == RailsTab)
            {
                PathShapeSelector.Enabled = false;
            }
            else if (ObjectTypeTabControl.SelectedTab == AreasTab)
            {
                AreaShapeComboBox.Enabled = false;
            }


            ignoreTextEvents = backup;
        }

        private void PropertyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var backup = ignoreTextEvents;
            ignoreTextEvents = true;

            if (PropertyListBox.SelectedItem == null)
            {
                PropertyDescriptionTextBox.Text = string.Empty;
                return;
            }

            selectedProperty = (PropertyDef)PropertyListBox.SelectedItem;

            if (currentEditInfo.Properties.TryGetValue(selectedProperty.Name, out string desc))
            {
                PropertyDescriptionTextBox.Text = desc;
            }
            else
            {
                PropertyDescriptionTextBox.Text = string.Empty;
            }

            PropertyDescriptionTextBox.Enabled = true;

            ignoreTextEvents = backup;
        }

        private void PropertyDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextEvents)
                return;

            if(!currentEditInfo.Properties.ContainsKey(selectedProperty.Name))
                PropertyListBox.Refresh();

            currentEditInfo.Properties[selectedProperty.Name] = PropertyDescriptionTextBox.Text;
            OnInformationEdited();
        }

        private void ObjectDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextEvents)
                return;

            currentEditInfo.Description = ObjectDescriptionTextBox.Text;
            OnInformationEdited();
        }

        private void EnglishNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && Focused)
            {
                focusControl.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void EnglishNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextEvents)
                return;

            currentEditInfo.EnglishName = EnglishNameTextBox.Text;
            OnInformationEdited();
        }



        private void NewAddObjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SubmitInformation();

            if (editedInformations.Count == 0)
                return;

            string message = "";

            foreach (var className in editedInformations)
                message += className + '\n';

            message += '\n' + UnsavedDescriptionsFooter;


            DialogResult result = MessageBox.Show(message, UnsavedDescriptionsHeader, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Program.InformationDB.Save(Program.SODDPath);
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                //restore database from backup
                Program.InformationDB = new ObjectInformationDatabase(new MemoryStream(infoDB_backup));
            }
        }

        private void ToQuickFavoritesButton_Click(object sender, EventArgs e)
        {
            if (TryGetPlacementHandler(out var placementHandler, out string text))
            {
                quickFavorites.AddFavorite(new QuickFavoriteControl.QuickFavorite(text, placementHandler));
            }
        }

        private void SelectObjectButton_Click(object sender, EventArgs e)
        {
            if (TryGetPlacementHandler(out var placementHandler, out string _))
            {
                scene.ObjectPlaceDelegate = placementHandler;

                SomethingWasSelected = true;

                Close();
            }
        }
    }
}
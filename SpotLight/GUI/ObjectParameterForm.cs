using GL_EditorFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight
{


    public partial class ObjectParameterForm : Form
    {
        public class TypeDef
        {
            public object DefaultValue { get; private set; }
            public Type Type { get; private set; }
            public string ActualName { get; private set; }
            public string AltName { get; private set; }
            public byte TypeID { get; private set; }

            public static TypeDef FromTypeID(byte typeID) => Defs[typeID];

            public static bool TryGetFromNodeType(BYAML.ByamlFile.ByamlNodeType nodeType, out TypeDef def)
            {
                switch (nodeType)
                {
                    case BYAML.ByamlFile.ByamlNodeType.Integer:
                        def = IntDef;
                        return true;

                    case BYAML.ByamlFile.ByamlNodeType.Float:
                        def = FloatDef;
                        return true;

                    case BYAML.ByamlFile.ByamlNodeType.StringIndex:
                    case BYAML.ByamlFile.ByamlNodeType.Null:
                        def = StringDef;
                        return true;

                    case BYAML.ByamlFile.ByamlNodeType.Boolean:
                        def = BoolDef;
                        return true;

                    default:
                        def = null;
                        return false;
                }
            }

            private TypeDef(Type type, object defaultValue, string actualName, string altName, byte typeID)
            {
                DefaultValue = defaultValue;
                Type = type;
                ActualName = actualName;
                AltName = altName;
                TypeID = typeID;
            }

            public override string ToString()
            {
                return UseAltNames ? AltName : ActualName;
            }

            public static readonly TypeDef[] Defs = new TypeDef[]
            {
                new TypeDef(typeof(int),    0,     "int",    String.Empty, 0),
                new TypeDef(typeof(float),  0f,    "float",  String.Empty, 1),
                new TypeDef(typeof(string), "",    "string", String.Empty, 2),
                new TypeDef(typeof(bool),   false, "bool",   String.Empty, 3)
            };

            public static readonly TypeDef IntDef = Defs[0];
            public static readonly TypeDef FloatDef = Defs[1];
            public static readonly TypeDef StringDef = Defs[2];
            public static readonly TypeDef BoolDef = Defs[3];

            public static void Localize()
            {
                IntDef.AltName = Program.CurrentLanguage.GetTranslation("TypeDefInteger") ?? "Whole Number";
                FloatDef.AltName = Program.CurrentLanguage.GetTranslation("TypeDefSingle") ?? "Decimal Number";
                StringDef.AltName = Program.CurrentLanguage.GetTranslation("TypeDefString") ?? "Text";
                BoolDef.AltName = Program.CurrentLanguage.GetTranslation("TypeDefBoolean") ?? "Checkbox";
            }
        }
        


        

        public IReadOnlyList<(TypeDef typeDef, string name)> EditedParameterInfos => MainEditorControl.parameters;

        public static bool UseAltNames = true;

        public ObjectParameterForm(List<(TypeDef typeDef, string name)> parameters)
        {
            InitializeComponent();
            Localize();

            for (int i = 0; i < TypeDef.Defs.Length; i++)
            {
                NewTypeComboBox.Items.Add(TypeDef.Defs[i].AltName);
            }
            NewTypeComboBox.SelectedIndex = 0;

            MainEditorControl.parameters = parameters;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (NewNameTextBox.Text == "")
                return;

            MainEditorControl.parameters.Add((TypeDef.Defs[NewTypeComboBox.SelectedIndex], NewNameTextBox.Text));
            NewNameTextBox.Text = "";
            MainEditorControl.Refresh();
        }

        private void CheckUseProgrammingTerms_CheckedChanged(object sender, EventArgs e)
        {
            UseAltNames = !UseProgrammingTermsCheckbox.Checked;
            MainEditorControl.Refresh();

            int tmp = NewTypeComboBox.SelectedIndex;

            NewTypeComboBox.Items.Clear();
            
            if (UseAltNames)
            {
                for (int i = 0; i < TypeDef.Defs.Length; i++)
                {
                    NewTypeComboBox.Items.Add(TypeDef.Defs[i].AltName);
                }
            }
            else
            {
                for (int i = 0; i < TypeDef.Defs.Length; i++)
                {
                    NewTypeComboBox.Items.Add(TypeDef.Defs[i].ActualName);
                }
            }

            NewTypeComboBox.SelectedIndex = tmp;
        }

        private void Localize()
        {
            Text = Program.CurrentLanguage.GetTranslation("ObjectParametersTitle") ?? "Spotlight - Object Parameter Editor";
            TypeDef.Localize();
            RemoveParameterText = Program.CurrentLanguage.GetTranslation("RemoveParameterText") ?? "Remove";
            UseProgrammingTermsCheckbox.Text = Program.CurrentLanguage.GetTranslation("UseProgrammingTermsCheckbox") ?? "Use Programming Terms";
            TypeLabel.Text = Program.CurrentLanguage.GetTranslation("GlobalTypeText") ?? "Type";
            NameLabel.Text = Program.CurrentLanguage.GetTranslation("GlobalPropertyNameText") ?? "Property Name";
            AddButton.Text = Program.CurrentLanguage.GetTranslation("ObjectParameterAddText") ?? "Add";
            OKButton.Text = Program.CurrentLanguage.GetTranslation("OKButton") ?? "OK";
            CancelSelectionButton.Text = Program.CurrentLanguage.GetTranslation("CancelSelectionButton") ?? "Cancel";
        }

        private static string TypeDefInteger { get; set; }
        private static string TypeDefSingle { get; set; }
        private static string TypeDefString { get; set; }
        private static string TypeDefBoolean { get; set; }
        internal static string RemoveParameterText { get; set; }
    }

    public class ObjectParameterEditorControl : FlexibleUIControl
    {
        public List<(ObjectParameterForm.TypeDef typeDef, string name)> parameters = new List<(ObjectParameterForm.TypeDef typeDef, string name)>();

        protected override bool HasNoUIContent() => false;
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!TryInitDrawing(e))
                return;
            
            bool alreadyRemoved = false;

            for (int i = parameters.Count-1; i >= 0; i--)
            {
                int currentY = 10 + i * 30 + AutoScrollPosition.Y;

                if (Button(usableWidth-60, currentY - 2, 50, ObjectParameterForm.RemoveParameterText) && !alreadyRemoved)
                {
                    parameters.RemoveAt(i);
                    alreadyRemoved = true;
                }
                else
                {
                    parameters[i] = (
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, parameters[i].typeDef, ObjectParameterForm.TypeDef.Defs),
                    TextInputField(170, currentY, usableWidth - 180 - 60, parameters[i].name, false)
                    );
                }
            }

            AutoScrollMinSize = new Size(0, parameters.Count * 30 + 20);
        }
    }
}

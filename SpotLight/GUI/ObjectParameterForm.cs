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

            private TypeDef(Type type, object defaultValue, string actualName, byte typeID)
            {
                DefaultValue = defaultValue;
                Type = type;
                ActualName = actualName;
                TypeID = typeID;
            }

            public override string ToString()
            {
                return ActualName;
            }

            public static readonly TypeDef[] Defs = new TypeDef[]
            {
                new TypeDef(typeof(int),    0,     "int",    0),
                new TypeDef(typeof(float),  0f,    "float",  1),
                new TypeDef(typeof(string), "",    "string", 2),
                new TypeDef(typeof(bool),   false, "bool",   3)
            };

            public static readonly TypeDef IntDef = Defs[0];
            public static readonly TypeDef FloatDef = Defs[1];
            public static readonly TypeDef StringDef = Defs[2];
            public static readonly TypeDef BoolDef = Defs[3];
        }
        


        

        public IReadOnlyList<(TypeDef typeDef, string name)> EditedParameterInfos => MainEditorControl.parameters;

        public ObjectParameterForm(List<(TypeDef typeDef, string name)> parameters)
        {
            InitializeComponent();
            Localize();

            MainEditorControl.parameters = parameters;
        }

        private void Localize()
        {
            Text = Program.CurrentLanguage.GetTranslation("ObjectParametersTitle") ?? "Spotlight - Object Parameter Editor";
            RemoveParameterText = Program.CurrentLanguage.GetTranslation("RemoveParameterText") ?? "Remove";
            TypeLabel.Text = Program.CurrentLanguage.GetTranslation("GlobalTypeText") ?? "Type";
            NameLabel.Text = Program.CurrentLanguage.GetTranslation("GlobalPropertyNameText") ?? "Property Name";
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


        (ObjectParameterForm.TypeDef typeDef, string name) newParameter = (ObjectParameterForm.TypeDef.IntDef, string.Empty);

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

                var closeIconHeight = GL_EditorFramework.Properties.Resources.CloseTabIcon.Height;

                if (ImageButton(usableWidth - 20, currentY+textBoxHeight / 2 - closeIconHeight / 2,
                    GL_EditorFramework.Properties.Resources.CloseTabIcon, null,
                    GL_EditorFramework.Properties.Resources.CloseTabIconHover) && !alreadyRemoved)
                {
                    parameters.RemoveAt(i);
                    alreadyRemoved = true;
                }
                else
                {
                    parameters[i] = (
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, parameters[i].typeDef, ObjectParameterForm.TypeDef.Defs),
                    TextInputField(170, currentY, usableWidth - 180 - 20, parameters[i].name, false)
                    );
                }
            }

            {
                int currentY = 10 + parameters.Count * 30 + AutoScrollPosition.Y;

                var old = newParameter;

                newParameter = (
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, newParameter.typeDef, ObjectParameterForm.TypeDef.Defs),
                    TextInputField(170, currentY, usableWidth - 180 - 20, newParameter.name, false)
                    );

                g.DrawString("New Parameter", Font, Brushes.Gray, 170, currentY);

                if (old.name != newParameter.name)
                {
                    parameters.Add(newParameter);

                    newParameter.name = String.Empty;
                }
            }

            AutoScrollMinSize = new Size(0, parameters.Count * 30 + 20);
        }
    }
}

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

namespace SpotLight
{
    public partial class ObjectParameterForm : Form
    {
        public struct TypeDef
        {
            public readonly object DefaultValue;
            public readonly string ActualName;
            public readonly string AltName;

            public TypeDef(object defaultValue, string actualName, string altName)
            {
                DefaultValue = defaultValue;
                ActualName = actualName;
                AltName = altName;
            }

            public override string ToString()
            {
                return useAltNames ? AltName : ActualName;
            }
        }
        
        public static TypeDef[] typeDefs = new TypeDef[]
        {
            new TypeDef(0,     "int",    "Whole Number"),
            new TypeDef(0f,    "float",  "Decimal Number"),
            new TypeDef("",    "string", "Text/Word"),
            new TypeDef(false, "bool",   "Flag")
        };

        public IReadOnlyList<(TypeDef typeDef, string name)> Parameters => editorControl.parameters;

        public static bool useAltNames = true;

        public ObjectParameterForm(List<(TypeDef typeDef, string name)> parameters)
        {
            InitializeComponent();

            for (int i = 0; i < typeDefs.Length; i++)
            {
                cbNewType.Items.Add(typeDefs[i].AltName);
            }
            cbNewType.SelectedIndex = 0;

            editorControl.parameters = parameters;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (tbNewName.Text == "")
                return;

            editorControl.parameters.Add((typeDefs[cbNewType.SelectedIndex], tbNewName.Text));
            tbNewName.Text = "";
            editorControl.Refresh();
        }

        private void CheckUseProgrammingTerms_CheckedChanged(object sender, EventArgs e)
        {
            useAltNames = !checkUseProgrammingTerms.Checked;
            editorControl.Refresh();

            int tmp = cbNewType.SelectedIndex;

            cbNewType.Items.Clear();
            
            if (useAltNames)
            {
                for (int i = 0; i < typeDefs.Length; i++)
                {
                    cbNewType.Items.Add(typeDefs[i].AltName);
                }
            }
            else
            {
                for (int i = 0; i < typeDefs.Length; i++)
                {
                    cbNewType.Items.Add(typeDefs[i].ActualName);
                }
            }

            cbNewType.SelectedIndex = tmp;
        }
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

                if (Button(usableWidth-60, currentY - 2, 50, "Remove") && !alreadyRemoved)
                {
                    parameters.RemoveAt(i);
                    alreadyRemoved = true;
                }
                else
                {
                    parameters[i] = (
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, parameters[i].typeDef, ObjectParameterForm.typeDefs),
                    TextInputField(170, currentY, usableWidth - 180 - 60, parameters[i].name, false)
                    );
                }
            }

            AutoScrollMinSize = new Size(0, parameters.Count * 30 + 20);
        }
    }
}

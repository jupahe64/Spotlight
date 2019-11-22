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
                return AltName;
            }
        }

        List<ComboBox> comboBoxes = new List<ComboBox>();
        public static TypeDef[] typeDefs = new TypeDef[]
        {
            new TypeDef(0,     "int",    "Integer"),
            new TypeDef(0f,    "float",  "Floating Point Number"),
            new TypeDef("",    "string", "Text/Word"),
            new TypeDef(false, "bool",   "Flag")
        };

        public ObjectParameterForm()
        {
            InitializeComponent();

            comboBoxes.Add(cbNewType);

            for (int i = 0; i < typeDefs.Length; i++)
            {
                cbNewType.Items.Add(typeDefs[i].AltName);
            }
        }
    }

    public class ObjectParameterEditorControl : FlexibleUIControl
    {
        List<(ObjectParameterForm.TypeDef typeDef, string name)> parameters = new List<(ObjectParameterForm.TypeDef typeDef, string name)>()
        {
            (ObjectParameterForm.typeDefs[0], "SomeParameter"),
            (ObjectParameterForm.typeDefs[3], "SomeOtherParameter")
        };

        protected override bool HasNoUIContent() => false;
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!TryInitDrawing(e))
                return;

            int currentY = 10;

            bool alreadyRemoved = false;

            for (int i = parameters.Count-1; i >= 0; i--)
            {
                if(Button(Width-60, currentY - 2, 50, "Remove") && !alreadyRemoved)
                {
                    parameters.RemoveAt(i);
                    alreadyRemoved = true;
                }
                else
                {
                    parameters[i] = (
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, parameters[i].typeDef, ObjectParameterForm.typeDefs),
                    TextInputField(170, currentY, Width - 180 - 60, parameters[i].name, false)
                    );
                    
                    currentY += 30;
                }
            }
        }
    }
}

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
                return UseAltNames ? AltName : ActualName;
            }
        }
        
        public static TypeDef[] TypeDefs => new TypeDef[]
        {
            new TypeDef(0,     "int",    TypeDefInteger),
            new TypeDef(0f,    "float",  TypeDefSingle),
            new TypeDef("",    "string", TypeDefString),
            new TypeDef(false, "bool",   TypeDefBoolean)
        };

        public IReadOnlyList<(TypeDef typeDef, string name)> Parameters => MainEditorControl.parameters;

        public static bool UseAltNames = true;

        public ObjectParameterForm(List<(TypeDef typeDef, string name)> parameters)
        {
            InitializeComponent();
            Localize();

            for (int i = 0; i < TypeDefs.Length; i++)
            {
                NewTypeComboBox.Items.Add(TypeDefs[i].AltName);
            }
            NewTypeComboBox.SelectedIndex = 0;

            MainEditorControl.parameters = parameters;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (NewNameTextBox.Text == "")
                return;

            MainEditorControl.parameters.Add((TypeDefs[NewTypeComboBox.SelectedIndex], NewNameTextBox.Text));
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
                for (int i = 0; i < TypeDefs.Length; i++)
                {
                    NewTypeComboBox.Items.Add(TypeDefs[i].AltName);
                }
            }
            else
            {
                for (int i = 0; i < TypeDefs.Length; i++)
                {
                    NewTypeComboBox.Items.Add(TypeDefs[i].ActualName);
                }
            }

            NewTypeComboBox.SelectedIndex = tmp;
        }

        public static void LocalizeTypeDefs()
        {
            TypeDefInteger = Program.CurrentLanguage.GetTranslation("TypeDefInteger") ?? "Whole Number";
            TypeDefSingle = Program.CurrentLanguage.GetTranslation("TypeDefSingle") ?? "Decimal Number";
            TypeDefString = Program.CurrentLanguage.GetTranslation("TypeDefString") ?? "Text";
            TypeDefBoolean = Program.CurrentLanguage.GetTranslation("TypeDefBoolean") ?? "Checkbox";
        }
        private void Localize()
        {
            Text = Program.CurrentLanguage.GetTranslation("ObjectParametersTitle") ?? "Spotlight - Object Parameter Editor";
            LocalizeTypeDefs();
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
                    (ObjectParameterForm.TypeDef)ChoicePickerField(10, currentY, 150, parameters[i].typeDef, ObjectParameterForm.TypeDefs),
                    TextInputField(170, currentY, usableWidth - 180 - 60, parameters[i].name, false)
                    );
                }
            }

            AutoScrollMinSize = new Size(0, parameters.Count * 30 + 20);
        }
    }
}

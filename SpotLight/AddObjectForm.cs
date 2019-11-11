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
                string Category = "";
                switch (Database.ObjectParameters[i].CategoryID)
                {
                    case 0:
                        Category = "Area";
                        break;
                    case 1:
                        Category = "Checkpoint";
                        break;
                    case 2:
                        Category = "Demo";
                        break;
                    case 3:
                        Category = "Goal";
                        break;
                    case 4:
                        Category = "Object";
                        break;
                    case 5:
                        Category = "Player";
                        break;
                    case 6:
                        Category = "Sky";
                        break;
                    case 7:
                        Category = "Zone";
                        break;
                    default:
                        Category = "Uncategorized";
                        break;
                }
                ListViewItem LVI = new ListViewItem(new string[] { Category, Database.ObjectParameters[i].ClassName });
                ObjectSelectListView.Items.Add(LVI);
            }
        }
    }
}

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
            
        }
    }
}

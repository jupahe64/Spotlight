using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;
using BYAML;
using System.IO;

namespace SpotLight
{
    public partial class LevelParameterForm : Form
    {
        public LevelParameterForm()
        {
            InitializeComponent();
            CenterToParent();
            StageList = new StageList(Program.GamePath + "\\SystemData\\StageList.szs");
        }

        public StageList StageList;

    }
}
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
            SarcData Data = SARC.UnpackRamN(YAZ0.Decompress(Program.GamePath + "\\SystemData\\StageList.szs"));
            if (Data.Files.ContainsKey("StageList.byml"))
            {
                BymlFileData x = ByamlFile.LoadN(new MemoryStream(Data.Files["StageList.byml"]),false,Syroot.BinaryData.Endian.Big);
                
            }
            else
            {
                MessageBox.Show("Failed to find \"StageList.byml\"", "SARC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        
    }
}
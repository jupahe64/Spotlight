using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BYAML;
using Syroot.BinaryData;
using SZS;

namespace Spotlight
{
    public class LevelParam
    {
        /// <summary>
        /// The ID of the course for the whole game
        /// </summary>
        public int CourseID { get; set; } = 0;
        /// <summary>
        /// The ID of the course for the world that it's located in
        /// </summary>
        public int StageID { get; set; } = 0;
        /// <summary>
        /// Internal name of the stage
        /// </summary>
        public string StageName { get; set; } = "";
        /// <summary>
        /// Unknown
        /// </summary>
        public string StageType { get; set; } = "";
        /// <summary>
        /// The availible time to complete the level
        /// </summary>
        public int Timer { get; set; } = 500;
        /// <summary>
        /// Number of Green stars in this level
        /// </summary>
        public int GreenStarNum { get; set; } = 0;
        /// <summary>
        /// Number of Green Stars required to unlock this level
        /// </summary>
        public int GreenStarLock { get; set; } = 0;
        /// <summary>
        /// The amount of Clones you can have in the level
        /// </summary>
        public int DoubleMario { get; set; } = 0;
        /// <summary>
        /// Unknown - has to do with Mii Ghosts
        /// </summary>
        public int GhostID { get; set; } = -1;
        /// <summary>
        /// Unknown - Has to do with Mii Ghosts
        /// </summary>
        public int GhostBaseTime { get; set; } = 0;
        /// <summary>
        /// Stamp count. Can only be 1 or 0.
        /// </summary>
        public int IllustItemNum { get; set; } = 0;

        public LevelParam()
        {

        }
        public LevelParam(dynamic LevelInfo)
        {
            CourseID = LevelInfo["CourseId"];
            StageID = LevelInfo["StageId"];
            StageName = LevelInfo["StageName"];
            StageType = LevelInfo["StageType"];
            Timer = LevelInfo["StageTimer"];
            GreenStarNum = LevelInfo["GreenStarNum"];
            GreenStarLock = LevelInfo["GreenStarLock"];
            DoubleMario = LevelInfo["DoubleMarioNum"];
            GhostID = LevelInfo["GhostId"];
            GhostBaseTime = LevelInfo["GhostBaseTime"];
            IllustItemNum = LevelInfo["IllustItemNum"];
        }

        public override string ToString() => $"({StageID}) {StageName} - {Timer} {(GreenStarNum > 0 ? $"[{GreenStarNum} Green Star{(GreenStarNum > 1 ? "s" : "")}] " : "")}{(IllustItemNum > 0 ? "[Stamp] " : "")}{(GreenStarLock > 0 ? $"[Green Star Gate ({GreenStarLock})] " : "")}";

        public dynamic ToByaml() => new Dictionary<string, object>
            {
                { "CourseId", CourseID },
                { "DoubleMarioNum", DoubleMario },
                { "GhostBaseTime", GhostBaseTime },
                { "GhostId", GhostID },
                { "GreenStarLock", GreenStarLock },
                { "GreenStarNum", GreenStarNum },
                { "IllustItemNum", IllustItemNum },
                { "StageId", StageID },
                { "StageName", StageName },
                { "StageTimer", Timer },
                { "StageType", StageType }
            };
    }

    public class World
    {
        public int WorldID { get; set; }

        public IReadOnlyList<LevelParam> Levels => levels;

        private List<LevelParam> levels = new List<LevelParam>();

        public World(dynamic WorldInfo)
        {
            WorldID = WorldInfo["WorldId"];
            List<dynamic> temp = WorldInfo["StageList"];

            foreach (dynamic levelEntry in temp.OrderBy(O=>O["StageId"]))
                levels.Add(new LevelParam(levelEntry));
        }

        public int Add(LevelParam levelParam)
        {
            int index = -1;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levelParam.StageID >= levels[i].StageID)
                    index = i;
            }

            levels.Insert(index + 1, levelParam);

            return index + 1;
        }

        public int Add(string stageName, int courseId)
        {
            int index;
            int stageId;

            if (levels.Count == 0 || levels[0].StageID>1)
            {
                index = 0;
                stageId = 1;
            }
            else
            {
                index = levels.Count;
                stageId = levels.Count+1;

                for (int i = 1; i < levels.Count; i++)
                {
                    if (levels[i].StageID - levels[i-1].StageID > 1)
                    {
                        index = i;
                        stageId = i + 1;
                        break;
                    }
                }
            }
            levels.Insert(index, new LevelParam()
            {
                StageID = stageId,
                CourseID = courseId,
                StageName = stageName
            });

            return index;
        }

        public int UpdateLevelIndex(LevelParam levelParam)
        {
            levels.Remove(levelParam);
            return Add(levelParam);
        }

        public int IndexOf(LevelParam levelParam) => levels.IndexOf(levelParam);

        public void Remove(LevelParam levelParam)
        {
            levels.Remove(levelParam);
        }

        public void RemoveAt(int index)
        {
            levels.RemoveAt(index);
        }

        public override string ToString() => $"World {WorldID}";

        public dynamic ToByaml()
        {
            Dictionary<string, object> Final = new Dictionary<string, object>() { {"WorldId", WorldID } };
            List<dynamic> entries = new List<dynamic>();
            for (int i = 0; i < levels.Count; i++)
                entries.Add(levels[i].ToByaml());
            Final.Add("StageList",entries);
            return Final;
        }
    }

    /// <summary>
    /// Container for the StageList BYAML file
    /// </summary>
    public class StageList
    {
        public string Filename { get; set; }

        public ByteOrder ByteOrder { get; set; }

        /// <summary>
        /// A List of all the worlds in SM3DW
        /// </summary>
        public List<World> Worlds;

        public static bool TryOpen(string filename, out StageList stageList)
        {
            stageList = null;

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(filename));
            
            BymlFileData byml;
            if (sarc.Files.ContainsKey("StageList.byml"))
                byml = ByamlFile.LoadN(new MemoryStream(sarc.Files["StageList.byml"]), true, ByteOrder.BigEndian);
            else
                throw new Exception("Failed to find the StageList");


            if (!byml.RootNode.TryGetValue("WorldList", out dynamic worldList))
                return false;


            List<World> worlds = new List<World>();

            for (int i = 0; i < worldList.Count; i++)
                worlds.Add(new World(worldList[i]));

            stageList = new StageList(filename, worlds, byml.byteOrder);

            return true;
        }

        private StageList(string filename, List<World> worlds, ByteOrder byteOrder)
        {

            Worlds = worlds;
            Filename = filename;
            ByteOrder = byteOrder;
        }

        public int GetNextUniqueCourseID()
        {
            int max = 0;
            for (int i = 0; i < Worlds.Count; i++)
            {
                for (int j = 0; j < Worlds[i].Levels.Count; j++)
                {
                    max = Math.Max(max, Worlds[i].Levels[j].CourseID);
                }
            }

            return max + 1;
        }

        public void Save()
        {
            //byte[] tmp = ToByaml();
            //File.WriteAllBytes("Test.byml", tmp);
            BymlFileData Output = new BymlFileData() { Version = 1, SupportPaths = false, byteOrder = ByteOrder };

            Dictionary<string, dynamic> FinalRoot = new Dictionary<string, dynamic>();
            List<dynamic> worlds = new List<dynamic>();

            for (int i = 0; i < Worlds.Count; i++)
                worlds.Add(Worlds[i].ToByaml());

            FinalRoot.Add("WorldList", worlds);
            Output.RootNode = FinalRoot;



            SarcData Data = new SarcData() { byteOrder = ByteOrder, Files = new Dictionary<string, byte[]>() };
            Data.Files.Add("StageList.byml", ByamlFile.SaveN(Output));
            Tuple<int, byte[]> x = SARC.PackN(Data);


            if (Filename.StartsWith(Program.GamePath) && !string.IsNullOrEmpty(Program.ProjectPath))
            {
                switch (MessageBox.Show(
                    Program.CurrentLanguage.GetTranslation("SaveStageListInProjectText") ?? "Would you like to save the StageList.szs to your ProjectPath instead of your BaseGame?",
                    Program.CurrentLanguage.GetTranslation("SaveStageListInProjectHeader") ?? "Save in ProjectPath",
                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        Directory.CreateDirectory(Path.Combine(Program.ProjectPath, "SystemData"));
                        Filename = Path.Combine(Program.ProjectPath, "SystemData", "StageList.szs");
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            File.WriteAllBytes(Filename, YAZ0.Compress(x));
            //File.WriteAllBytes("Broken.byml", Data.Files["StageList.byml"]);
        }

        private byte[] ToByaml()
        {
            List<byte> ReturningBytes = new List<byte>() { 0x42, 0x59, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0xCC, 0xCC, 0xCC, 0xCC, 0xDD, 0xDD, 0xDD, 0xDD };

            ReturningBytes.AddRange(WriteStringSection(ReturningBytes.Count, new List<string>() { "CourseId","DoubleMarioNum","GhostBaseTime","GhostId","GreenStarLock","GreenStarNum","IllustItemNum","StageId","StageList","StageName","StageTimer","StageType","WorldId","WorldList" }));
            byte[] Write = BitConverter.GetBytes(ReturningBytes.Count);
            ReturningBytes[8] = Write[3];
            ReturningBytes[9] = Write[2];
            ReturningBytes[10] = Write[1];
            ReturningBytes[11] = Write[0];
            List<string> ValueStrings = GetValueStrings();
            ReturningBytes.AddRange(WriteStringSection(0x10,ValueStrings));
            Write = BitConverter.GetBytes(ReturningBytes.Count);
            ReturningBytes[12] = Write[3];
            ReturningBytes[13] = Write[2];
            ReturningBytes[14] = Write[1];
            ReturningBytes[15] = Write[0];

            int Offset = ReturningBytes.Count;
            //Write the root dictionary node
            List<byte[]> Nodes = new List<byte[]>
            {
                WriteDictionaryNode(1, new List<int>() { 0x0D }, new List<byte>() { 0xC0 })
            };
            Offset += Nodes[0].Length;
            byte[] Offsetarray = BitConverter.GetBytes(Offset);
            Nodes[0][8] = Offsetarray[3];
            Nodes[0][9] = Offsetarray[2];
            Nodes[0][10] = Offsetarray[1];
            Nodes[0][11] = Offsetarray[0];
            List<byte> arraynodes = new List<byte>();
            for (int i = 0; i < Worlds.Count; i++)
                arraynodes.Add(0xC1);
            Nodes.Add(WriteArrayNode(arraynodes));
            Offset += Nodes[1].Length;
            for (int i = 0; i < Worlds.Count; i++)
            {
                Offsetarray = BitConverter.GetBytes(Offset);
                int Padding = 0;
                while (((i * 4) + 4 + Worlds.Count + 0+Padding) % 4 != 0)
                    Padding++;
                Nodes[1][(i * 4) + 4 + Worlds.Count + 0 + Padding] = Offsetarray[3];
                Nodes[1][(i * 4) + 4 + Worlds.Count + 1 + Padding] = Offsetarray[2];
                Nodes[1][(i * 4) + 4 + Worlds.Count + 2 + Padding] = Offsetarray[1];
                Nodes[1][(i * 4) + 4 + Worlds.Count + 3 + Padding] = Offsetarray[0];

                arraynodes = new List<byte>();
                Nodes.Add(WriteDictionaryNode(2,new List<int>() { 8, 12 },new List<byte>() { 0xC0, 0xD1 }, new List<object>() { null, Worlds[i].WorldID }));
                Offset += Nodes[Nodes.Count-1].Length;
                Offsetarray = BitConverter.GetBytes(Offset);
                Nodes[Nodes.Count - 1][8] = Offsetarray[3];
                Nodes[Nodes.Count - 1][9] = Offsetarray[2];
                Nodes[Nodes.Count - 1][10] = Offsetarray[1];
                Nodes[Nodes.Count - 1][11] = Offsetarray[0];
                for (int j = 0; j < Worlds[i].Levels.Count; j++)
                    arraynodes.Add(0xC1);
                Nodes.Add(WriteArrayNode(arraynodes));
                Offset += Nodes[Nodes.Count - 1].Length;
                int NodeId = Nodes.Count - 1;
                for (int x = 0; x < Worlds[i].Levels.Count; x++)
                {
                    Offsetarray = BitConverter.GetBytes(Offset);
                    Padding = 0;
                    while (((x * 4) + 4 + Worlds[i].Levels.Count + 0+Padding) % 4 != 0)
                        Padding++;
                    Nodes[NodeId][(x * 4) + 4 + Worlds[i].Levels.Count + 0 + Padding] = Offsetarray[3];
                    Nodes[NodeId][(x * 4) + 4 + Worlds[i].Levels.Count + 1 + Padding] = Offsetarray[2];
                    Nodes[NodeId][(x * 4) + 4 + Worlds[i].Levels.Count + 2 + Padding] = Offsetarray[1];
                    Nodes[NodeId][(x * 4) + 4 + Worlds[i].Levels.Count + 3 + Padding] = Offsetarray[0];

                    int NameID = 0;
                    int TypeID = 0;
                    for (int j = 0; j < ValueStrings.Count; j++)
                    {
                        if (ValueStrings[j] == Worlds[i].Levels[x].StageName)
                            NameID = j;
                        if (ValueStrings[j] == Worlds[i].Levels[x].StageType)
                            TypeID = j;
                    }
                    Nodes.Add(WriteDictionaryNode(0x0B,new List<int>() { 0,1,2,3,4,5,6,7,9,10,11 }, new List<byte>() { 0xD1, 0xD1, 0xD1, 0xD1, 0xD1, 0xD1, 0xD1, 0xD1, 0xA0, 0xD1, 0xA0 }, new List<object>() { Worlds[i].Levels[x].CourseID, Worlds[i].Levels[x].DoubleMario, Worlds[i].Levels[x].GhostBaseTime, Worlds[i].Levels[x].GhostID, Worlds[i].Levels[x].GreenStarLock, Worlds[i].Levels[x].GreenStarNum, Worlds[i].Levels[x].IllustItemNum, Worlds[i].Levels[x].StageID, NameID, Worlds[i].Levels[x].Timer, TypeID }));
                    Offset += Nodes[Nodes.Count - 1].Length;
                }
            }

            for (int i = 0; i < Nodes.Count; i++)
                ReturningBytes.AddRange(Nodes[i]);

            return ReturningBytes.ToArray();
        }

        private byte[] WriteStringSection(int Offset, List<string> Strings)
        {
            List<byte> ReturningBytes = new List<byte>() { 0xC2 };
            byte[] Write = BitConverter.GetBytes(Strings.Count);
            ReturningBytes.Add(Write[2]);
            ReturningBytes.Add(Write[1]);
            ReturningBytes.Add(Write[0]);
            Offset += 4;
            int length = (4 * Strings.Count)+4;
            Offset += length;
            for (int i = 0; i < Strings.Count; i++)
            {
                Write = BitConverter.GetBytes(Offset-0x10);
                Array.Reverse(Write);
                ReturningBytes.AddRange(Write);
                Offset += Encoding.GetEncoding(932).GetBytes(Strings[i]).Length+1;
            }
            Write = BitConverter.GetBytes(Offset - 0x10);
            Array.Reverse(Write);
            ReturningBytes.AddRange(Write);
            for (int i = 0; i < Strings.Count; i++)
            {
                ReturningBytes.AddRange(Encoding.GetEncoding(932).GetBytes(Strings[i]));
                ReturningBytes.Add(0x00);
            }
            while (ReturningBytes.Count % 4 != 0)
                ReturningBytes.Add(0x00);

            return ReturningBytes.ToArray();
        }

        private List<string> GetValueStrings()
        {
            List<string> result = new List<string>();
            List<string> Japanese = new List<string>();
            for (int i = 0; i < Worlds.Count; i++)
            {
                for (int j = 0; j < Worlds[i].Levels.Count; j++)
                {
                    if (!result.Any(O => O == Worlds[i].Levels[j].StageName))
                        result.Add(Worlds[i].Levels[j].StageName);
                    if (!Japanese.Any(O => O == Worlds[i].Levels[j].StageType))
                        Japanese.Add(Worlds[i].Levels[j].StageType);
                }
            }
            result.Sort();
            string item = "";
            for (int i = 0; i < Japanese.Count; i++)
            {
                if (Japanese[i] == "DRC専用")
                {
                    item = Japanese[i];
                    Japanese.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i][0] == 'D' || result[i][0] == 'd')
                {
                    result.Insert(i, item);
                    break;
                }
            }

            Japanese.Sort();
            string[] x = new string[]
            {
                "カジノ部屋",
                "キノピオの家",
                "キノピオ探検隊",
                "クッパ城",
                "クッパ城[砦]",
                "クッパ城[戦車]",
                "クッパ城[列車]",
                "クッパ城[列車通常]",
                "ゲートキーパー",
                "ゲートキーパー[GPあり]",
                "ゴールデンエクスプレス",
                "チャンピオンシップ",
                "ミステリーハウス",
                "隠しキノピオの家",
                "隠し土管",
                "通常",
                "妖精の家"
            };
            result.AddRange(x);
            return result;
        }

        private byte[] WriteDictionaryNode(int DataCount, List<int> NodeNameID, List<byte> NextNodeType, List<object> Data = null)
        {
            List<byte> ReturnList = new List<byte>() { 0xC1 };
            byte[] Write = BitConverter.GetBytes(DataCount);
            ReturnList.Add(Write[2]);
            ReturnList.Add(Write[1]);
            ReturnList.Add(Write[0]);
            for (int i = 0; i < DataCount; i++)
            {
                Write = BitConverter.GetBytes(NodeNameID[i]);
                ReturnList.Add(Write[2]);
                ReturnList.Add(Write[1]);
                ReturnList.Add(Write[0]);
                ReturnList.Add(NextNodeType[i]);
                if (NextNodeType[i] == 0xD1 || NextNodeType[i] == 0xA0)
                {
                    Write = BitConverter.GetBytes(Convert.ToInt32(Data[i]));
                    Array.Reverse(Write);
                    ReturnList.AddRange(Write);
                }
                else
                {
                    ReturnList.AddRange(new byte[4] { 0xAA, 0xAA, 0xAA, 0xAA });
                }
            }


            
            return ReturnList.ToArray();
        }

        private byte[] WriteArrayNode(List<byte> NodeTypes)
        {
            List<byte> ReturnList = new List<byte>() { 0xC0 };
            byte[] Write = BitConverter.GetBytes(NodeTypes.Count);
            ReturnList.Add(Write[2]);
            ReturnList.Add(Write[1]);
            ReturnList.Add(Write[0]);
            ReturnList.AddRange(NodeTypes);
            while (ReturnList.Count % 4 != 0)
                ReturnList.Add(0x00);
            for (int i = 0; i < NodeTypes.Count; i++)
                ReturnList.AddRange(new byte[4] { 0xDD, 0xDD, 0xDD, 0xDD });


            return ReturnList.ToArray();
        }

        public override string ToString()
        {
            int x = 0;
            for (int i = 0; i < Worlds.Count; i++)
                x += Worlds[i].Levels.Count;
            
            return $"{Worlds.Count} Worlds, {x} Levels";
        }
    }
}

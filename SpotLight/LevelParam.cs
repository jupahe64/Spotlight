using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYAML;
using SZS;

namespace SpotLight
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

        public override string ToString()
        {
            return $"({StageID}) {StageName} - {Timer} {(GreenStarNum > 0 ? $"[{GreenStarNum} Green Star{(GreenStarNum > 1 ? "s":"")}] ":"")}{(IllustItemNum > 0 ? "[Stamp] " : "")}{(GreenStarLock > 0 ? $"[Green Star Gate ({GreenStarLock})] " : "")}";
        }
    }

    public class World
    {
        public int WorldID { get; set; }
        public List<LevelParam> Levels = new List<LevelParam>();

        public World(dynamic WorldInfo)
        {
            WorldID = WorldInfo["WorldId"];
            List<dynamic> temp = WorldInfo["StageList"];
            for (int i = 0; i < temp.Count; i++)
            {
                Levels.Add(new LevelParam(temp[i]));
            }
        }

        public override string ToString()
        {
            return $"World {WorldID}";
        }
    }

    /// <summary>
    /// Container for the StageList BYAML file
    /// </summary>
    public class StageList
    {
        public string Filename { get; set; }
        /// <summary>
        /// A List of all the worlds in SM3DW
        /// </summary>
        public List<World> Worlds = new List<World>();

        public StageList(string input)
        {
            Filename = input;

            BymlFileData Input;
            SarcData Data = SARC.UnpackRamN(YAZ0.Decompress(input));
            if (Data.Files.ContainsKey("StageList.byml"))
                Input = ByamlFile.LoadN(new MemoryStream(Data.Files["StageList.byml"]), false, Syroot.BinaryData.Endian.Big);
            else
                throw new Exception("Failed to find the StageList");

            List<dynamic> temp = Input.RootNode["WorldList"];

            for (int i = 0; i < temp.Count; i++)
            {
                Worlds.Add(new World(temp[i]));
            }
        }

        public void Save()
        {

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

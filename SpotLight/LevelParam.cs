using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class World
    {
        public int WorldID { get; set; }
        public List<LevelParam> LevelParameters = new List<LevelParam>();
    }
}

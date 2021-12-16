using System.Collections.Generic;

namespace Spotlight.Level
{
    public enum StageArcType
    {
        NotSpecified,
        Split,
        Combined
    }

    public struct StageInfo
    {
        public StageInfo(string directory, string stageName)
        {
            StageName = stageName;
            Directory = directory;
            StageArcType = StageArcType.NotSpecified;
        }

        public StageInfo(string directory, string stageName, StageArcType stageArcType)
        {
            StageName = stageName;
            Directory = directory;
            StageArcType = stageArcType;
        }

        public string StageName { get; set; }
        public string Directory { get; set; }
        public StageArcType StageArcType { get; set; }

        public override bool Equals(object obj)
        {
            return obj is StageInfo info &&
                   StageName == info.StageName &&
                   Directory == info.Directory &&
                   StageArcType == info.StageArcType;
        }

        public override int GetHashCode()
        {
            int hashCode = 1030067254;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StageName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Directory);
            hashCode = hashCode * -1521134295 + StageArcType.GetHashCode();
            return hashCode;
        }
    }
}

using OpenTK;
using Spotlight.EditorDrawables;
using System.Collections.Generic;

namespace Spotlight
{
    public static class SceneDrawState
    {
        public static ZoneTransform ZoneTransform = ZoneTransform.Identity;

        public static Vector4? HighlightColorOverride = null;

        public static HashSet<string> EnabledLayers = null;
    }

    public static class SceneObjectIterState
    {
        public static bool InLinks = false;
    }
}

using OpenTK;
using Spotlight.EditorDrawables;

namespace Spotlight
{
    public static class SceneDrawState
    {
        public static ZoneTransform ZoneTransform = ZoneTransform.Identity;

        public static Vector4? HighlightColorOverride = null;
    }

    public static class SceneObjectIterState
    {
        public static bool InLinks = false;
    }
}

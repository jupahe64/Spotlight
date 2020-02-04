using OpenTK;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotLight
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

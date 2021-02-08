using Spotlight.Level;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Spotlight
{
    public static class GlobalUndoRedo
    {
        public static bool Enabled { get; set; }

        enum ActionType
        {
            NONE,
            UNDO,
            REDO,
            SUBMIT
        }

        static ActionType lastAction = ActionType.NONE;

        public static void Clear()
        {
            lastAction = ActionType.NONE;
            index = 0;
            currentZoneHistory.Clear();
        }

        public static bool Undo(out SM3DWorldZone zoneToFocus)
        {
            if (index != 0)
            {
                if (lastAction == ActionType.UNDO)
                    index--;

                zoneToFocus = currentZoneHistory[index];
                lastAction = ActionType.UNDO;
                return true;
            }
            else
            {
                zoneToFocus = null;
                return false;
            }
        }

        public static bool Redo(out SM3DWorldZone zoneToFocus)
        {
            if (index != currentZoneHistory.Count-1)
            {
                if (lastAction == ActionType.REDO)
                    index++;

                zoneToFocus = currentZoneHistory[index];
                lastAction = ActionType.REDO;
                return true;
            }
            else
            {
                zoneToFocus = null;
                return false;
            }
        }

        public static void Submit(SM3DWorldZone currentZone)
        {
            currentZoneHistory.Add(currentZone);
            index++;
            lastAction = ActionType.SUBMIT;
        }

        static int index = 0;

        readonly static List<SM3DWorldZone> currentZoneHistory = new List<SM3DWorldZone>();
    }
}

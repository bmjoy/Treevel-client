using Project.Scripts.Utils.Definitions;
using System;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    /// <summary>
    /// PlayerPrefs で使うキー群
    /// </summary>
    public static class PlayerPrefsKeys
    {
        public const string TREE_RELEASED = "TREE_RELEASED";
        public const string TREE_CLEARED = "TREE_CLEARED";
        public const string ROAD = "ROAD";
        public const string BGM_VOLUME = "BGM_VOLUME";
        public const string SE_VOLUME = "SE_VOLUME";
        public const string LANGUAGE = "LANGUAGE";
        public const string STAGE_DETAILS = "STAGE_DETAILS";
        public const string LEVEL_SELECT_CANVAS_SCALE = "LEVEL_SELECT_CANVAS_SCALE";
        public const string LEVEL_SELECT_SCROLL_POSITION = "LEVEL_SELECT_SCROLL_POSITION";
        public const char KEY_CONNECT_CHAR = '-';
    }
}

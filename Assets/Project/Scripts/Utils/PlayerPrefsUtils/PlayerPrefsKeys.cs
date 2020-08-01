using Project.Scripts.Utils.Definitions;
using System;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    /// <summary>
    /// PlayerPrefs で使うキー群
    /// </summary>
    public static class PlayerPrefsKeys
    {
        public const string TREE = "TREE";
        public const string BRANCH_STATE = "BranchState";
        public const string BGM_VOLUME = "BGM_VOLUME";
        public const string SE_VOLUME = "SE_VOLUME";
        public const string LANGUAGE = "LANGUAGE";
        public const string STAGE_DETAILS = "STAGE_DETAILS";
        public const string LEVEL_SELECT_CANVAS_SCALE = "LEVEL_SELECT_CANVAS_SCALE";
        public const string LEVEL_SELECT_SCROLL_POSITION = "LEVEL_SELECT_SCROLL_POSITION";
        public const char KEY_CONNECT_CHAR = '-';

        /// <summary>
        /// 起動日数
        /// </summary>
        public const string STARTUP_DAYS = "STARTUP_DAYS";

        /// <summary>
        /// 最終起動日
        /// </summary>
        public const string LAST_STARTUP_DATE = "LAST_STARTUP_DATE";
    }
}

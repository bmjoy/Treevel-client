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

        /// <summary>
        /// ステージのkeyを生成する
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        /// <returns> StageKey(= treeId_stageNumber) </returns>
        public static string EncodeStageIdKey(ETreeId treeId, int stageNumber)
        {
            return $"{treeId}{KEY_CONNECT_CHAR}{stageNumber}";
        }

        /// <summary>
        /// ステージのkeyからステージ情報を返す
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns> (treeId, stageNumber) </returns>
        public static(ETreeId, int) DecodeStageIdKey(string stageId)
        {
            var retValues = stageId.Split(KEY_CONNECT_CHAR);
            if (retValues.Length != 2) throw new Exception("Wrong key format");
            try {
                var treeId = (ETreeId) Enum.ToObject(typeof(ETreeId), retValues[0]);
                var stageNumber = int.Parse(retValues[1]);
                return (treeId, stageNumber);
            } catch (Exception e) {
                throw e;
            }
        }
    }
}

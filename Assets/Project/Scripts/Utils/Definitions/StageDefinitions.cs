using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// ステージの難易度一覧
    /// </summary>
    public enum EStageLevel {
        Easy = 0,
        Normal,
        Hard,
        VeryHard
    }

    public static class StageInfo
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        public static readonly Dictionary<EStageLevel, int> Num = new Dictionary<EStageLevel, int>()
        {
            {EStageLevel.Easy, 10},
            {EStageLevel.Normal, 10},
            {EStageLevel.Hard, 10},
            {EStageLevel.VeryHard, 10}
        };

        /// <summary>
        /// ステージ id の開始値
        /// </summary>
        public static readonly Dictionary<EStageLevel, int> StageStartId = new Dictionary<EStageLevel, int>()
        {
            {EStageLevel.Easy, 1},
            {EStageLevel.Normal, 1001},
            {EStageLevel.Hard, 2001},
            {EStageLevel.VeryHard, 3001}
        };

        /// <summary>
        /// 難易度ごとに固有のボタン色
        /// </summary>
        /// TODO: 今後，インスペクタから変更できるようにする
        public static readonly Dictionary<EStageLevel, Color> LevelColor = new Dictionary<EStageLevel, Color>()
        {
            {EStageLevel.Easy, Color.magenta},
            {EStageLevel.Normal, Color.green},
            {EStageLevel.Hard, Color.yellow},
            {EStageLevel.VeryHard, Color.cyan}
        };
    }

    public static class StageSize
    {
        /// <summary>
        /// ステージのタイル行数
        /// </summary>
        public const int ROW = 5;

        /// <summary>
        /// ステージのタイル列数
        /// </summary>
        public const int COLUMN = 3;

        /// <summary>
        /// タイルの合計数
        /// </summary>
        public const int TILE_NUM = ROW * COLUMN;

        /// <summary>
        /// ナンバーパネルの数
        /// </summary>
        public const int NUMBER_PANEL_NUM = 8;
    }
}

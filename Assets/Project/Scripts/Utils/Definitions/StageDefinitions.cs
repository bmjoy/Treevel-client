using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    public enum EStageLevel {
        Easy = 0,
        Normal,
        Hard,
        VeryHard
    }

    public static class StageInfo
    {
        public static readonly Dictionary<EStageLevel, int> Num = new Dictionary<EStageLevel, int>()
        {
            {EStageLevel.Easy, 10},
            {EStageLevel.Normal, 10},
            {EStageLevel.Hard, 10},
            {EStageLevel.VeryHard, 10}
        };

        public static readonly Dictionary<EStageLevel, int> StageStartId = new Dictionary<EStageLevel, int>()
        {
            {EStageLevel.Easy, 1},
            {EStageLevel.Normal, 1001},
            {EStageLevel.Hard, 2001},
            {EStageLevel.VeryHard, 3001}
        };

        /// <summary>
        /// 難易度ごとに固有の難易度名
        /// </summary>
        public static readonly Dictionary<EStageLevel, string> LevelName = new Dictionary<EStageLevel, string>()
        {
            {EStageLevel.Easy, "簡単"},
            {EStageLevel.Normal, "普通"},
            {EStageLevel.Hard, "ムズイ"},
            {EStageLevel.VeryHard, "激ムズ"}
        };

        /// <summary>
        /// 難易度ごとに固有のボタン色
        /// </summary>
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
        public const int ROW = 5;
        public const int COLUMN = 3;
        public const int TILE_NUM = ROW * COLUMN;
        public const int NUMBER_PANEL_NUM = 8;
    }
}

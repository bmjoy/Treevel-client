using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// ステージの難易度一覧
    /// </summary>
    public enum ELevelName {
        Easy = 0,
        Normal,
        Hard,
        VeryHard
    }

    public static class LevelInfo
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        public static readonly Dictionary<ELevelName, int> Num = new Dictionary<ELevelName, int>()
        {
            {ELevelName.Easy, 10},
            {ELevelName.Normal, 10},
            {ELevelName.Hard, 10},
            {ELevelName.VeryHard, 10}
        };

        /// <summary>
        /// ステージ id の開始値
        /// </summary>
        public static readonly Dictionary<ELevelName, int> StageStartId = new Dictionary<ELevelName, int>()
        {
            {ELevelName.Easy, 1},
            {ELevelName.Normal, 1001},
            {ELevelName.Hard, 2001},
            {ELevelName.VeryHard, 3001}
        };

        /// <summary>
        /// 難易度ごとに固有のボタン色
        /// </summary>
        /// TODO: 今後，インスペクタから変更できるようにする
        public static readonly Dictionary<ELevelName, Color> LevelColor = new Dictionary<ELevelName, Color>()
        {
            {ELevelName.Easy, Color.magenta},
            {ELevelName.Normal, Color.green},
            {ELevelName.Hard, Color.yellow},
            {ELevelName.VeryHard, Color.cyan}
        };
    }
}

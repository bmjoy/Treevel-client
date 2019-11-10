﻿using System.Collections.Generic;
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
        /// 各レベルの木の種類数
        /// </summary>
        public static readonly Dictionary<ELevelName, int> TreeNum = new Dictionary<ELevelName, int>()
        {
            {ELevelName.Easy, 2},
            {ELevelName.Normal, 1},
            {ELevelName.Hard, 1},
            {ELevelName.VeryHard, 1}
        };

        /// <summary>
        /// ステージ数
        /// </summary>
        /// TODO: StageSelectSceneで木に応じたステージ数を表示するときは使っていない
        ///       RecordSceneで使っているので残しているが、将来的にはTreeDefinition.TreeInfo.Numを使う
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
        /// TODO: 今後、StageStartIdから一意にステージを選択するのではなく、
        ///       (LevelId, TreeId, StageId)の組で一意にステージを選択する
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

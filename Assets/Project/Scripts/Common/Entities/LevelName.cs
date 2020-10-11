﻿using System;
using System.Collections.Generic;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Entities
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
        /// TODO: 実際にある各レベルの木の数で決める
        public static readonly Dictionary<ELevelName, int> TREE_NUM = new Dictionary<ELevelName, int>()
        {
            {ELevelName.Easy, 3},
            {ELevelName.Normal, 3},
            {ELevelName.Hard, 3},
            {ELevelName.VeryHard, 3}
        };

        /// <summary>
        /// ステージ数
        /// </summary>
        /// TODO: StageSelectSceneで木に応じたステージ数を表示するときは使っていない
        /// RecordSceneで使っているので残しているが、将来的にはTreeDefinition.TreeInfo.Numを使う
        public static readonly Dictionary<ELevelName, int> NUM = new Dictionary<ELevelName, int>()
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
        public static readonly Dictionary<ELevelName, int> STAGE_START_ID = new Dictionary<ELevelName, int>()
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
        public static readonly Dictionary<ELevelName, Color> LEVEL_COLOR = new Dictionary<ELevelName, Color>()
        {
            {ELevelName.Easy, new Color(1.0f, 0.65f, 0.78f)},
            {ELevelName.Normal, new Color(0.64f, 0.91f, 0.38f)},
            {ELevelName.Hard, new Color(0.93f, 0.38f, 0.10f)},
            {ELevelName.VeryHard, new Color(0.67f, 0.76f, 0.76f)}
        };

        /// <summary>
        /// レベルに応じたStageSelectSceneをロードする
        /// </summary>
        /// <param name="sceneName"> 木のレベル </param>
        public static void LoadStageSelectScene(ELevelName levelName)
        {
            switch (levelName) {
                case ELevelName.Easy:
                    AddressableAssetManager.LoadScene(Constants.SceneName.SPRING_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Normal:
                    AddressableAssetManager.LoadScene(Constants.SceneName.SUMMER_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Hard:
                    AddressableAssetManager.LoadScene(Constants.SceneName.AUTUMN_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.VeryHard:
                    AddressableAssetManager.LoadScene(Constants.SceneName.WINTER_STAGE_SELECT_SCENE);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Managers;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    public class LevelSelectDirector : SingletonObjectBase<LevelSelectDirector>
    {
        /// <summary>
        /// 木
        /// </summary>
        private List<LevelTreeController> _trees;

        /// <summary>
        /// 道
        /// </summary>
        private List<RoadController> _roads;

        /// <summary>
        /// アプリ閉じているかどうか（#837の回避策）
        /// </summary>
        private bool _isQuitApplication;

        /// <summary>
        /// 解放演出再生完了の道
        /// </summary>
        public List<string> releaseAnimationPlayedRoads;

        /// <summary>
        /// 解放演出再生完了の木
        /// </summary>
        public List<ETreeId> releaseAnimationPlayedTrees;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(Constants.TagName.TREE).Select(tree => tree.GetComponent<LevelTreeController>()).ToList();
            _roads = GameObject.FindGameObjectsWithTag(Constants.TagName.ROAD).Select(road => road.GetComponent<RoadController>()).ToList();
            ResetController.DataReset.Subscribe(_ => {
                // 木と道の状態のリセット
                releaseAnimationPlayedRoads.Clear();
                releaseAnimationPlayedTrees.Clear();
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.TREE_ANIMATION_STATE);
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.ROAD_ANIMATION_STATE);
            }).AddTo(this);

            releaseAnimationPlayedRoads = PlayerPrefsUtility.GetList<string>(Constants.PlayerPrefsKeys.ROAD_ANIMATION_STATE);
            releaseAnimationPlayedTrees = PlayerPrefsUtility.GetList<ETreeId>(Constants.PlayerPrefsKeys.TREE_ANIMATION_STATE);
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == Constants.SceneName.MENU_SELECT_SCENE) {
                SoundManager.Instance.PlaySE(ESEKey.LevelSelect_River);
            }
            _trees.ForEach(tree => tree.UpdateState());
            _roads.ForEach(road => road.UpdateStateAsync().Forget());
        }

        private void OnApplicationQuit()
        {
            _isQuitApplication = true;
        }

        /// <summary>
        /// 木と道の状態の保存
        /// </summary>
        private void OnDisable()
        {
            if (!_isQuitApplication)
                SoundManager.Instance.StopSE(ESEKey.LevelSelect_River);
            PlayerPrefsUtility.SetList(Constants.PlayerPrefsKeys.TREE_ANIMATION_STATE, releaseAnimationPlayedTrees);
            PlayerPrefsUtility.SetList(Constants.PlayerPrefsKeys.ROAD_ANIMATION_STATE, releaseAnimationPlayedRoads);
        }

        /// <summary>
        /// 道の幅の変更
        /// </summary>
        /// <param name="scale"> 拡大率 </param>
        public void ScaleRoad(float scale)
        {
            _roads.ForEach(road => road.Scale.Value = scale);
        }
    }
}

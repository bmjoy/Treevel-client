using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Treevel.Common.Managers;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Playables;
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

        /// <summary>
        /// 木の解放演出のディレクター
        /// </summary>
        public PlayableDirector UnlockLevelDirector;

        /// <summary>
        /// Disableなどの時に実行中のタスクをキャンセル用
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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

            this.OnDisableAsObservable()
                .Subscribe(_ => {
                    _cancellationTokenSource.Cancel();
                }).AddTo(this);
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == Constants.SceneName.MENU_SELECT_SCENE) {
                SoundManager.Instance.PlaySE(ESEKey.LevelSelect_River);
            }
            // 木の状態を更新
            _trees.ForEach(tree => tree.UpdateState());
            // 道の状態を更新
            _roads.ForEach(road => road.UpdateState());

            // 解放できる道を計算し、演出を再生
            CheckReleaseNewLevelAsync().Forget();
        }

        private async UniTask CheckReleaseNewLevelAsync()
        {
            // 解放する道のリストを作成
            var waitForReleaseRoads = _roads.Where(road => road.CanBeReleased() && !releaseAnimationPlayedRoads.Contains(road.saveKey)).ToList();

            while (waitForReleaseRoads.Count > 0) {
                // 道を解放する演出を再生
                var releaseRoad = waitForReleaseRoads[0];
                UnlockLevelDirector.SetReferenceValue(Constants.TimelineReferenceKey.ROAD_TO_RELEASE, releaseRoad);
                UnlockLevelDirector.SetReferenceValue(Constants.TimelineReferenceKey.TREE_TO_RELEASE, releaseRoad.EndObjectController);
                UnlockLevelDirector.Play();

                // 演出終了まで待つ
                await UniTask.WaitUntil(() => UnlockLevelDirector.state != PlayState.Playing, cancellationToken: _cancellationTokenSource.Token);

                // 木の解放演出見たことを記録する
                releaseAnimationPlayedTrees.Add(releaseRoad.EndObjectController.treeId);

                // 再生状態を保存
                releaseAnimationPlayedRoads.Add(releaseRoad.saveKey);
                waitForReleaseRoads.RemoveAt(0);
            }
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
    }
}

using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Managers;
using Cysharp.Threading.Tasks;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UnityEngine;

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

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(Constants.TagName.TREE).Select(tree => tree.GetComponent<LevelTreeController>()).ToList();
            _roads = GameObject.FindGameObjectsWithTag(Constants.TagName.ROAD).Select(road => road.GetComponent<RoadController>()).ToList();
            ResetController.DataReset.Subscribe(_ => {
                // 木と道の状態のリセット
                _trees.ForEach(tree => tree.Reset());
                _roads.ForEach(road => road.Reset());
            }).AddTo(this);
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            SoundManager.Instance.PlaySE(ESEKey.LevelSelect_River);
            _trees.ForEach(tree => tree.UpdateStateAsync().Forget());
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
            _trees.ForEach(tree => tree.SaveState());
            _roads.ForEach(road => road.SaveState());
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

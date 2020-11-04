using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    public class LevelSelectDirector : SingletonObject<LevelSelectDirector>
    {
        /// <summary>
        /// 木
        /// </summary>
        private static List<LevelTreeController> _trees;

        /// <summary>
        /// 道
        /// </summary>
        private static List<RoadController> _roads;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(Constants.TagName.TREE).Select(tree => tree.GetComponent<LevelTreeController>()).ToList<LevelTreeController>();
            _roads = GameObject.FindGameObjectsWithTag(Constants.TagName.ROAD).Select(road => road.GetComponent<RoadController>()).ToList<RoadController>();
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            _trees.ForEach(tree => tree.UpdateState());
            _roads.ForEach(road => StartCoroutine(road.UpdateState()));
        }

        /// <summary>
        /// 木と道の状態の保存
        /// </summary>
        private void OnDisable()
        {
            _trees.ForEach(tree => tree.SaveState());
            _roads.ForEach(road => road.SaveState());
        }

        /// <summary>
        /// 木と道の状態のリセット
        /// </summary>
        public static void Reset()
        {
            _trees.ForEach(tree => tree.Reset());
            _roads.ForEach(road => road.Reset());
        }

        /// <summary>
        /// 道の幅の変更
        /// </summary>
        /// <param name="scale"> 拡大率 </param>
        public void ScaleRoad(float scale)
        {
            _roads.ForEach(road => road.ScaleWidth(scale));
        }
    }
}

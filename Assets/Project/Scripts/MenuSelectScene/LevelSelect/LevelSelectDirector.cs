using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class LevelSelectDirector : SingletonObject<LevelSelectDirector>
    {
        /// <summary>
        /// 木
        /// </summary>
        private static List<TreeController> _trees;

        /// <summary>
        /// 道
        /// </summary>
        private static List<RoadController> _roads;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(TagName.TREE).Select(tree => tree.GetComponent<TreeController>()).ToList<TreeController>();
            _roads = GameObject.FindGameObjectsWithTag(TagName.ROAD).Select(road => road.GetComponent<RoadController>()).ToList<RoadController>();
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            _trees.ForEach(tree => tree.UpdateReleased());
            _roads.ForEach(road => road.UpdateReleased());
        }

        /// <summary>
        /// 木と道の状態の保存
        /// </summary>
        private void OnDisable()
        {
            _trees.ForEach(tree => tree.SaveReleased());
            _roads.ForEach(road => road.SaveReleased());
        }

        /// <summary>
        /// 木と道の状態のリセット
        /// </summary>
        public static void Reset()
        {
            _trees.ForEach(tree => tree.Reset());
            _roads.ForEach(road => road.Reset());
        }
    }
}

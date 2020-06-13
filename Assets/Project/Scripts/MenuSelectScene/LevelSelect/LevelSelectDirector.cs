using System.Collections.Generic;
using UnityEngine;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using System.Linq;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class LevelSelectDirector : SingletonObject<LevelSelectDirector>
    {
        private static List<TreeController> trees;

        private static List<RoadController> roads;

        private void Awake() {
            var treeObjects = GameObject.FindGameObjectsWithTag(TagName.TREE);
            trees = treeObjects.Select(tree => tree.GetComponent<TreeController>()).ToList<TreeController>();
            var roadObjects = GameObject.FindGameObjectsWithTag(TagName.ROAD);
            roads = roadObjects.Select(road => road.GetComponent<RoadController>()).ToList<RoadController>();
        }

        /// <summary>
        /// 木と道の解放状況の更新
        /// </summary>
        private void OnEnable() {
            foreach (var tree in trees) {
                tree.UpdateReleased();
            }
            foreach (var road in roads) {
                road.UpdateReleased();
            }
        }

        /// <summary>
        /// 木と道の状態の保存
        /// </summary>
        private void OnDisable() {
            // 保存
            foreach (var tree in trees) {
                tree.SaveReleased();
            }
            foreach (var road in roads) {
                road.SaveReleased();
            }
        }

        /// <summary>
        /// 木と道の状態のリセット
        /// </summary>
        public static void Reset() {
            foreach (var tree in trees) {
                tree.Reset();
            }
            foreach (var road in roads) {
                road.Reset();
            }
        }
    }
}

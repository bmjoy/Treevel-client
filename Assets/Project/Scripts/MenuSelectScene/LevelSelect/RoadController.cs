using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class RoadController : LineController
    {
        private LevelTreeController _endObjectController;

        protected override void Awake()
        {
            base.Awake();
            _endObjectController = endObject.GetComponent<LevelTreeController>();
        }

        protected override void SetSaveKey()
        {
            saveKey = $"{startObject.GetComponent<LevelTreeController>().treeId}{PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponent<LevelTreeController>().treeId}";
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(saveKey);
        }

        /// <summary>
        /// 道の状態の更新
        /// </summary>
        public override void UpdateState()
        {
            released = PlayerPrefs.GetInt(saveKey, Default.ROAD_RELEASED) == 1;

            if (!released) {
                if (constraintObjects.Length == 0) {
                    // 初期状態で解放されている道
                    released = true;
                    // 終点の木の状態の更新
                    _endObjectController.state = ETreeState.Released;
                    _endObjectController.ReflectTreeState();
                } else {
                    released = constraintObjects.All(tree => tree.GetComponent<LevelTreeController>().state >= ETreeState.Cleared);
                    if (released) {
                        // 道が非解放状態から解放状態に変わった時
                        StartCoroutine(ReleaseEndObject());
                    }
                }
            }

            if (!released) {
                // 非解放時
                _renderer.startColor = new Color(0.2f, 0.2f, 0.7f);
                _renderer.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        /// <summary>
        /// 道が非解放状態から解放状態に変わった時のアニメーション(100フレームで色を変化させる)
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReleaseEndObject()
        {
            for (var i = 0; i < 100; i++) {
                _renderer.startColor = new Color((float)i / 100, (float)i / 100, (float)i / 100);
                _renderer.endColor = new Color((float)i / 100, (float)i / 100, (float)i / 100);
                yield return null;
            }
            // 終点の木の状態の更新
            _endObjectController.state = ETreeState.Released;
            _endObjectController.ReflectTreeState();
        }

        /// <summary>
        /// 道の状態の保存
        /// </summary>
        public override void SaveState()
        {
            PlayerPrefs.SetInt(saveKey, Convert.ToInt32(released));
        }
    }
}

using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class RoadController : LineController
    {
        /// <summary>
        /// 道のID
        /// </summary>
        [SerializeField] private ERoadId _roadId;

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.ROAD + _roadId.ToString());
        }

        /// <summary>
        /// 道の状態の更新
        /// </summary>
        public override void UpdateReleased()
        {
            released = PlayerPrefs.GetInt(PlayerPrefsKeys.ROAD + _roadId.ToString(), Default.ROAD_RELEASED) == 1;

            if (!released) {
                if (constraintObjects.Length == 0) {
                    // 初期状態で解放されている道
                    released = true;
                } else {
                    released = constraintObjects.All(tree => tree.GetComponent<TreeController>().cleared);
                }
            }

            // 終点の木の状態の更新
            endObject.GetComponent<TreeController>().released = released;
            button.enabled = released;

            if (!released) {
                // 非解放時
                _render.startColor = new Color(0.2f, 0.2f, 0.7f);
                _render.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        /// <summary>
        /// 道の状態の保存
        /// </summary>
        public override void SaveReleased()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.ROAD + _roadId.ToString(), Convert.ToInt32(released));
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Linq;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class RoadController : LineController
    {
        [SerializeField] private ERoadId _roadId;

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.ROAD + _roadId.ToString());
        }

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

            endObject.GetComponent<TreeController>().released = released;
            button.enabled = released;

            if (!released) {
                // 非解放時
                _render.startColor = new Color(0.2f, 0.2f, 0.7f);
                _render.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        public override void SaveReleased()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.ROAD + _roadId.ToString(), Convert.ToInt32(released));
        }
    }
}

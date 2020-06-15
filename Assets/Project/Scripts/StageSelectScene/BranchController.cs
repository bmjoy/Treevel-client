using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.MenuSelectScene.LevelSelect;
using Project.Scripts.StageSelectScene;
using System;
using System.Linq;

namespace Project.Scripts.StageSelectScene
{
    public class BranchController : LineController
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        private int _startNumber;

        private int _endNumber;

        protected override void Awake() {
            base.Awake();
            _startNumber = startObject.GetComponent<StageController>().stageNumber;
            _endNumber = endObject.GetComponent<StageController>().stageNumber;
        }

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(_treeId.ToString() + _startNumber + PlayerPrefsKeys.KEY_CONNECT_CHAR + _endNumber);
        }

        public override void UpdateReleased()
        {
            released = PlayerPrefs.GetInt(_treeId.ToString() + _startNumber + PlayerPrefsKeys.KEY_CONNECT_CHAR + _endNumber, Default.BRANCH_RELEASED) == 1;

            if (!released) {
                if (constraintObjects.Length == 0) {
                    // 初期状態で解放されている道
                    released = true;
                } else {
                    released = constraintObjects.All(stage => stage.GetComponent<StageController>().cleared);
                }

                if (released) {
                    button.enabled = true;
                }
            }
            
            button.enabled = released;
            endObject.transform.Find("Lock")?.gameObject.SetActive(!released);

            if (!released) {
                // 非解放時
                _render.startColor = new Color(0.2f, 0.2f, 0.7f);
                _render.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        public override void SaveReleased()
        {
            PlayerPrefs.SetInt(_treeId.ToString() + _startNumber + PlayerPrefsKeys.KEY_CONNECT_CHAR + _endNumber, Convert.ToInt32(released));
        }
    }
}

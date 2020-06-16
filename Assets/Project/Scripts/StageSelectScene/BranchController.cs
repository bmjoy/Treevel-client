using Project.Scripts.MenuSelectScene.LevelSelect;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
    public class BranchController : LineController
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        /// <summary>
        /// 始点のステージ番号
        /// </summary>
        private int _startNumber;

        /// <summary>
        /// 終点のステージ番号
        /// </summary>
        private int _endNumber;

        protected override void Awake()
        {
            base.Awake();
            _startNumber = startObject.GetComponent<StageController>().stageNumber;
            _endNumber = endObject.GetComponent<StageController>().stageNumber;
        }

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(_treeId.ToString() + _startNumber + PlayerPrefsKeys.KEY_CONNECT_CHAR + _endNumber);
        }

        /// <summary>
        /// 枝の状態の更新
        /// </summary>
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
            }

            // 終点のステージの状態の更新
            endObject.GetComponent<StageController>().released = released;
            button.enabled = released;
            // 鍵穴付けるか
            endObject.transform.Find("Lock")?.gameObject.SetActive(!released);

            if (!released) {
                // 非解放時
                _render.startColor = new Color(0.2f, 0.2f, 0.7f);
                _render.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        /// <summary>
        /// 枝の状態の保存
        /// </summary>
        public override void SaveReleased()
        {
            PlayerPrefs.SetInt(_treeId.ToString() + _startNumber + PlayerPrefsKeys.KEY_CONNECT_CHAR + _endNumber, Convert.ToInt32(released));
        }
    }
}

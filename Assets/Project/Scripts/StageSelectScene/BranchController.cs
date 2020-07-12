﻿using Project.Scripts.MenuSelectScene.LevelSelect;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class BranchController : LineController
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        private Button _endObjectButton;

        protected override void Awake()
        {
            _endObjectButton = endObject.GetComponent<Button>();
        }

        protected override void SetSaveKey()
        {
            saveKey = $"{_treeId}{PlayerPrefsKeys.KEY_CONNECT_CHAR}{startObject.GetComponent<StageController>().stageNumber}{PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponent<StageController>().stageNumber}";
        }

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(saveKey);
        }

        /// <summary>
        /// 枝の状態の更新
        /// </summary>
        public override void UpdateState()
        {
            released = PlayerPrefs.GetInt(saveKey, Default.BRANCH_RELEASED) == 1;

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
            _endObjectButton.enabled = released;
            // 鍵穴付けるか
            endObject.transform.Find("Lock")?.gameObject.SetActive(!released);

            if (!released) {
                // 非解放時
                _renderer.startColor = new Color(0.2f, 0.2f, 0.7f);
                _renderer.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        /// <summary>
        /// 枝の状態の保存
        /// </summary>
        public override void SaveState()
        {
            PlayerPrefs.SetInt(saveKey, Convert.ToInt32(released));
        }
    }
}
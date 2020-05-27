using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class ResetController : MonoBehaviour
    {
        /// <summary>
        /// ステージリセットボタン
        /// </summary>
        private Button _resetButton;

        private void Awake()
        {
            _resetButton = GetComponent<Button>();
            _resetButton.onClick.AddListener(ResetButtonDown);
        }

        /// <summary>
        /// ステージリセットボタンを押した場合の処理
        /// </summary>
        private static void ResetButtonDown()
        {
            // 全ステージをリセット
            foreach (ELevelName levelName in Enum.GetValues(typeof(ELevelName))) {
                var stageNum = LevelInfo.NUM[levelName];
                var stageStartId = LevelInfo.STAGE_START_ID[levelName];

                for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                    StageStatus.Reset(stageId);
                }
            }

            // キャンバスの設定をリセット
            UserSettings.CanvasScale = Default.CANVAS_SCALE;
        }
    }
}

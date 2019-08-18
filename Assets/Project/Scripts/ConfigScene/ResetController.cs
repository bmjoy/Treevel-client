using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;


namespace Project.Scripts.ConfigScene
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
            foreach (EStageLevel stageLevel in Enum.GetValues(typeof(EStageLevel))) {
                var stageNum = StageInfo.Num[stageLevel];
                var stageStartId = StageInfo.StageStartId[stageLevel];

                for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                    StageStatus.Reset(stageId);
                }
            }
        }
    }
}

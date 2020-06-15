using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    [RequireComponent(typeof(Button))]
    public class StageController : MonoBehaviour
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        /// <summary>
        /// ステージの番号
        /// </summary>
        [SerializeField] public int stageNumber;

        public bool cleared = false;


        public void UpdateReleased()
        {
            var stageData = GameDataBase.GetStage(_treeId, stageNumber);
            if (stageData == null)
                return;

            var isUnLocked = stageData.IsUnLocked();
            // ボタンのクリック可能か
            GetComponent<Button>().enabled = isUnLocked;

            // 鍵穴付けるか
            transform.Find("Lock")?.gameObject.SetActive(!isUnLocked);

            // クリアしたらグレイスケールを解除
            cleared = StageStatus.Get(_treeId, stageNumber).passed;
            if (cleared) {
                GetComponent<Image>().material = null;
            }
        }

        public void StageButtonDown()
        {
            if (UserSettings.StageDetails == 1) {
                StageSelectDirector.Instance.ShowOverPopup(_treeId, stageNumber);
            } else {
                GoToGame();
            }
        }

        /// <summary>
        /// ステージ選択画面からゲーム選択画面へ移動する
        /// </summary>
        private void GoToGame()
        {
            StageSelectDirector.Instance.GoToGame(_treeId, stageNumber);
        }
    }
}

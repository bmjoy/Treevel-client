using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
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

        /// <summary>
        /// 解放状態
        /// </summary>
        [SerializeField] public bool released;

        /// <summary>
        /// クリア状態
        /// </summary>
        [NonSerialized] public bool cleared = false;

        /// <summary>
        /// ステージの状態の更新
        /// </summary>
        public void UpdateReleased()
        {
            var stageData = GameDataBase.GetStage(_treeId, stageNumber);
            if (stageData == null)
                return;

            var stageStatus = StageStatus.Get(_treeId, stageNumber);
            if (stageNumber != 1)
                released = stageStatus.released;

            cleared = stageStatus.cleared;

            // 鍵穴付けるか
            transform.Find("Lock")?.gameObject.SetActive(!released);

            // クリアしたらグレイスケールを解除
            if (cleared) {
                GetComponent<Image>().material = null;
            }
        }

        /// <summary>
        /// ステージの状態の保存
        /// </summary>
        public void StageButtonDown()
        {
            if (UserSettings.StageDetails == 1) {
                StageSelectDirector.Instance.ShowOverPopup(_treeId, stageNumber);
            } else {
                StageSelectDirector.Instance.GoToGame(_treeId, stageNumber);
            }
        }
    }
}

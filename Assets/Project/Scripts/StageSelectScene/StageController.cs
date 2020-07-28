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
        /// ステージ情報
        /// </summary>
        public StageStatus stageStatus;

        /// <summary>
        /// ステージの状態
        /// </summary>
        public EStageState state;

        /// <summary>
        /// ボタン
        /// </summary>
        [SerializeField] private Button _button;

        /// <summary>
        /// ステージの状態の更新
        /// </summary>
        public void UpdateState()
        {
            var stageData = GameDataBase.GetStage(_treeId, stageNumber);
            if (stageData == null)
                return;

            stageStatus = StageStatus.Get(_treeId, stageNumber);
            state = stageStatus.state;
            
            // 状態の反映
            ReflectTreeState();
        }

        public void ReflectTreeState()
        {
            switch (state) {
                case EStageState.Unreleased:
                    transform.Find("Lock")?.gameObject.SetActive(true);
                    _button.enabled = false;
                    break;
                case EStageState.Released:
                    transform.Find("Lock")?.gameObject.SetActive(false);
                    _button.enabled = true;
                    break;
                case EStageState.Cleared:
                    transform.Find("Lock")?.gameObject.SetActive(false);
                    GetComponent<Image>().material = null;
                    _button.enabled = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void ReleaseStage() {
            state = EStageState.Released;
            stageStatus.ReleaseStage(_treeId, stageNumber);
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

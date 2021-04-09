using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
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
        private StageStatus _stageStatus;

        /// <summary>
        /// ステージの状態
        /// </summary>
        public EStageState state;

        /// <summary>
        /// ボタン
        /// </summary>
        [SerializeField] private Button _button;

        private async void Awake()
        {
            _stageStatus = await NetworkService.Execute(new GetStageStatusRequest(_treeId, stageNumber));

            UpdateState().Forget();
        }

        /// <summary>
        /// ステージの状態の更新
        /// </summary>
        private async UniTask UpdateState()
        {
            var stageData = GameDataManager.GetStage(_treeId, stageNumber);
            if (stageData == null) return;

            if (_stageStatus.successNum > 0) {
                state = EStageState.Cleared;
            } else if (stageData.ConstraintStages.Count == 0) {
                state = EStageState.Released;
            } else {
                state = (await UniTask.WhenAll(stageData.ConstraintStages.Select(constraintStage => {
                    var (treeId, stageNum) = StageData.DecodeStageIdKey(constraintStage);
                    return NetworkService.Execute(new GetStageStatusRequest(treeId, stageNum));
                }))).All(stageStatus => stageStatus.successNum > 0)
                    ? EStageState.Released
                    : EStageState.Unreleased;
            }

            // 状態の反映
            ReflectTreeState();
        }

        public void ReflectTreeState()
        {
            switch (state) {
                case EStageState.Unreleased:
                    // 鍵とグレースケール
                    transform.Find("Lock")?.gameObject.SetActive(true);
                    _button.enabled = false;
                    break;
                case EStageState.Released:
                    // グレースケール
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

        /// <summary>
        /// ステージを解放状態にする
        /// </summary>
        public void ReleaseStage()
        {
            state = EStageState.Released;
            _stageStatus?.ReleaseStage();
        }

        /// <summary>
        /// ステージの状態の保存
        /// </summary>
        public void StageButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);

            if (UserSettings.StageDetails == 1) {
                StageSelectDirector.Instance.ShowOverPopup(_treeId, stageNumber);
            } else {
                StageSelectDirector.Instance.GoToGameAsync(_treeId, stageNumber).Forget();
            }
        }
    }
}

using System;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Networks.Objects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
{
    public class OverviewPopup : MonoBehaviour
    {
        /// <summary>
        /// ゲームを開始するボタン
        /// </summary>
        public Button goToGameButton;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <param name="stats"> サーバからもらったステージスタッツデータ </param>
        public void Initialize(ETreeId treeId, int stageNumber, StageStats stats)
        {
            var stageData = GameDataManager.GetStage(treeId, stageNumber);

            if (stageData == null) {
                UIManager.Instance.ShowErrorMessage(EErrorCode.UnknownError);
                return;
            }

            // ステージID
            var stageNumberText = transform.Find("PanelBackground/StageNumIconBase/StageNumText").GetComponent<Text>();
            stageNumberText.text = stageNumber.ToString();

            // クリア割合
            var clearPercentageText = transform.Find("PanelBackground/StatusPanel/SuccessPercentage/Data")
                .GetComponent<Text>();
            clearPercentageText.text = $"{stats.ClearRate * 100f:n2}%";

            // 最速クリアタイム
            var minClearTimeText = transform.Find("PanelBackground/StatusPanel/ShortestClearTime/Data")
                .GetComponent<Text>();
            var time = TimeSpan.FromSeconds(stats.MinClearTime);
            minClearTimeText.text = $"{time.Minutes}:{time.Seconds}'{time.Milliseconds}";

            // 登場ギミック
            var overviewGimmicks = stageData.OverviewGimmicks;
            var appearingGimmicks = transform.Find("PanelBackground/AppearingGimmicks").gameObject;
            for (var i = 1; i <= 3; ++i) {
                var newFeatureOverview = appearingGimmicks.transform.Find($"GimmickOverview{i}");
                if (overviewGimmicks.Count >= i) {
                    newFeatureOverview.GetComponentInChildren<Text>().text = overviewGimmicks[i - 1].ToString();
                    newFeatureOverview.gameObject.SetActive(true);
                } else {
                    newFeatureOverview.gameObject.SetActive(false);
                }
            }

            // ゲームを開始するボタン
            goToGameButton = transform.Find("PanelBackground/GoToGame").GetComponent<Button>();
            goToGameButton.OnClickAsObservable()
                .Subscribe(_ => StageSelectDirector.Instance.GoToGameAsync(treeId, stageNumber))
                .AddTo(this);
        }
    }
}

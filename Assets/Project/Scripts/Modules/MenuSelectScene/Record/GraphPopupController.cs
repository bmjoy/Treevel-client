using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Objects;
using Treevel.Common.Networks.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class GraphPopupController : MonoBehaviour
    {
        /// <summary>
        /// [Text] 挑戦回数
        /// </summary>
        [SerializeField] private Text _challengeNumText;

        /// <summary>
        /// [Text] 成功率
        /// </summary>
        [SerializeField] private Text _clearRateText;

        /// <summary>
        /// 補助線
        /// </summary>
        [SerializeField] private GameObject _indicatorLine;

        /// <summary>
        /// 現在表示しているポップアップのステージ番号
        /// </summary>
        public int currentStageNumber;

        public async void Initialize(ESeasonId seasonId, ETreeId treeId, int stageNumber, Vector3 graphPosition)
        {
            var stageStatus = StageStatus.Get(treeId, stageNumber);

            var challengeNum = stageStatus.challengeNum;
            _challengeNumText.text = challengeNum.ToString();

            var isClear = stageStatus.successNum > 0;
            gameObject.GetComponent<Image>().color = isClear ? seasonId.GetColor() : Color.gray;

            // ポップアップが表示される位置を、該当する棒グラフの位置に合わせて変える
            var rectTransform = GetComponent<RectTransform>();
            // 左右に必要なマージン。pivot が 0.5 なので、ポップアップの横幅も考慮する
            var margin = 50 + rectTransform.rect.width / 2;
            var position = rectTransform.position;
            if (graphPosition.x < margin) {
                // 左限界
                position.x = margin;
            } else if (Screen.width - margin < graphPosition.x) {
                // 右限界
                position.x = Screen.width - margin;
            } else {
                position.x = graphPosition.x;
            }

            rectTransform.position = position;

            // 補助線を適切な位置に配置する
            var lineRectTransform = _indicatorLine.GetComponent<RectTransform>();
            var linePosition = lineRectTransform.position;
            linePosition.x = graphPosition.x;
            lineRectTransform.sizeDelta = new Vector2(lineRectTransform.rect.width, linePosition.y - graphPosition.y);
            _indicatorLine.GetComponent<RectTransform>().position = linePosition;

            var data = await NetworkService.Execute(new GetStageStatsRequest(StageData.EncodeStageIdKey(treeId, stageNumber)));
            var stageStats = (StageStats)data;
            _clearRateText.text = $"{stageStats.ClearRate * 100f:n2}";

            currentStageNumber = stageNumber;
        }
    }
}

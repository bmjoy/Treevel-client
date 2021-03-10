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

        /// <summary>
        /// ポップアップの横幅
        /// </summary>
        private float _width;

        /// <summary>
        /// ポップアップの縦幅
        /// </summary>
        private float _height;

        /// <summary>
        /// 左右に必要なマージン
        /// </summary>
        private float _margin;

        /// <summary>
        /// 表示される最大挑戦回数
        /// </summary>
        private const int _MAX_CHALLENGE_NUM_DISPLAYED = 9999;

        private void Awake()
        {
            _width = Screen.width * 0.5f;
            _height = _width * 0.5f;
            _margin = Screen.width * 0.05f;

            GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _height);
        }

        public async void Initialize(Color seasonColor, ETreeId treeId, int stageNumber, Vector3 graphPosition)
        {
            var stageStatus = await NetworkService.Execute(new GetStageStatusRequest(treeId, stageNumber));

            var challengeNum = stageStatus.challengeNum;
            if (challengeNum <= _MAX_CHALLENGE_NUM_DISPLAYED) {
                _challengeNumText.text = challengeNum + " 回プレイ";
            } else {
                _challengeNumText.text = $"{_MAX_CHALLENGE_NUM_DISPLAYED}+ 回プレイ";
            }

            var isClear = stageStatus.successNum > 0;
            gameObject.GetComponent<Image>().color = isClear ? seasonColor : Color.gray;

            // ポップアップが表示される位置を、該当する棒グラフの位置に合わせて変える
            var rectTransform = GetComponent<RectTransform>();
            // pivot が 0.5 なので、ポップアップの横幅も考慮する
            var margin = _margin + _width / 2;
            var position = rectTransform.position;
            position.x = Mathf.Clamp(graphPosition.x, margin, Screen.width - margin);
            rectTransform.position = position;

            // 補助線を適切な位置に配置する
            var lineRectTransform = _indicatorLine.GetComponent<RectTransform>();
            var linePosition = lineRectTransform.position;
            linePosition.x = graphPosition.x;
            lineRectTransform.sizeDelta = new Vector2(lineRectTransform.rect.width, linePosition.y - graphPosition.y);
            lineRectTransform.position = linePosition;

            // TODO: implement PlayFab version
            var stageStats = new StageStats();
            // var stageStats = (StageStats)await NetworkService.Execute(new GetStageStatsRequest(StageData.EncodeStageIdKey(treeId, stageNumber)));

            _clearRateText.text = $"{stageStats.ClearRate * 100f:n2}";

            currentStageNumber = stageNumber;
        }
    }
}

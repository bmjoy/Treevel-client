using System;
using System.Linq;
using Treevel.Common.Entities;
using UnityEngine;
using UnityEngine.UI;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.StageSelectScene;
using UniRx;
using UniRx.Triggers;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class IndividualRecordDirector : MonoBehaviour
    {
        /// <summary>
        /// [UI] グラフのポップアップ
        /// </summary>
        [SerializeField] private GameObject _graphPopup;

        /// <summary>
        /// [UI] "ステージクリア数" の prefab
        /// </summary>
        [SerializeField] private GameObject _clearStageNum;

        /// <summary>
        /// [UI] 棒グラフの prefab
        /// 現状，グラフの数は 10 で決めうち
        /// </summary>
        [SerializeField] private GameObject[] _graphBars = new GameObject[10];

        /// <summary>
        /// [UI] 棒グラフの y 軸ラベルの text
        /// 一番下のラベルは 0 で確定なので 3 つを考慮
        /// </summary>
        [SerializeField] private Text[] _graphAxisLabels = new Text[3];

        /// <summary>
        /// [UI] "ステージ一覧へ"
        /// </summary>
        [SerializeField] private Button _toStageSelectSceneButton;

        private StageStatus[] _stageStatuses;

        /// <summary>
        /// y 軸ラベルの最小値
        /// </summary>
        private const int _MIN_AXIS_LABEL_NUM = 30;

        /// <summary>
        /// y 軸ラベルの最大値
        /// </summary>
        private const int _MAX_AXIS_LABEL_NUM = 999;

        /// <summary>
        /// y 軸ラベルの更新幅
        /// </summary>
        private const int _AXIS_LABEL_MARGIN = 30;

        private void Awake()
        {
            _toStageSelectSceneButton.onClick.AddListener(() =>
            {
                // TODO: 季節，木を拡張したら，こちらも拡張する
                StageSelectDirector.seasonId = ESeasonId.Spring;
                StageSelectDirector.treeId = ETreeId.Spring_1;
                AddressableAssetManager.LoadScene(Constants.SceneName.SPRING_STAGE_SELECT_SCENE);
            });

            _graphBars
                // index の抽出
                .Select((graphBar, index) => (graphBar, index + 1)).ToList()
                .ForEach(args => {
                    var (graphBar, stageNumber) = args;

                    graphBar.AddComponent<ObservableEventTrigger>()
                        .OnPointerDownAsObservable()
                        .Subscribe(_ => {
                            var graphPopupController = _graphPopup.GetComponent<GraphPopupController>();
                            if (_graphPopup.activeSelf && graphPopupController.currentStageNumber == stageNumber) {
                                // 再度同じステージ番号のグラフがタップされたら、ポップアップを閉じる
                                _graphPopup.SetActive(false);
                            } else {
                                var positionX = _graphBars[stageNumber - 1].GetComponent<RectTransform>().position.x;
                                _graphPopup.SetActive(true);
                                graphPopupController.Initialize(ETreeId.Spring_1, stageNumber, positionX);
                            }
                        })
                        .AddTo(this);
                });
        }

        private void OnEnable()
        {
            _stageStatuses = GameDataManager.GetStages(ETreeId.Spring_1)
                .Select(stage => StageStatus.Get(stage.TreeId, stage.StageNumber))
                .ToArray();

            var clearStageNum = _stageStatuses.Count(stageStatus => stageStatus.successNum > 0);
            var totalStageNum = _stageStatuses.Length;
            _clearStageNum.GetComponent<ClearStageNumController>().SetUp(clearStageNum, totalStageNum, Color.magenta);

            SetupBarGraph();
        }

        private void SetupBarGraph()
        {
            var challengeNumMax = (float) _stageStatuses.Select(stageStatus => stageStatus.challengeNum).Max();

            // 1~30 は 30、31~60 は 60 にするために Ceiling を使用
            var maxAxisLabelNum = Mathf.Clamp((int) Math.Ceiling(challengeNumMax / _AXIS_LABEL_MARGIN) * _AXIS_LABEL_MARGIN, _MIN_AXIS_LABEL_NUM,  _MAX_AXIS_LABEL_NUM);

            _graphAxisLabels[0].text = (maxAxisLabelNum / 3).ToString();
            _graphAxisLabels[1].text = (maxAxisLabelNum * 2 / 3).ToString();
            _graphAxisLabels[2].text = maxAxisLabelNum != _MAX_AXIS_LABEL_NUM ? maxAxisLabelNum.ToString() : maxAxisLabelNum + "+";

            _stageStatuses
                .Select((stageStatus, index) => (_graphBars[index], stageStatus.successNum, stageStatus.challengeNum))
                .ToList()
                .ForEach(args =>
                {
                    var (graphBar, successNum, challengeNum) = args;

                    graphBar.GetComponent<Image>().color = successNum > 0 ? Color.magenta : Color.gray;

                    var anchorMinY = graphBar.GetComponent<RectTransform>().anchorMin.y;
                    var anchorMaxY = Mathf.Min(anchorMinY + (1.0f - anchorMinY) * challengeNum / maxAxisLabelNum, 1.0f);
                    graphBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchorMaxY);
                });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Modules.StageSelectScene;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class IndividualRecordDirector : MonoBehaviour
    {
        /// <summary>
        /// Model
        /// </summary>
        private IndividualRecordModel _model;

        /// <summary>
        /// [View] グラフのポップアップ
        /// </summary>
        [SerializeField] private GameObject _graphPopup;

        /// <summary>
        /// [View] "ステージクリア数" の prefab
        /// </summary>
        [SerializeField] private GameObject _clearStageNum;

        /// <summary>
        /// [View] 棒グラフの prefab
        /// 現状，グラフの数は 10 で決めうち
        /// </summary>
        [SerializeField] private GameObject[] _graphBars = new GameObject[10];

        /// <summary>
        /// [View] 棒グラフの y 軸ラベルの text
        /// 一番下のラベルは 0 で確定なので 3 つを考慮
        /// </summary>
        [SerializeField] private Text[] _graphAxisLabels = new Text[3];

        /// <summary>
        /// [View] "ステージ一覧へ"
        /// </summary>
        [SerializeField] private Button _toStageSelectSceneButton;

        /// <summary>
        /// [View] ドロップダウン
        /// </summary>
        [SerializeField] private Dropdown _dropdown;

        /// <summary>
        /// [View] ドロップダウンテンプレート
        /// </summary>
        [SerializeField] private GameObject _dropdownTemplate;

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
            _model = new IndividualRecordModel();

            /*
             * Model -> View
             */

            _model.currentSeason.Subscribe(season => {
                _graphPopup.SetActive(false);
                SetDropdownOptions(season);
            }).AddTo(this);

            _model.currentTree.Subscribe(_ => {
                _graphPopup.SetActive(false);
            }).AddTo(this);

            _model.stageRecordArray
                .Subscribe(stageRecordArray => {
                    var clearStageNum = stageRecordArray.Count(stageRecord => stageRecord.IsCleared);
                    var totalStageNum = stageRecordArray.Length;

                    _clearStageNum.GetComponent<ClearStageNumController>()
                        .SetUp(clearStageNum, totalStageNum, _model.currentSeason.Value.GetColor());

                    SetupBarGraph();
                }).AddTo(this);

            /*
             * View -> Model
             */

            _dropdown.onValueChanged.AsObservable()
                .Subscribe(selected => {
                    _model.currentTree.Value = (ETreeId)Enum.Parse(typeof(ETreeId), _dropdown.options[selected].text);
                }).AddTo(this);

            foreach (var seasonToggle in FindObjectsOfType<SeasonSelectButton>()) {
                var toggle = seasonToggle.GetComponent<Toggle>();
                toggle.OnValueChangedAsObservable()
                    .Where(isOn => isOn)
                    .Subscribe(_ => {
                        _model.currentSeason.Value = seasonToggle.SeasonId;
                    }).AddTo(this);
            }

            _toStageSelectSceneButton.onClick.AsObservable()
                .Subscribe(_ => {
                    StageSelectDirector.seasonId = _model.currentSeason.Value;
                    StageSelectDirector.treeId = _model.currentTree.Value;

                    AddressableAssetManager.LoadScene(_model.currentSeason.Value.GetSceneName());
                }).AddTo(this);

            _graphBars
                // index の抽出
                .Select((graphBar, index) => (graphBar, index + 1)).ToList()
                .ForEach(args => {
                    var (graphBar, stageNumber) = args;

                    graphBar.transform.parent.gameObject.AddComponent<ObservableEventTrigger>()
                        .OnPointerDownAsObservable()
                        .Subscribe(_ => {
                            var graphPopupController = _graphPopup.GetComponent<GraphPopupController>();
                            if (_graphPopup.activeSelf && graphPopupController.currentStageNumber == stageNumber) {
                                // 再度同じステージ番号のグラフがタップされたら、ポップアップを閉じる
                                _graphPopup.SetActive(false);
                            } else {
                                var graphPosition = _graphBars[stageNumber - 1].GetComponent<RectTransform>().position;
                                _graphPopup.SetActive(true);
                                graphPopupController.InitializeAsync(_model.currentSeason.Value.GetColor(), _model.currentTree.Value, stageNumber, graphPosition).Forget();
                            }
                        })
                        .AddTo(this);
                });
        }

        private void OnEnable()
        {
            _model.FetchStageRecordArray();
        }

        private void OnDestroy()
        {
            _model.Dispose();
        }

        private void SetDropdownOptions(ESeasonId seasonId)
        {
            _dropdown.options = seasonId.GetTrees()
                .Select(tree => new Dropdown.OptionData {
                    text = Enum.GetName(typeof(ETreeId), tree),
                })
                .ToList();

            _dropdown.value = 0;
            _dropdownTemplate.GetComponent<Image>().color = seasonId.GetColor();
            _dropdown.image.color = seasonId.GetColor();
            _dropdown.RefreshShownValue();
        }

        private void SetupBarGraph()
        {
            var challengeNumMax = (float)_model.stageRecordArray.Value
                .Select(stageRecord => stageRecord.challengeNum).Max();

            // 1~30 は 30、31~60 は 60 にするために Ceiling を使用
            var maxAxisLabelNum =
                Mathf.Clamp((int)Math.Ceiling(challengeNumMax / _AXIS_LABEL_MARGIN) * _AXIS_LABEL_MARGIN,
                            _MIN_AXIS_LABEL_NUM, _MAX_AXIS_LABEL_NUM);

            _graphAxisLabels[0].text = (maxAxisLabelNum / 3).ToString();
            _graphAxisLabels[1].text = (maxAxisLabelNum * 2 / 3).ToString();
            _graphAxisLabels[2].text = maxAxisLabelNum != _MAX_AXIS_LABEL_NUM
                ? maxAxisLabelNum.ToString()
                : maxAxisLabelNum + "+";

            _model.stageRecordArray.Value
                .Select((stageRecord, index) => (_graphBars[stageRecord.stageNumber - 1], stageRecord.IsCleared, ChallengeNum: stageRecord.challengeNum))
                .ToList()
                .ForEach(args => {
                    var (graphBar, isClear, challengeNum) = args;

                    graphBar.GetComponent<Image>().color = isClear ? _model.currentSeason.Value.GetColor() : Color.gray;

                    var anchorMinY = graphBar.GetComponent<RectTransform>().anchorMin.y;
                    var anchorMaxY = Mathf.Min(anchorMinY + (1.0f - anchorMinY) * challengeNum / maxAxisLabelNum, 1.0f);
                    graphBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchorMaxY);
                });
        }
    }
}

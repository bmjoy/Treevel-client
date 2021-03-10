using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
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

        [SerializeField] private Dropdown _dropdown;

        [SerializeField] private GameObject _dropdownTemplate;

        /// <summary>
        /// 現在表示している季節
        /// </summary>
        private readonly ReactiveProperty<ESeasonId> _currentSeason = new ReactiveProperty<ESeasonId>(ESeasonId.Spring);

        /// <summary>
        /// 現在表示している木
        /// </summary>
        private readonly ReactiveProperty<ETreeId> _currentTree = new ReactiveProperty<ETreeId>();

        private void Awake()
        {
            _currentTree.Value = _currentSeason.Value.GetFirstTree();

            // 季節変更時の処理
            _currentSeason.Subscribe(season => {
                _graphPopup.SetActive(false);
                SetDropdownOptions(season);
                _currentTree.Value = season.GetFirstTree();
            }).AddTo(this);

            // 木変更時の処理
            _currentTree.Subscribe(tree => {
                _graphPopup.SetActive(false);
                SetStageStatuses();
                SetupBarGraph();
            }).AddTo(this);

            // 木UI制御
            _dropdown.onValueChanged.AsObservable()
                .Subscribe(selected => {
                    _currentTree.Value = (ETreeId)Enum.Parse(typeof(ETreeId), _dropdown.options[selected].text);
                }).AddTo(this);

            // 季節制御
            var seasonToggles = FindObjectsOfType<SeasonSelectButton>();
            foreach (var seasonToggle in seasonToggles) {
                var toggle = seasonToggle.GetComponent<Toggle>();
                toggle.OnValueChangedAsObservable()
                    .Where(isOn => isOn)
                    .Subscribe(isOn => {
                        _currentSeason.Value = seasonToggle.SeasonId;
                    }).AddTo(this);
            }

            _toStageSelectSceneButton.onClick.AsObservable()
                .Subscribe(_ => {
                    StageSelectDirector.seasonId = _currentSeason.Value;
                    StageSelectDirector.treeId = _currentTree.Value;
                    AddressableAssetManager.LoadScene(_currentSeason.Value.GetSceneName());
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
                                graphPopupController.Initialize(_currentSeason.Value.GetColor(), _currentTree.Value, stageNumber, graphPosition);
                            }
                        })
                        .AddTo(this);
                });
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

        private void OnEnable()
        {
            SetStageStatuses();
            SetupBarGraph();
        }

        private async void SetStageStatuses()
        {
            var tasks = GameDataManager.GetStages(_currentTree.Value)
                // FIXME: 呼ばれるたびに ステージ数 分リクエストしてしまうので、リクエストを減らす工夫をする
                .Select(stage => NetworkService.Execute(new GetStageStatusRequest(stage.TreeId, stage.StageNumber)));
            _stageStatuses = await UniTask.WhenAll(tasks);

            var clearStageNum = _stageStatuses.Count(stageStatus => stageStatus.successNum > 0);
            var totalStageNum = _stageStatuses.Length;
            _clearStageNum.GetComponent<ClearStageNumController>()
                .SetUp(clearStageNum, totalStageNum, _currentSeason.Value.GetColor());
        }

        private void SetupBarGraph()
        {
            var challengeNumMax = (float)_stageStatuses.Select(stageStatus => stageStatus.challengeNum).Max();

            // 1~30 は 30、31~60 は 60 にするために Ceiling を使用
            var maxAxisLabelNum =
                Mathf.Clamp((int)Math.Ceiling(challengeNumMax / _AXIS_LABEL_MARGIN) * _AXIS_LABEL_MARGIN,
                            _MIN_AXIS_LABEL_NUM, _MAX_AXIS_LABEL_NUM);

            _graphAxisLabels[0].text = (maxAxisLabelNum / 3).ToString();
            _graphAxisLabels[1].text = (maxAxisLabelNum * 2 / 3).ToString();
            _graphAxisLabels[2].text = maxAxisLabelNum != _MAX_AXIS_LABEL_NUM
                ? maxAxisLabelNum.ToString()
                : maxAxisLabelNum + "+";

            _stageStatuses
                .Select((stageStatus, index) => (_graphBars[index], stageStatus.successNum, stageStatus.challengeNum))
                .ToList()
                .ForEach(args => {
                    var (graphBar, successNum, challengeNum) = args;

                    graphBar.GetComponent<Image>().color =
                        successNum > 0 ? _currentSeason.Value.GetColor() : Color.gray;

                    var anchorMinY = graphBar.GetComponent<RectTransform>().anchorMin.y;
                    var anchorMaxY = Mathf.Min(anchorMinY + (1.0f - anchorMinY) * challengeNum / maxAxisLabelNum, 1.0f);
                    graphBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchorMaxY);
                });
        }
    }
}

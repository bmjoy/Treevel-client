using System;
using System.Linq;
using Treevel.Common.Components.UIs;
using Treevel.Common.Entities;
using UnityEngine;
using UnityEngine.UI;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.StageSelectScene;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class IndividualRecordDirector : MonoBehaviour
    {
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

        /// <summary>
        /// 現在表示している季節
        /// </summary>
        private ESeasonId _currentSeason;

        /// <summary>
        /// 現在表示している木
        /// </summary>
        private ETreeId _currentTree;

        private void Awake()
        {
            _currentSeason = ESeasonId.Spring;
            _currentTree = _currentSeason.GetFirstTree();

            _toStageSelectSceneButton.onClick.AddListener(() =>
            {
                StageSelectDirector.seasonId = _currentSeason;
                StageSelectDirector.treeId = _currentTree;
                AddressableAssetManager.LoadScene(_currentSeason.GetSceneName());
            });

            SetDropdownOptions(_currentSeason);
        }

        private void SetDropdownOptions(ESeasonId seasonId)
        {
            _dropdown.options = seasonId.GetTrees()
                .Select(tree => new Dropdown.OptionData { text = Enum.GetName(typeof(ETreeId), tree) })
                .ToList();
            _dropdown.RefreshShownValue();
        }

        private void OnEnable()
        {
            _stageStatuses = GameDataManager.GetStages(_currentTree)
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

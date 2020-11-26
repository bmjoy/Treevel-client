using System;
using System.Linq;
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
        private const int _MAX_AXIS_LABEL_NUM = 990;

        private void Awake()
        {
            _toStageSelectSceneButton.onClick.AddListener(() =>
            {
                // TODO: 季節，木を拡張したら，こちらも拡張する
                StageSelectDirector.levelName = ELevelName.Easy;
                StageSelectDirector.treeId = ETreeId.Spring_1;
                AddressableAssetManager.LoadScene(Constants.SceneName.SPRING_STAGE_SELECT_SCENE);
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
            float maxAxisLabelNum;
            var showPlus = false;

            if (challengeNumMax == 0) {
                maxAxisLabelNum = _MIN_AXIS_LABEL_NUM;
            } else if (challengeNumMax > _MAX_AXIS_LABEL_NUM) {
                maxAxisLabelNum = _MAX_AXIS_LABEL_NUM;
                showPlus = true;
            } else {
                maxAxisLabelNum = (float) Math.Ceiling(challengeNumMax / _MIN_AXIS_LABEL_NUM) * _MIN_AXIS_LABEL_NUM;
            }

            _graphAxisLabels[0].text = ((int) maxAxisLabelNum / 3).ToString();
            _graphAxisLabels[1].text = ((int) maxAxisLabelNum * 2 / 3).ToString();
            _graphAxisLabels[2].text = showPlus ? ((int) maxAxisLabelNum).ToString() : (int) maxAxisLabelNum + "+";

            _stageStatuses
                .Select((stageStatus, index) => (_graphBars[index], stageStatus.successNum, stageStatus.challengeNum))
                .ToList()
                .ForEach(args =>
                {
                    var (graphBar, successNum, challengeNum) = args;

                    graphBar.GetComponent<Image>().color = successNum > 0 ? Color.magenta : Color.gray;

                    var anchorMaxY = Mathf.Min(0.1f + 0.9f * challengeNum / maxAxisLabelNum, 1.0f);
                    graphBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchorMaxY);
                });
        }
    }
}

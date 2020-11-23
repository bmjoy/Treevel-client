using System.Collections.Generic;
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
        /// [UI] "ステージ一覧へ"
        /// </summary>
        [SerializeField] private Button _toStageSelectSceneButton;

        private StageStatus[] _stageStatuses;

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

            var clearStageNum = _stageStatuses.Select(stageStatus => stageStatus.successNum > 0 ? 1 : 0).Sum();
            var totalStageNum = _stageStatuses.Length;
            _clearStageNum.GetComponent<ClearStageNumDirector>().Setup(clearStageNum, totalStageNum, Color.magenta);

            SetupGraphBars();
        }

        private void SetupGraphBars()
        {
            var challengeNumMax = _stageStatuses.Select(stageStatus => stageStatus.challengeNum).Max();

            _stageStatuses
                .Select((stageStatus, index) => (_graphBars[index], stageStatus.successNum, stageStatus.challengeNum))
                .ToList()
                .ForEach(args =>
                {
                    var (graphBar, successNum, challengeNum) = args;

                    graphBar.GetComponent<Image>().color = successNum > 0 ? Color.magenta : Color.gray;

                    var anchorMaxY = 0.1f + 0.9f * challengeNum / challengeNumMax;
                    graphBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchorMaxY);
                });
        }
    }
}

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
        /// [UI] "ステージ一覧へ"
        /// </summary>
        [SerializeField] private Button _toStageSelectSceneButton;

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
            var stageStatuses = GameDataManager.GetStages(ETreeId.Spring_1)
                .Select(stage => StageStatus.Get(stage.TreeId, stage.StageNumber))
                .ToList();

            var clearStageNum = stageStatuses.Select(stageStatus => stageStatus.successNum > 0 ? 1 : 0).Sum();
            var totalStageNum = stageStatuses.Count;
            _clearStageNum.GetComponent<ClearStageNumDirector>().Setup(clearStageNum, totalStageNum, Color.magenta);
        }
    }
}

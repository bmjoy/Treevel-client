using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using UnityEngine;

namespace Treevel.Modules.StageSelectScene
{
    public class BranchController : LineControllerBase
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        /// <summary>
        /// 演出再生済みのブランチリスト
        /// </summary>
        public static List<string> animationPlayedBranches;

        protected override void SetSaveKey()
        {
            saveKey =
                $"{_treeId}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{startObject.GetComponent<StageController>().stageNumber}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponent<StageController>().stageNumber}";
        }

        protected override (Vector3, Vector3) GetEdgePointPosition()
        {
            var startPointPosition = startObject.transform.localPosition;
            var endPointPosition = endObject.transform.localPosition;
            return (startPointPosition, endPointPosition);
        }

        /// <summary>
        /// 枝の状態の更新
        /// </summary>
        public override void UpdateState()
        {
            var constraintStageData = GameDataManager.GetStage(_treeId, endObject.GetComponent<StageController>().stageNumber);
            var constraintStages = constraintStageData.ConstraintStages;
            if (constraintStages.Count == 0) return;

            if (constraintStages.Select(stageId => StageRecordService.Instance.Get(stageId)).All(stageData => stageData.IsCleared)) {
                if (!animationPlayedBranches.Contains(saveKey)) {
                    // TODO 枝解放時演出
                    Debug.Log($"{saveKey}が解放された");
                    animationPlayedBranches.Add(saveKey);
                }
            } else {
                // 未解放
                lineRenderer.startColor = new Color(0.2f, 0.2f, 0.7f);
                lineRenderer.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }
    }
}

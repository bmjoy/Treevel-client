using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
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

        /// <summary>
        /// 枝の状態の更新
        /// </summary>
        public override UniTask UpdateStateAsync()
        {
            if (constraintObjects.Length == 0) return UniTask.CompletedTask;

            if (constraintObjects.Select(gameObj => gameObj.GetComponent<StageController>())
                    .All(stageController => stageController.state == EStageState.Cleared)) {
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

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 枝の状態の保存
        /// </summary>
        public override void SaveState()
        {
            // branchStates[saveKey] = released;
        }
    }
}

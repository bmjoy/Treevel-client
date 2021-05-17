using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
{
    public class StageTreeController : TreeControllerBase
    {
        [SerializeField] private Image _treeImage;

        /// <summary>
        /// 画像に乗せるマテリアル(デフォルトでは何もなし)
        /// </summary>
        [SerializeField] private Material _material;

        /// <summary>
        /// 木全体を覆うオブジェクト(デフォルトでは表示されていない)
        /// </summary>
        [SerializeField] private GameObject _mask;

        /// <summary>
        /// 解放の制約となる木
        /// </summary>
        [SerializeField] private List<ETreeId> _constraintTrees;

        /// <summary>
        /// 解放の制約となる木のクリア条件
        /// </summary>
        private List<IClearTreeHandler> _constraintTreeClearHandlers;

        public override void Awake()
        {
            base.Awake();
            _constraintTreeClearHandlers = _constraintTrees.Select(id => id.GetClearTreeHandler()).ToList();
        }

        public override void UpdateState()
        {
            var treeData = GameDataManager.GetTreeData(treeId);

            // 解放条件がない場合、そのまま状態を反映
            if (treeData.constraintTrees.Count == 0) {
                state = clearHandler.GetTreeState();
                ReflectTreeState();
                return;
            }

            // 解放条件達成したか
            var isReleased = treeData.constraintTrees.All(constraint => {
                var constraintTreeData = GameDataManager.GetTreeData(constraint.treeId);
                var clearNumber = constraintTreeData.stages.Count(stageData => StageRecordService.Instance.Get(stageData).IsCleared);
                return clearNumber >= constraint.clearStageNumber;
            });

            // 非解放の場合も即反映
            if (!isReleased) {
                state = ETreeState.Unreleased;
            } else {
                state = clearHandler.GetTreeState();
            }

            // 状態の反映
            ReflectTreeState();
        }

        protected override void ReflectUnreleasedState()
        {
            _mask.SetActive(true);
            // グレースケール
            _treeImage.material = _material;
        }

        protected override void ReflectReleasedState()
        {
            _mask.SetActive(false);
        }

        protected override void ReflectClearedState()
        {
            _mask.SetActive(false);
            // アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            _mask.SetActive(false);
            // アニメーション
            Debug.Log($"{treeId} is all cleared.");
        }
    }
}

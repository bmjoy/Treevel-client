using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.StageSelectScene;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class LevelTreeController : TreeControllerBase
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ESeasonId _seasonId;

        [SerializeField] private Material _material;

        [SerializeField] private Button _button;

        public override void Awake()
        {
            base.Awake();
            _button.onClick.AsObservable()
                .Subscribe(_ => SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General))
                .AddTo(this);
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
                ReflectTreeState();
                return;
            }

            state = clearHandler.GetTreeState();
            // 木の解放演出が再生されたことがあったらそのまま反映する。
            if (LevelSelectDirector.Instance.releaseAnimationPlayedTrees.Contains(treeId)) {
                // 状態の反映
                ReflectTreeState();
            } else {
                // RoadControllerからの解放演出を待つ。まずは非解放状態にする
                ReflectUnreleasedState();
            }
        }

        protected override void ReflectUnreleasedState()
        {
            // グレースケール
            GetComponent<Image>().material = _material;
            _button.enabled = false;
        }

        protected override void ReflectReleasedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
        }

        protected override void ReflectClearedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
            // TODO: アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
            // TODO: アニメーション
            Debug.Log($"{treeId} is all cleared.");
        }

        /// <summary>
        /// 木が押されたとき
        /// </summary>
        public void TreeButtonDown()
        {
            StageSelectDirector.seasonId = _seasonId;
            StageSelectDirector.treeId = treeId;
            AddressableAssetManager.LoadScene(_seasonId.GetSceneName());
        }
    }
}

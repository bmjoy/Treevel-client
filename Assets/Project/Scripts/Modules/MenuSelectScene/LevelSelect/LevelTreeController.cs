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
    [RequireComponent(typeof(Button))]
    public class LevelTreeController : TreeControllerBase
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ESeasonId _seasonId;

        /// <summary>
        /// 木の画像
        /// </summary>
        [SerializeField] private Image _tree;

        /// <summary>
        /// 木の下の領域の画像
        /// </summary>
        [SerializeField] private Image _field;

        /// <summary>
        /// 未解放時のマテリアル
        /// </summary>
        [SerializeField] private Material _unreleasedMaterial;

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
            var isReleased = treeData.constraintTrees.All(constraintTreeId => GameDataManager.GetTreeData(constraintTreeId).GetClearTreeHandler().GetTreeState() >= ETreeState.Cleared);

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
            _tree.material = _unreleasedMaterial;
            _field.material = _unreleasedMaterial;
            _button.enabled = false;
        }

        protected override void ReflectReleasedState()
        {
            _tree.material = null;
            _field.material = null;
            _button.enabled = true;
        }

        protected override void ReflectClearedState()
        {
            _tree.material = null;
            _field.material = null;
            _button.enabled = true;
            // TODO: アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            _tree.material = null;
            _field.material = null;
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

        /// <summary>
        /// ワールド座標を取得
        /// </summary>
        public Vector3 GetTreeWorldPosition()
        {
            return gameObject.transform.localToWorldMatrix.MultiplyPoint3x4(gameObject.transform.localPosition);
        }
    }
}

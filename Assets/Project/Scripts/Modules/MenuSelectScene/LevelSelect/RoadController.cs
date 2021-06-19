using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    public class RoadController : LineControllerBase
    {
        public LevelTreeController EndObjectController { get; private set; }

        /// <summary>
        /// 解放時のテクスチャの占領比率(1.0だと全解放)
        /// </summary>
        private static readonly int _SHADER_PARAM_FILL_AMOUNT = Shader.PropertyToID("_fillAmount");

        /// <summary>
        /// LineRendererのマテリアル
        /// </summary>
        private Material _material;

        protected override void Awake()
        {
            base.Awake();
            EndObjectController = endObject.GetComponentInParent<LevelTreeController>();
            _material = lineRenderer.material;
        }

        protected override void SetSaveKey()
        {
            saveKey =
                $"{startObject.GetComponentInParent<LevelTreeController>().treeId}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponentInParent<LevelTreeController>().treeId}";
        }

        protected override (Vector3, Vector3) GetEdgePointPosition()
        {
            // 木の下の領域の中心座標は親オブジェクトの相対位置から計算する
            var startPointPosition = startObject.transform.parent.localPosition + startObject.transform.localPosition;
            var endPointPosition = endObject.transform.parent.localPosition + endObject.transform.localPosition;
            return (startPointPosition, endPointPosition);
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(saveKey);
        }

        /// <summary>
        /// 解放できるか
        /// </summary>
        /// <returns>
        /// 末端の木に解放条件がない(初期解放)ならfalse
        /// 末端の木に条件がありかつ条件クリアしたらtrue
        /// </returns>
        public bool CanBeReleased()
        {
            var treeData = GameDataManager.GetTreeData(EndObjectController.treeId);
            if (treeData.constraintTrees.Count == 0)
                return false;

            return EndObjectController.state == ETreeState.Released;
        }

        /// <summary>
        /// 道の状態の更新
        /// </summary>
        public override void UpdateState()
        {
            var endTreeData = GameDataManager.GetTreeData(EndObjectController.treeId);

            if (endTreeData.constraintTrees.Count == 0) {
                // 初期解放
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 1.0f);
                return;
            }

            if (EndObjectController.state == ETreeState.Unreleased) {
                // 未解放
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 0.0f);
            } else if (LevelSelectDirector.Instance.releaseAnimationPlayedRoads.Contains(saveKey)) {
                // 演出を再生したことがあればそのまま解放
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 1.0f);
            }
        }
    }
}

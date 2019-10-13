using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using System.Collections;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class WarpTileController : NormalTileController
    {
        // 相方のWarpTile
        [SerializeField, NonEditable] private GameObject _pairTile;

        // warpTileのエフェクト
        [SerializeField, NonEditable] private GameObject _warpTileEffectPrefab;

        public void Start() {
            var warpTileEffect = Instantiate(_warpTileEffectPrefab);
            warpTileEffect.transform.position = gameObject.transform.position;
            warpTileEffect.GetComponent<Animation>().Play();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="position"> 座標 </param>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <param name="pairTile"> 相方の WarpTile </param>
        public void Initialize(Vector2 position, int tileNum, GameObject pairTile)
        {
            base.Initialize(position, tileNum);
            name = TileName.WARP_TILE;
            this._pairTile = pairTile;
        }

        /// <inheritdoc />
        public override void HandlePanel(GameObject panel)
        {
            if (_pairTile.transform.childCount == 0) {
                // ワープ演出
                StartCoroutine(WarpPanel(panel));
            }
        }

        private IEnumerator WarpPanel(GameObject panel) {
            // warpTileの粒子アニメーション
            GetComponent<ParticleSystem>().Play();
            var anim = panel.GetComponent<Animation>();
            // panelがワープに入るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP);
            // アニメーションの終了を待つ
            while (anim.isPlaying) yield return new WaitForFixedUpdate();
            // panelの座標の更新
            panel.transform.parent = _pairTile.transform;
            panel.transform.position = _pairTile.transform.position;
            // panelがワープから戻るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP_REVERSE);
        }
    }
}

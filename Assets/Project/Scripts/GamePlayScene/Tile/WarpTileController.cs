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

        /// <summary>
        /// warpTile上のeffect
        /// </summary>
        private GameObject warpTileEffect;

        protected override void Awake() {
            base.Awake();
            warpTileEffect = transform.Find("warpTileEffectPrefab").gameObject;
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

        private void OnEnable()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        private void OnSucceed() {
            EndProcess();
        }

        private void OnFail() {
            EndProcess();
        }

        private void EndProcess() {
            // 粒子アニメーションの生成を止める
            GetComponent<ParticleSystem>().Stop();
            // すでに生成された粒子を消す
            GetComponent<ParticleSystem>().Clear();
            // warpTileEffectを止める
            warpTileEffect.GetComponent<Animation>().Stop();
        }

        /// <inheritdoc />
        public override void HandlePanel(GameObject panel)
        {
            base.HandlePanel(panel);
            // pair tileに子パネルがないならワープさせる
            if (!_pairTile.GetComponent<NormalTileController>().hasPanel) {
                // ワープ演出
                StartCoroutine(WarpPanel(panel));
            }
        }

        private IEnumerator WarpPanel(GameObject panel)
        {
            // warpTileの粒子アニメーション
            GetComponent<ParticleSystem>().Play();
            var anim = panel.GetComponent<Animation>();
            // panelがワープに入るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP);
            // アニメーションの終了を待つ
            while (anim.isPlaying) yield return new WaitForFixedUpdate();
            // panelの座標の更新
            LeavePanel(panel);
            panel.transform.parent = _pairTile.transform;
            _pairTile.GetComponent<NormalTileController>().hasPanel = true;
            panel.transform.position = _pairTile.transform.position;
            // panelがワープから戻るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP_REVERSE);
        }
    }
}

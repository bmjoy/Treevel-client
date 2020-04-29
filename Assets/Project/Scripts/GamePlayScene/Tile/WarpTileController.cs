using System.Collections;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class WarpTileController : AbstractTileController
    {
        // 相方のWarpTile
        [SerializeField, NonEditable] private GameObject _pairTile;

        /// <summary>
        /// warpTile上のeffect
        /// </summary>
        private GameObject _warpTileEffect;

        /// <summary>
        /// warpできるかどうか
        /// </summary>
        private bool _warpEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            panelHandler = new WarpTileBottleHandler(this);
            _warpTileEffect = transform.Find("WarpTileEffectPrefab").gameObject;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <param name="pairTile"> 相方の WarpTile </param>
        public void Initialize(int tileNum, GameObject pairTile)
        {
            base.Initialize(tileNum);
            #if UNITY_EDITOR
            name = TileName.WARP_TILE;
            #endif
            _pairTile = pairTile;
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

        private void OnSucceed()
        {
            EndProcess();
        }

        private void OnFail()
        {
            EndProcess();
        }

        private void EndProcess()
        {
            // 粒子アニメーションの生成を止める
            GetComponent<ParticleSystem>().Stop();
            // すでに生成された粒子を消す
            GetComponent<ParticleSystem>().Clear();
            // warpTileEffectを止める
            _warpTileEffect.GetComponent<Animation>().Stop();
        }

        private IEnumerator WarpPanel(GameObject panel)
        {
            var pairTileController = _pairTile.GetComponent<WarpTileController>();

            // panelをフリックできないようにする
            panel.GetComponent<FlickGesture>().enabled = false;

            // 相方を一時的にワープ不能にする
            pairTileController._warpEnabled = false;

            // warpTileの粒子アニメーション
            GetComponent<ParticleSystem>().Play();
            var anim = panel.GetComponent<Animation>();
            // panelがワープに入るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP);
            // アニメーションの終了を待つ
            while (anim.isPlaying) yield return new WaitForFixedUpdate();

            // ボトルを移動する
            BoardManager.SetPanel(panel.GetComponent<AbstractBottleController>(), pairTileController.TileNumber);

            // panelがワープから戻るアニメーション
            anim.Play(AnimationClipName.PANEL_WARP_REVERSE);
            // アニメーションの終了を待つ
            while (anim.isPlaying) yield return new WaitForFixedUpdate();

            // 相方のワープ状態を戻す
            pairTileController._warpEnabled = true;

            // panelをフリックできるようにする
            panel.GetComponent<FlickGesture>().enabled = true;
        }

        private bool CanWarp()
        {
            return _warpEnabled && BoardManager.GetPanel(_pairTile.GetComponent<AbstractTileController>().TileNumber) == null;
        }

        private void StartWarp(GameObject panel)
        {
            StartCoroutine(WarpPanel(panel));
        }

        private sealed class WarpTileBottleHandler : DefaultBottleHandler
        {
            private readonly WarpTileController _parent;

            public WarpTileBottleHandler(WarpTileController parent)
            {
                _parent = parent;
            }

            public override void OnPanelEnter(GameObject panel)
            {
                // pair tileに子ボトルがないならワープさせる
                if (_parent.CanWarp()) {
                    // ワープ演出
                    _parent.StartWarp(panel);
                }
            }
        }
    }
}

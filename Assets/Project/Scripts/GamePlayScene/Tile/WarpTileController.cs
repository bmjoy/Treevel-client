using System.Collections;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    [RequireComponent(typeof(Animator))]
    public class WarpTileController : AbstractTileController
    {
        // 相方のWarpTile
        [SerializeField, NonEditable] private GameObject _pairTile;

        /// <summary>
        /// ワープするボトルをアタッチするオブジェクト
        /// </summary>
        private GameObject _warpTarget;

        /// <summary>
        /// warpできるかどうか
        /// </summary>
        private bool _warpEnabled = true;

        private const string _ANIMATOR_PARAM_TRIGGER_BOTTLEIN = "BottleIn";
        private const string _ANIMATOR_PARAM_TRIGGER_BOTTLEOUT = "BottleOut";
        private static readonly int _ANIMATOR_NAME_HASH_BOTTLEIN = Animator.StringToHash("WarpTile@bottle_in");
        private static readonly int _ANIMATOR_NAME_HASH_BOTTLEOUT = Animator.StringToHash("WarpTile@bottle_out");

        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new WarpTileBottleHandler(this);
            _warpTarget = transform.Find("WarpTarget").gameObject;
            _animator = GetComponent<Animator>();
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
            _animator.speed = 0;
        }

        private IEnumerator WarpBottle(GameObject bottle)
        {
            var pairTileController = _pairTile.GetComponent<WarpTileController>();

            // bottleをフリックできないようにする
            bottle.GetComponent<FlickGesture>().enabled = false;

            // 相方を一時的にワープ不能にする
            pairTileController._warpEnabled = false;

            // 元々の親を一時保存
            var bottleParent = bottle.transform.parent;
            var bottleScale = bottle.transform.localScale;

            // 相方のワープオブジェクトの大きさをゼロにしておく
            var pairTileWarpObject = _pairTile.transform.Find("WarpTarget");
            pairTileWarpObject.transform.localScale = Vector3.zero;

            // warpTileの粒子アニメーション
            GetComponent<ParticleSystem>().Play();

            // ボトルをWarpTargetの子オブジェクトに
            bottle.transform.SetParent(_warpTarget.transform);
            // bottleがワープに入るアニメーション
            _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLEIN);
            // アニメーションの終了を待つ
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ANIMATOR_NAME_HASH_BOTTLEIN);

            // ボトルを移動する
            BoardManager.Instance.Move(bottle.GetComponent<DynamicBottleController>(), pairTileController.TileNumber, null, immediately: true);

            // 相方のWarpTargetの子オブジェクトに
            bottle.transform.SetParent(pairTileWarpObject, false);

            // bottleがワープから戻るアニメーション
            var pairAnimator = _pairTile.GetComponent<Animator>();
            pairAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLEOUT);

            // アニメーション開始直後にスケール戻す
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => pairAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ANIMATOR_NAME_HASH_BOTTLEOUT);

            // ボトルの親オブジェクトを戻す
            bottle.transform.SetParent(bottleParent);

            // 相方のワープ状態を戻す
            pairTileController._warpEnabled = true;

            // bottleをフリックできるようにする
            bottle.GetComponent<FlickGesture>().enabled = true;
        }

        private bool CanWarp()
        {
            return _warpEnabled && BoardManager.Instance.GetBottle(_pairTile.GetComponent<AbstractTileController>().TileNumber) == null;
        }

        private void StartWarp(GameObject bottle)
        {
            StartCoroutine(WarpBottle(bottle));
        }

        private sealed class WarpTileBottleHandler : DefaultBottleHandler
        {
            private readonly WarpTileController _parent;

            public WarpTileBottleHandler(WarpTileController parent)
            {
                _parent = parent;
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                // pair tileに子ボトルがないならワープさせる
                if (_parent.CanWarp()) {
                    // ワープ演出
                    _parent.StartWarp(bottle);
                }
            }
        }
    }
}

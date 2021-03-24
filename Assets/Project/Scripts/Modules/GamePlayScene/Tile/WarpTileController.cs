using Cysharp.Threading.Tasks;
using TouchScript.Gestures;
using Treevel.Common.Attributes;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    [RequireComponent(typeof(Animator))]
    public class WarpTileController : TileControllerBase
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

        /// <summary>
        /// ワープするボトルの情報を保存用
        /// </summary>
        private WarpBottleInfo _warpBottleInfo;

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

            // ボトルが入るアニメーション再生終了後の処理
            _animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_NAME_HASH_BOTTLEIN)
                .Subscribe(_ => {
                    OnExitBottleInAnimationAsync();
                })
                .AddTo(this);

            // ボトルが出てくるアニメーション再生終了後の処理
            _animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_NAME_HASH_BOTTLEOUT)
                .Subscribe(_ => {
                    OnExitBottleOutAnimationAsync();
                })
                .AddTo(this);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <param name="pairTile"> 相方の WarpTile </param>
        public void Initialize(int tileNum, GameObject pairTile)
        {
            Initialize(tileNum);
            #if UNITY_EDITOR
            name = Constants.TileName.WARP_TILE;
            #endif
            _pairTile = pairTile;
        }

        private void OnEnable()
        {
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => EndProcess()).AddTo(this);
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

        private void WarpIn(GameObject bottle)
        {
            var pairTileController = _pairTile.GetComponent<WarpTileController>();

            // ワープするボトルの情報設定
            var parent = bottle.transform.parent;
            _warpBottleInfo = new WarpBottleInfo {
                gameObject = bottle,
                originalParent = parent,
            };

            // 相方にも情報を渡す
            pairTileController._warpBottleInfo = new WarpBottleInfo {
                gameObject = bottle,
                originalParent = parent,
            };

            // bottleをフリックできないようにする
            bottle.GetComponent<FlickGesture>().enabled = false;

            // 相方を一時的にワープ不能にする
            pairTileController._warpEnabled = false;

            // warpTileの粒子アニメーション
            GetComponent<ParticleSystem>().Play();

            // ボトルをWarpTargetの子オブジェクトに
            bottle.transform.SetParent(_warpTarget.transform);

            // bottleがワープに入るアニメーション
            _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLEIN);
        }

        /// <summary>
        /// ボトルを吸い込むアニメーション完了後の処理
        /// </summary>
        private async void OnExitBottleInAnimationAsync()
        {
            // 呼ばれたタイミングは僅かですがまだ再生終わっていないので待つ
            await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ANIMATOR_NAME_HASH_BOTTLEIN);

            var pairTileController = _pairTile.GetComponent<WarpTileController>();
            var bottle = _warpBottleInfo.gameObject;

            // ボードマネージャーにワープ先のタイルを登録する。登録したらOnBottleEnterでワープ先でのアニメーションを発動する
            BoardManager.Instance.RegisterBottle(bottle.GetComponent<BottleControllerBase>(), pairTileController.TileNumber);
        }

        private void WarpOut()
        {
            var bottle = _warpBottleInfo.gameObject;

            // WarpTargetの子オブジェクトに
            bottle.transform.SetParent(_warpTarget.transform, false);

            // bottleがワープから戻るアニメーション再生
            _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLEOUT);
        }

        /// <summary>
        /// ボトルを放り出すアニメーション完了後の処理
        /// </summary>
        private async void OnExitBottleOutAnimationAsync()
        {
            // 呼ばれたタイミングは僅かですがまだ再生終わっていない
            await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ANIMATOR_NAME_HASH_BOTTLEOUT);

            var bottle = _warpBottleInfo.gameObject;

            // ボトルの親オブジェクトを戻す
            bottle.transform.SetParent(_warpBottleInfo.originalParent);

            // 相方のワープ状態を戻す
            _warpEnabled = true;

            // bottleをフリックできるようにする
            bottle.GetComponent<FlickGesture>().enabled = true;
        }

        private bool CanWarp()
        {
            return _warpEnabled &&
                   BoardManager.Instance.GetBottle(_pairTile.GetComponent<TileControllerBase>().TileNumber) == null;
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
                // ワープ先にボトルがなければワープ開始
                if (_parent.CanWarp()) {
                    _parent.WarpIn(bottle);
                }

                // 相方のタイルからボトルを渡された場合
                if (!_parent._warpEnabled && _parent._warpBottleInfo.gameObject == bottle) {
                    _parent.WarpOut();
                }
            }
        }

        /// <summary>
        /// ワープするボトルの情報を一時保存するための構造体
        /// </summary>
        private struct WarpBottleInfo
        {
            public GameObject gameObject;
            public Transform originalParent;
        }
    }
}

using Cysharp.Threading.Tasks;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Common.Managers;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    [RequireComponent(typeof(Animator))]
    public class SpiderwebTileController : TileControllerBase
    {
        [SerializeField] private SpriteRenderer _spiderwebLayer;

        private string _spriteName;

        private static readonly int _ANIMATOR_NAME_HASH_GAME_START = Animator.StringToHash("SpiderwebTile@game_start");

        public Animator animator;

        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new SpiderwebTileBottleHandler(this);
            animator = GetComponent<Animator>();
            // ゲーム開始時の表示後の処理
            animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_NAME_HASH_GAME_START)
                .Subscribe(_ => SetOnBottleSprite()).AddTo(this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => animator.enabled = false).AddTo(this);
        }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);

            spriteRendererUnifier.SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileData.number));
            // 場所に応じて画像を変更する
            _spriteName = tileData.number switch {
                1 =>
                    // 左上
                    Constants.Address.SPIDERWEB_TILE_TOP_LEFT_SPRITE,
                3 =>
                    // 右上
                    Constants.Address.SPIDERWEB_TILE_TOP_RIGHT_SPRITE,
                13 =>
                    // 左下
                    Constants.Address.SPIDERWEB_TILE_BOTTOM_LEFT_SPRITE,
                15 =>
                    // 右下
                    Constants.Address.SPIDERWEB_TILE_BOTTOM_RIGHT_SPRITE,
                _ =>
                    // それ以外
                    Constants.Address.SPIDERWEB_TILE_SPRITE,
            };

            _spiderwebLayer.sprite = AddressableAssetManager.GetAsset<Sprite>(_spriteName);

            #if UNITY_EDITOR
            name = Constants.TileName.SPIDERWEB_TILE;
            #endif
        }

        /// <summary>
        /// 崩れた蜘蛛の巣画像に変更する
        /// </summary>
        private void SetOnBottleSprite()
        {
            if (!_spriteName.Contains(Constants.Address.SPIDERWEB_TILE_ON_BOTTLE_PREFIX)) _spriteName += Constants.Address.SPIDERWEB_TILE_ON_BOTTLE_PREFIX;

            _spiderwebLayer.sprite = AddressableAssetManager.GetAsset<Sprite>(_spriteName);
        }

        private sealed class SpiderwebTileBottleHandler : DefaultBottleHandler
        {
            private readonly SpiderwebTileController _parent;

            private static readonly int _ANIMATOR_PARAM_TRIGGER_GAME_START = Animator.StringToHash("GameStart");
            private static readonly int _ANIMATOR_PARAM_TRIGGER_BOTTLE_STOP = Animator.StringToHash("BottleStop");
            private static readonly int _ANIMATOR_NAME_HASH_BOTTLE_STOP = Animator.StringToHash("SpiderwebTile@bottle_stop");

            public SpiderwebTileBottleHandler(SpiderwebTileController parent)
            {
                _parent = parent;
            }

            public override void OnGameStart(GameObject bottle)
            {
                if (bottle == null) {
                    // 登場アニメーション
                    _parent.animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_GAME_START);
                    return;
                }

                // ゲーム開始時の時点でSpiderwebTile上にBottleがある場合
                _parent.SetOnBottleSprite();

                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<DynamicBottleController>() == null) return;

                StopBottleAsync(bottle).Forget();
            }

            private async UniTask StopBottleAsync(GameObject bottle)
            {
                var bottleController = bottle.GetComponent<DynamicBottleController>();
                bottleController.IsMovable = false;
                // アニメーションの開始
                _parent.animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLE_STOP);

                // Animation Stateがidleから変わるのを待つ
                await Observable.TimerFrame(0);
                // アニメーション終了まで待つ
                await UniTask.WaitUntil(() => _parent.animator != null && _parent.animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ANIMATOR_NAME_HASH_BOTTLE_STOP);

                bottleController.IsMovable = true;
            }
        }
    }
}

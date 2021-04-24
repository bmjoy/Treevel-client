using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class IceTileController : TileControllerBase
    {
        private SpriteRendererUnifier _spriteRendererUnifier;
        [SerializeField] private SpriteRenderer _iceLayer;

        protected override void Awake()
        {
            base.Awake();
            _spriteRendererUnifier = GetComponent<SpriteRendererUnifier>();
            GamePlayDirector.Instance.OpeningAnimationStart.Subscribe(_ => {
                _spriteRendererUnifier.enabled = true;
                _iceLayer.enabled = true;
            }).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => _iceLayer.material = AddressableAssetManager.GetAsset<Material>(Constants.Address.ICE_LAYER_MATERIAL)).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                #if UNITY_EDITOR
                _iceLayer.material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
                #else
                _iceLayer.material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
                #endif
            }).AddTo(this);
        }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);

            _spriteRendererUnifier.SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileData.number));

            #if UNITY_EDITOR
            name = Constants.TileName.ICE_TILE;
            #endif

            bottleHandler = new IceTileBottleHandler(TileNumber);
        }

        private sealed class IceTileBottleHandler : DefaultBottleHandler
        {
            private readonly int _tileNumber;

            public IceTileBottleHandler(int tileNumber)
            {
                _tileNumber = tileNumber;
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                // 方向を持たない場合は何もしない
                if (direction == null) return;

                var dynamicBottleController = bottle.GetComponent<DynamicBottleController>();

                // 移動できるボトルではなかったら何もしない
                if (dynamicBottleController == null) return;

                var x = direction.Value.x;
                var y = direction.Value.y;

                int targetTileNum;

                if (x == 0) {
                    // 縦方向の移動の場合
                    targetTileNum = _tileNumber + 3 * y;
                } else if (y == 0) {
                    // 横方向の移動の場合
                    targetTileNum = _tileNumber + x;
                } else {
                    Debug.LogError("invalid direction");
                    return;
                }

                BoardManager.Instance.MoveAsync(dynamicBottleController, targetTileNum, direction.Value);
            }
        }
    }
}

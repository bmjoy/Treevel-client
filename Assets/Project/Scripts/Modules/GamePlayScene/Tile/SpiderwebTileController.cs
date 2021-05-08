using Cysharp.Threading.Tasks;
using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Common.Managers;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class SpiderwebTileController : TileControllerBase
    {
        /// <summary>
        /// 蜘蛛の巣によってボトルが止まる秒数
        /// </summary>
        private const int _BIND_TIME = 2;

        [SerializeField] private SpriteRenderer _spiderwebLayer;

        private string _spriteName;
        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new SpiderwebTileBottleHandler();
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

        private sealed class SpiderwebTileBottleHandler : DefaultBottleHandler
        {
            public override void OnGameStart(GameObject bottle)
            {
                if (bottle == null) return;
                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<DynamicBottleController>() == null) return;

                StopBottleAsync(bottle).Forget();
            }

            private static async UniTask StopBottleAsync(GameObject bottle)
            {
                Debug.Log("蜘蛛の巣タイルで拘束");
                bottle.GetComponent<DynamicBottleController>().IsMovable = false;

                // 引数は milliseconds
                await UniTask.Delay(_BIND_TIME * 1000);

                bottle.GetComponent<DynamicBottleController>().IsMovable = true;
                Debug.Log("蜘蛛の巣タイルから解放");
            }
        }
    }
}

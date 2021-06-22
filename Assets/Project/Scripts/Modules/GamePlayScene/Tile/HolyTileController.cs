using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class HolyTileController : TileControllerBase
    {
        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new HolyTileBottleHandler();
        }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileData.number));
            #if UNITY_EDITOR
            name = Constants.TileName.HOLY_TILE;
            #endif
        }

        private sealed class HolyTileBottleHandler : DefaultBottleHandler
        {
            public override void OnGameStart(GameObject bottle)
            {
                if (bottle == null) return;
                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<BottleControllerBase>() == null) return;

                // 親ボトルを無敵状態にする
                bottle.GetComponent<BottleControllerBase>().isInvincibleByHoly.Value = true;
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // 親ボトルを無敵状態から元に戻す
                bottle.GetComponent<BottleControllerBase>().isInvincibleByHoly.Value = false;
            }
        }
    }
}

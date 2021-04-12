using Treevel.Common.Components;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class NormalTileController : TileControllerBase
    {
        public override void Initialize(int tileNum)
        {
            base.Initialize(tileNum);
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileNum));
            #if UNITY_EDITOR
            name = Constants.TileName.NORMAL_TILE + tileNum;
            #endif
        }
    }
}

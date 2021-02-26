using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class NormalTileController : AbstractTileController
    {
        public override bool RunOnBottleEnterAtInit => false;

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);
            color = tileData.color;
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(tileData.color.GetTileAddress()));
            #if UNITY_EDITOR
            name = Constants.TileName.GOAL_TILE + tileData.number;
            #endif
        }

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

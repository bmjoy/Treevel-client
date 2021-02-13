using Treevel.Common.Components;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEditor;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class NormalTileController : AbstractTileController
    {
        public override bool RunOnBottleEnterAtInit => false;

        public override void Initialize(int tileNum)
        {
            base.Initialize(tileNum);
            SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileNum));
            #if UNITY_EDITOR
            name = Constants.TileName.NORMAL_TILE + tileNum;
            #endif
        }

        /// <summary>
        /// タイルの画像を設定する
        /// </summary>
        /// <param name="sprite">タイル画像</param>
        public void SetSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<SpriteRendererUnifier>().Unify();
        }
    }
}

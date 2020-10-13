using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class NormalTileController : AbstractTileController
    {
        public override bool RunOnBottleEnterAtInit => false;

        /// <summary>
        /// タイルの画像を設定する
        /// </summary>
        /// <param name="sprite">タイル画像</param>
        public void SetSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}

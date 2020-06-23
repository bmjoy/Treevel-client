using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NormalTileController : AbstractTileController
    {
        /// <summary>
        /// タイルの画像を設定する
        /// </summary>
        /// <param name="sprite">タイル画像</param>
        public void SetSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            InitializeSprite();
        }
    }
}

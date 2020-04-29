using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class AbstractTileController : MonoBehaviour
    {
        /// <summary>
        /// タイルの番号
        /// </summary>
        public int TileNumber
        {
            get;
            private set;
        }

        protected IBottleHandler bottleHandler = new DefaultBottleHandler();

        protected virtual void Awake()
        {
            InitializeSprite();
        }

        /// <summary>
        /// 画像の大きさを調整
        /// </summary>
        protected void InitializeSprite()
        {
            // タイル画像のサイズを取得
            var tileWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var tileHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // タイルの初期設定
            transform.localScale = new Vector2(TileSize.WIDTH / tileWidth, TileSize.HEIGHT / tileHeight);

            GetComponent<Renderer>().sortingLayerName = SortingLayerName.TILE;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        public virtual void Initialize(int tileNum)
        {
            TileNumber = tileNum;
        }

        public void OnBottleEnter(GameObject bottle)
        {
            bottleHandler.OnBottleEnter(bottle);
        }

        public void OnBottleExit(GameObject bottle)
        {
            bottleHandler.OnBottleExit(bottle);
        }

        protected interface IBottleHandler
        {
            void OnBottleEnter(GameObject bottle);
            void OnBottleExit(GameObject bottle);
        }

        // 何もしないボトルハンドラー
        protected class DefaultBottleHandler : IBottleHandler
        {
            public virtual void OnBottleEnter(GameObject bottle) {}

            public virtual void OnBottleExit(GameObject bottle) {}
        }
    }
}

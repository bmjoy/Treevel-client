using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class AbstractTile : MonoBehaviour
    {
        /// <summary>
        /// タイルの番号
        /// </summary>
        private int _tileNum;
        public int TileNumber => _tileNum;

        protected IPanelHandler _panelHandler = new DefaultPanelHandler();

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
            _tileNum = tileNum;
        }

        public void OnPanelEnter(GameObject panel)
        {
            _panelHandler.OnPanelEnter(panel);
        }

        public void OnPanelExit(GameObject panel)
        {
            _panelHandler.OnPanelExit(panel);
        }

        protected interface IPanelHandler
        {
            void OnPanelEnter(GameObject panel);
            void OnPanelExit(GameObject panel);
        }

        // 何もしないパネルハンドラー
        protected class DefaultPanelHandler : IPanelHandler
        {
            public virtual void OnPanelEnter(GameObject panel) {}

            public virtual void OnPanelExit(GameObject panel) {}
        }
    }
}

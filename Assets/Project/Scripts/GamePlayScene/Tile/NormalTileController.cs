using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NormalTileController : MonoBehaviour
    {
        /// <summary>
        /// タイルの番号
        /// </summary>
        private int _tileNum;
        public int TileNumber => _tileNum;

        /// <summary>
        /// タイルに乗っているパネルの有無
        /// </summary>
        public bool hasPanel = false;

        protected virtual void Awake()
        {
            var tileWidth = GetComponent<SpriteRenderer>().size.x;
            var tileHeight = GetComponent<SpriteRenderer>().size.y;
            transform.localScale = new Vector2(TileSize.WIDTH / tileWidth, TileSize.HEIGHT / tileHeight);
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.TILE;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        public virtual void Initialize(int tileNum)
        {
            #if UNITY_EDITOR
            name = TileName.NORMAL_TILE;
            #endif
            _tileNum = tileNum;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// このタイルに任意のパネルが移動してきた場合の処理
        /// </summary>
        /// <param name="panel"></param>
        public virtual void HandlePanel(GameObject panel)
        {
            hasPanel = true;
        }

        /// <summary>
        /// このタイル上のパネルが移動した場合の処理
        /// </summary>
        /// <param name="panel"></param>
        public void LeavePanel(GameObject panel)
        {
            hasPanel = false;
        }

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

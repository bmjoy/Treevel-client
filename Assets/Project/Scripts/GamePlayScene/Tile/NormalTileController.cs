using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NormalTileController : MonoBehaviour
    {
        /// <summary>
        /// 右にあるタイル
        /// </summary>
        [NonEditable] public GameObject rightTile;

        /// <summary>
        /// 左にあるタイル
        /// </summary>
        [NonEditable] public GameObject leftTile;

        /// <summary>
        /// 上にあるタイル
        /// </summary>
        [NonEditable] public GameObject upperTile;

        /// <summary>
        /// 下にあるタイル
        /// </summary>
        [NonEditable] public GameObject lowerTile;

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
        /// <param name="position"> 座標 </param>
        /// <param name="tileNum"> タイルの番号 </param>
        public virtual void Initialize(Vector2 position, int tileNum)
        {
            transform.position = position;
            name = TileName.NORMAL_TILE;
            _tileNum = tileNum;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// タイルの上下左右のタイルへの参照を入れる
        /// </summary>
        /// <param name="rightTile"> 右のタイル </param>
        /// <param name="leftTile"> 左のタイル </param>
        /// <param name="upperTile"> 上のタイル </param>
        /// <param name="lowerTile"> 下のタイル </param>
        public void MakeRelation(GameObject rightTile, GameObject leftTile, GameObject upperTile, GameObject lowerTile)
        {
            this.rightTile = rightTile;
            this.leftTile = leftTile;
            this.upperTile = upperTile;
            this.lowerTile = lowerTile;
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

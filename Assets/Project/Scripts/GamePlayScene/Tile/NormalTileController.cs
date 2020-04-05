using Project.Scripts.Utils;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        }

        /// <summary>
        /// タイルの番号と一致していたら自身を返す
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <returns></returns>
        public GameObject GetTile(int tileNum)
        {
            return _tileNum == tileNum ? gameObject : null;
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
        /// 番号に合わせた画像に変更
        /// </summary>
        /// <param name="panelNum"> このタイルをゴールとするパネルの番号 </param>
        public async void SetSprite(int panelNum)
        {
            // TODO 事前にロードを行って、ここはセットだけ
            var sprite = await Addressables.LoadAssetAsync<Sprite>($"{Address.NUMBER_TILE_SPRITE_PREFIX}{panelNum}").Task;
            if (sprite != null) GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}

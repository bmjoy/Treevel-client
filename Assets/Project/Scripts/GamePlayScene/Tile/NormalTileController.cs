using UnityEngine;
using Project.Scripts.Utils.Attribute;
using Project.Scripts.Utils.Definitions;
using JetBrains.Annotations;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NormalTileController : MonoBehaviour
    {
        [CanBeNull, NonEditable] public GameObject rightTile;
        [CanBeNull, NonEditable] public GameObject leftTile;
        [CanBeNull, NonEditable] public GameObject upperTile;
        [CanBeNull, NonEditable] public GameObject lowerTile;

        private int tileNum;

        private void Awake()
        {
            var tileWidth = GetComponent<SpriteRenderer>().size.x;
            var tileHeight = GetComponent<SpriteRenderer>().size.y;
            transform.localScale = new Vector2(TileSize.WIDTH / tileWidth, TileSize.HEIGHT / tileHeight);
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.TILE;
        }

        public virtual void Initialize(Vector2 position, int tileNum)
        {
            transform.position = position;
            name = TileName.NORMAL_TILE;
            this.tileNum = tileNum;
        }

        /* 自身のタイル番号に該当した番号が与えられたら，自身の GameObject を返す */
        public GameObject GetTile(int tileNum)
        {
            if (this.tileNum == tileNum) {
                return gameObject;
            }

            return null;
        }

        // 自身タイルの上下左右のタイルへの参照を入れる
        public void MakeRelation(GameObject rightTile, GameObject leftTile, GameObject upperTile, GameObject lowerTile)
        {
            this.rightTile = rightTile;
            this.leftTile = leftTile;
            this.upperTile = upperTile;
            this.lowerTile = lowerTile;
        }

        /* パネルがタイルに移動してきたときの処理 */
        public virtual void HandlePanel(GameObject panel)
        {
        }
    }
}

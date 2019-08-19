﻿using UnityEngine;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NormalTileController : MonoBehaviour
    {
        [NonEditable] public GameObject _rightTile;
        [NonEditable] public GameObject _leftTile;
        [NonEditable] public GameObject _upperTile;
        [NonEditable] public GameObject _lowerTile;

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
            this._rightTile = rightTile;
            this._leftTile = leftTile;
            this._upperTile = upperTile;
            this._lowerTile = lowerTile;
        }

        /* パネルがタイルに移動してきたときの処理 */
        public virtual void HandlePanel(GameObject panel)
        {
        }
    }
}

using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using System.Collections.Generic;
using Project.Scripts.GameDatas;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObject<TileGenerator>
    {
        [SerializeField] private GameObject _normalTilePrefab;
        [SerializeField] private GameObject _warpTilePrefab;

        private readonly GameObject[,] _tiles = new GameObject[StageSize.ROW, StageSize.COLUMN];

        public void CreateTiles(ICollection<TileData> tileDatas)
        {
            foreach (TileData tileData in tileDatas) {
                switch (tileData.type) {
                    case ETileType.Normal:
                        CreateNormalTile(tileData.number);
                        break;
                    case ETileType.Warp:
                        CreateWarpTiles(tileData.number, tileData.pairNumber);
                        break;
                }
            }

            // Normal Tile は明示的に作らなくても，一括作成可能
            CreateNormalTiles();
        }

        /// <summary>
        /// 普通タイルの作成
        /// </summary>
        public void CreateNormalTiles()
        {
            for (var tileNum = 1; tileNum <= StageSize.TILE_NUM; tileNum++) {
                // 行 (0~4)
                var row = (tileNum - 1) / StageSize.COLUMN;
                // 列 (0~2)
                var column = (tileNum - 1) % StageSize.COLUMN;

                // 既に他タイルを作成している場合はスルー
                if (_tiles[row, column] != null) continue;

                var tile = Instantiate(_normalTilePrefab);

                var tilePosition = GetTilePosition(tileNum);

                tile.GetComponent<NormalTileController>().Initialize(tilePosition, tileNum);

                SetTile(tileNum, tile);
            }

            // タイルの位置関係を作成する
            MakeRelations(_tiles);
        }

        public void CreateNormalTile(int tileNum)
        {
            var normalTile = Instantiate(_normalTilePrefab);

            var normalTilePosition = GetTilePosition(tileNum);

            normalTile.GetComponent<NormalTileController>().Initialize(normalTilePosition, tileNum);

            SetTile(tileNum, normalTile);
        }

        /// <summary>
        /// ワープタイルの作成 (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        public void CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTile = Instantiate(_warpTilePrefab);
            var secondTile = Instantiate(_warpTilePrefab);

            var firstTilePosition = GetTilePosition(firstTileNum);
            var secondTilePosition = GetTilePosition(secondTileNum);

            firstTile.GetComponent<WarpTileController>().Initialize(firstTilePosition, firstTileNum, secondTile);
            secondTile.GetComponent<WarpTileController>().Initialize(secondTilePosition, secondTileNum, firstTile);

            SetTile(firstTileNum, firstTile);
            SetTile(secondTileNum, secondTile);
        }

        /// <summary>
        /// タイルの座標を計算し，取得
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <returns></returns>
        private static Vector2 GetTilePosition(int tileNum)
        {
            // 処理計算では0から扱いたい
            tileNum -= 1;
            // 行 (0~4)
            var row = tileNum / StageSize.COLUMN;
            // 列 (0~2)
            var column = tileNum % StageSize.COLUMN;
            // 作成するタイルのx,y座標
            var positionX = TileSize.WIDTH * (column - 1);
            var positionY = TileSize.HEIGHT * (StageSize.ROW / 2 - row);

            return new Vector2(positionX, positionY);
        }

        /// <summary>
        /// タイルを配列に格納
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <param name="tile"> タイル </param>
        private void SetTile(int tileNum, GameObject tile)
        {
            // 行 (0~4)
            var row = (tileNum - 1) / StageSize.COLUMN;
            // 列 (0~2)
            var column = (tileNum - 1) % StageSize.COLUMN;

            _tiles[row, column] = tile;
        }

        /// <summary>
        /// タイルの上下左右のタイルへの参照を入れる
        /// </summary>
        /// <param name="tiles"></param>
        private static void MakeRelations(GameObject[,] tiles)
        {
            var row = tiles.GetLength(0);
            var column = tiles.GetLength(1);

            for (var i = 0; i < row; i++) {
                for (var j = 0; j < column; j++) {
                    var rightTile = j + 1 == column ? null : tiles[i, j + 1];
                    var leftTile = j == 0 ? null : tiles[i, j - 1];
                    var upperTile = i == 0 ? null : tiles[i - 1, j];
                    var lowerTile = i + 1 == row ? null : tiles[i + 1, j];
                    // タイルオブジェクトのスクリプトに上下左右のタイルオブジェクトを格納する
                    tiles[i, j].GetComponent<NormalTileController>()
                    .MakeRelation(rightTile, leftTile, upperTile, lowerTile);
                }
            }
        }
    }
}

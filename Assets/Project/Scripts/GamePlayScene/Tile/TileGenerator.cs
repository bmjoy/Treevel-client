using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObject<TileGenerator>
    {
        private readonly GameObject[,] _tiles = new GameObject[StageSize.ROW, StageSize.COLUMN];

        private void Awake()
        {
            // シーンに配置したタイルを初期化
            for (int i = 0; i < StageSize.ROW ; ++i) {
                for (int j = 0; j < StageSize.COLUMN; ++j) {
                    var tileNum = i * StageSize.COLUMN + j + 1;
                    var currTile = transform.Find($"NormalTile{tileNum}").gameObject;

                    _tiles[i, j] = currTile;

                    // initialize tile
                    currTile.GetComponent<NormalTileController>().Initialize(GetTilePosition(tileNum), tileNum);

                    // show sprite
                    currTile.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        public async Task CreateTiles(ICollection<TileData> tileDatas)
        {
            foreach (var tileData in tileDatas) {
                switch (tileData.type) {
                    case ETileType.Normal:
                        break;
                    case ETileType.Warp:
                        await CreateWarpTiles(tileData.number, tileData.pairNumber);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // タイルの位置関係を作成する
            MakeRelations(_tiles);
        }

        /// <summary>
        /// ワープタイルの作成 (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        private async Task CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;
            var secondTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;

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
            var positionX = TileSize.WIDTH * (column - StageSize.COLUMN / 2);
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

            // すでにあったら削除
            if (_tiles[row, column] != null) {
                Destroy(_tiles[row, column]);
            }

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

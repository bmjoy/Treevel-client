using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using System.Collections.Generic;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObject<TileGenerator>
    {
        [SerializeField] private GameObject _numberTile1Prefab;
        [SerializeField] private GameObject _numberTile2Prefab;
        [SerializeField] private GameObject _numberTile3Prefab;
        [SerializeField] private GameObject _numberTile4Prefab;
        [SerializeField] private GameObject _numberTile5Prefab;
        [SerializeField] private GameObject _numberTile6Prefab;
        [SerializeField] private GameObject _numberTile7Prefab;
        [SerializeField] private GameObject _numberTile8Prefab;
        [SerializeField] private GameObject _normalTilePrefab;
        [SerializeField] private GameObject _warpTilePrefab;

        private List<GameObject> _numberTilePrefabs;
        private readonly GameObject[,] _tiles = new GameObject[StageSize.ROW, StageSize.COLUMN];

        private void Awake()
        {
            _numberTilePrefabs = new List<GameObject> {
                _numberTile1Prefab,
                _numberTile2Prefab,
                _numberTile3Prefab,
                _numberTile4Prefab,
                _numberTile5Prefab,
                _numberTile6Prefab,
                _numberTile7Prefab,
                _numberTile8Prefab
            };
        }

        /* 普通タイルの作成 */
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

        /* ワープタイルの作成 */
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

        /* ナンバータイルの作成 */
        public void CreateNumberTile(int panelNum, int tileNum)
        {
            // パネルに合わせたタイルを選択
            var numberTile = Instantiate(_numberTilePrefabs[panelNum - 1]);

            var tilePosition = GetTilePosition(tileNum);

            numberTile.GetComponent<NumberTileController>().Initialize(tilePosition, tileNum);

            SetTile(tileNum, numberTile);
        }

        /* タイルの座標を取得 */
        private static Vector2 GetTilePosition(int tileNum)
        {
            // 最上タイルのy座標
            const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);

            // 処理計算では0から扱いたい
            tileNum -= 1;
            // 行 (0~4)
            var row = tileNum / StageSize.COLUMN;
            // 列 (0~2)
            var column = tileNum % StageSize.COLUMN;
            // 作成するタイルのx,y座標
            var positionX = TileSize.WIDTH * (column - 1);
            var positionY = topTilePositionY - TileSize.HEIGHT * row;

            return new Vector2(positionX, positionY);
        }

        /* タイルを配列に格納 */
        private void SetTile(int tileNum, GameObject tile)
        {
            // 行 (0~4)
            var row = (tileNum - 1) / StageSize.COLUMN;
            // 列 (0~2)
            var column = (tileNum - 1) % StageSize.COLUMN;

            _tiles[row, column] = tile;
        }

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

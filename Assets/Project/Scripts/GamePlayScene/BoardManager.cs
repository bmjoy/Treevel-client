using System.Collections.Generic;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public class BoardManager : SingletonObject<BoardManager>
    {
        /// <summary>
        /// タイル、ボトルとそれぞれのワールド座標を保持する square の二次元配列
        /// </summary>
        private readonly Square[,] _squares = new Square[StageSize.ROW, StageSize.COLUMN];

        /// <summary>
        /// key: ボトル (GameObject)，value: ボトルの現在位置 (Vector2Int)
        /// </summary>
        private readonly Dictionary<GameObject, Vector2Int> _bottlePositions = new Dictionary<GameObject, Vector2Int>();

        private void Awake()
        {
            // `squares` の初期化
            for (var row = 0; row < StageSize.ROW; ++row) {
                for (var col = 0; col < StageSize.COLUMN; ++col) {
                    // ワールド座標を求める
                    var x = TileSize.WIDTH * (col - StageSize.COLUMN / 2);
                    var y = TileSize.HEIGHT * (StageSize.ROW / 2 - row);

                    _squares[row, col] = new Square(x, y);
                }
            }
        }

        /// <summary>
        /// タイル番号からタイルを取得
        /// </summary>
        /// <param name="tileNum"> タイル番号 </param>
        /// <returns> タイル </returns>
        [CanBeNull]
        public GameObject GetTile(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);

            return _squares[x, y].Tile != null ? _squares[x, y].Tile.gameObject : null;
        }

        /// <summary>
        /// タイル番号からそのタイルの上にあるボトルを取得
        /// </summary>
        /// <param name="tileNum"> タイル番号 </param>
        /// <returns> ボトル </returns>
        [CanBeNull]
        public GameObject GetBottle(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);

            return _squares[x, y].Bottle != null ? _squares[x, y].Bottle.gameObject : null;
        }

        /// <summary>
        /// 行 (`vec.x`)、列 `(vec.y)` からタイル番号を取得
        /// </summary>
        /// <param name="vec"> 行、列の二次元ベクトル </param>
        /// <returns> タイル番号 </returns>
        private int XYToTileNum(Vector2Int vec)
        {
            return XYToTileNum(vec.x, vec.y);
        }

        /// <summary>
        /// 行 (`x`)、列 (`y`) からタイル番号を取得
        /// </summary>
        /// <param name="x"> 行 </param>
        /// <param name="y"> 列 </param>
        /// <returns> タイル番号 </returns>
        private int XYToTileNum(int x, int y)
        {
            return (x * _squares.GetLength(1)) + y + 1;
        }

        /// <summary>
        /// タイル番号から行、列に変換する
        /// </summary>
        /// <param name="tileNum"> タイル番号 </param>
        /// <returns> (行, 列) </returns>
        private(int, int) TileNumToXY(int tileNum)
        {
            var x = (tileNum - 1) / _squares.GetLength(1);
            var y = (tileNum - 1) % _squares.GetLength(1);

            return (x, y);
        }

        /// <summary>
        /// ボトルをフリックする方向に移動する
        /// </summary>
        /// <param name="bottle"> フリックするボトル </param>
        /// <param name="direction"> フリックする方向 </param>
        public static void Move(GameObject bottle, Vector2 direction)
        {
            var bottleController = bottle?.GetComponent<AbstractBottleController>();
            if (!(bottleController is DynamicBottleController))
                return;

            // 移動方向を正規化
            // ワールド座標型のX,Yを時計回りに90度回転させ行列におけるX,Yを求める
            var directionInt = Vector2Int.RoundToInt(Vector2.Perpendicular(direction.Direction()));

            // 該当ボトルの現在位置
            var currPos = _bottlePositions[bottle];

            var targetPos = currPos + directionInt;
            // 移動目標地をボードの範囲内に収める
            targetPos.x = Mathf.Clamp(targetPos.x, 0, StageSize.ROW - 1);
            targetPos.y = Mathf.Clamp(targetPos.y, 0, StageSize.COLUMN - 1);

            var targetTileNum = XYToTileNum(targetPos.x, targetPos.y);

            SetBottle(bottleController, targetTileNum);
        }

        /// <summary>
        /// </summary>
        {
            return ((tileNum - 1) / _board.GetLength(1), (tileNum - 1) % _board.GetLength(1));
        }

        /// <summary>
        /// タイル`tile`をタイル番号`tileNum`の格子に設置する
        /// </summary>
        /// <param name="tile">設置するタイル</param>
        /// <param name="tileNum">タイル番号</param>
        public static void SetTile(AbstractTileController tile, int tileNum)
        {
            if (tile == null)
                return;

            lock (_board) {
                var(x, y) = TileNumToXY(tileNum);
                var target = _board[x, y];

                // 既にタイルが置いてあった場合、disabledにする(ノーマルタイルは重複利用されるため、消したくない)
                if (target.Tile != null) {
                    target.Tile.gameObject.SetActive(false);
                }

                target.Tile = tile;
                tile.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// ボトルをタイル番号`tileNum`の位置に設置する
        /// </summary>
        /// <param name="bottle">設置するボトル</param>
        /// <param name="tileNum">目標タイル番号</param>
        public static void SetBottle(AbstractBottleController bottle, int tileNum)
        {
            lock (_board) {
                // 目標の格子を取得
                var(targetX, targetY) = TileNumToXY(tileNum);
                var targetSquare = _board[targetX, targetY];

                // 目標位置にすでにボトルがある
                if (targetSquare.Bottle != null)
                    return;

                // ボトルの元の位置が保存されたらその位置のボトルを消す
                if (_bottlePositions.ContainsKey(bottle.gameObject)) {
                    var from = _bottlePositions[bottle.gameObject];
                    _squares[from.x, from.y].Bottle = null;
                }

                // 新しい格子に設定
                _bottlePositions[bottle.gameObject] = new Vector2Int(targetX, targetY);
                targetSquare.Bottle = bottle;
            }
        }

        /// <summary>
        /// ボトルがいるタイル番号を取得
        /// </summary>
        /// <param name="bottle"> ボトル </param>
        /// <returns> タイル番号 </returns>
        public int GetBottlePos(AbstractBottleController bottle)
        {
            return XYToTileNum(_bottlePositions[bottle.gameObject]);
        }

        /// <summary>
        /// ボード上の格子単位のデータを格納するクラス
        /// </summary>
        private class Square
        {
            /// <summary>
            /// 格子にあるタイル
            /// </summary>
            private AbstractTileController _tile;

            public AbstractTileController Tile
            {
                get => _tile;
                set {
                    _tile = value;
                    if (_tile != null)
                        _tile.transform.position = _worldPosition;
                }
            }

            /// <summary>
            /// 格子にあるボトル
            /// </summary>
            [CanBeNull] private AbstractBottleController _bottle;

            [CanBeNull]
            public AbstractBottleController Bottle
            {
                get => _bottle;
                set {
                    if (_bottle == value)
                        return;

                    if (value == null && _bottle != null) {
                        // ボトルがこの格子から離れる
                        _tile.OnBottleExit(_bottle.gameObject);
                        _bottle = null;
                    } else {
                        // 新しいボトルがこの格子に入る
                        _bottle = value;

                        // 移動する
                        _bottle.transform.position = _worldPosition;

                        // ボトルがタイルに入る時のボトルの処理
                        _bottle.OnEnterTile(_tile.gameObject);

                        // ボトルがタイルに入る時のボトルの処理
                        _tile.OnBottleEnter(_bottle.gameObject);
                    }
                }
            }

            /// <summary>
            /// 格子のワールド座標
            /// </summary>
            private readonly Vector2 _worldPosition;


            public Square(float x, float y)
            {
                _worldPosition = new Vector2(x, y);
            }
        }
    }
}

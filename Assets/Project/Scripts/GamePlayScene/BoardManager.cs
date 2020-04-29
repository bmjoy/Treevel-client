using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public static class BoardManager
    {
        /// <summary>
        /// タイル、ボトルとそれぞれのワールド座標を保持する「ボード」
        /// </summary>
        private static readonly Square[,] _board = new Square[StageSize.ROW, StageSize.COLUMN];

        /// <summary>
        /// ボトルの現在位置を保存するボトルから参照できる辞書
        /// </summary>
        /// <typeparam name="GameObject">ボトルのゲームオブジェクト</typeparam>
        /// <typeparam name="Vector2">現在位置`(x, y)=>（row, column）`</typeparam>
        private static readonly Dictionary<GameObject, Vector2Int> _bottlePositions = new Dictionary<GameObject, Vector2Int>();

        /// <summary>
        /// ボードを初期化、行数×列数分の格子（`Square`）を用意し、
        /// 格子毎のワールド座標をタイルのサイズに基づき計算する
        /// </summary>
        public static void Initialize()
        {
            for (var row = 0; row < StageSize.ROW; ++row) {
                for (var col = 0; col < StageSize.COLUMN; ++col) {
                    var x = TileSize.WIDTH * (col - StageSize.COLUMN / 2);
                    var y = TileSize.HEIGHT * (StageSize.ROW / 2 - row);
                    _board[row, col] = new Square(x, y);
                }
            }
        }

        /// <summary>
        /// タイル番号が`tileNum`のタイルを取得
        /// </summary>
        /// <param name="tileNum">タイル番号</param>
        /// <returns>タイルのゲームオブジェクト</returns>
        public static GameObject GetTile(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);
            return _board[x, y]?.Tile?.gameObject;
        }

        /// <summary>
        /// タイル番号が`tileNum`のタイルの上にボトルを取得
        /// </summary>
        /// <param name="tileNum">タイル番号</param>
        /// <returns>対象ボトルのゲームオブジェクト | null</returns>
        public static GameObject GetPanel(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);
            return _board[x, y]?.Panel?.gameObject;
        }

        /// <summary>
        /// ボトルをフリックする方向に移動する
        /// </summary>
        /// <param name="panel"> フリックするボトル </param>
        /// <param name="direction"> フリックする方向 </param>
        public static void Move(GameObject panel, Vector2 direction)
        {
            var panelController = panel?.GetComponent<AbstractBottleController>();
            if (!(panelController is DynamicBottleController))
                return;

            // 移動方向を正規化
            // ワールド座標型のX,Yを時計回りに90度回転させ行列におけるX,Yを求める
            var directionInt = Vector2Int.RoundToInt(Vector2.Perpendicular(direction.Direction()));

            // 該当ボトルの現在位置
            var currPos = _bottlePositions[panel];

            var targetPos = currPos + directionInt;
            // 移動目標地をボードの範囲内に収める
            targetPos.x = Mathf.Clamp(targetPos.x, 0, StageSize.ROW - 1);
            targetPos.y = Mathf.Clamp(targetPos.y, 0, StageSize.COLUMN - 1);

            var targetTileNum = XYToTileNum(targetPos.x, targetPos.y);

            SetPanel(panelController, targetTileNum);
        }

        /// <summary>
        /// 行(`vec.x`)、列`(vec.y)`からタイル番号に変換
        /// </summary>
        /// <param name="vec">行、列の二次元ベクトル</param>
        /// <returns>タイル番号</returns>
        private static int XYToTileNum(Vector2Int vec)
        {
            return XYToTileNum(vec.x, vec.y);
        }

        /// <summary>
        /// 行(`vec.x`)、列`(vec.y)`からタイル番号に変換
        /// </summary>
        /// <returns>タイル番号</returns>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <returns>タイル番号</returns>
        private static int XYToTileNum(int x, int y)
        {
            if (x >= _board.GetLength(0) || y >= _board.GetLength(1)) {
                Debug.LogWarning($"Invalid row or column (row, column = ({x},{y})");
            }
            return (x * _board.GetLength(1)) + y + 1;
        }

        /// <summary>
        /// タイル番号から行、列に変換する
        /// </summary>
        /// <param name="tileNum">タイル番号</param>
        /// <returns>（行, 列)</returns>
        private static(int, int) TileNumToXY(int tileNum)
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
        /// <param name="panel">設置するボトル</param>
        /// <param name="tileNum">目標タイル番号</param>
        public static void SetPanel(AbstractBottleController panel, int tileNum)
        {
            lock (_board) {
                // 目標の格子を取得
                var(targetX, targetY) = TileNumToXY(tileNum);
                var targetSquare = _board[targetX, targetY];

                // 目標位置にすでにボトルがある
                if (targetSquare.Panel != null)
                    return;

                // ボトルの元の位置が保存されたらその位置のボトルを消す
                if (_bottlePositions.ContainsKey(panel.gameObject)) {
                    var from = _bottlePositions[panel.gameObject];
                    _board[from.x, from.y].Panel = null;
                }

                // 新しい格子に設定
                _bottlePositions[panel.gameObject] = new Vector2Int(targetX, targetY);
                targetSquare.Panel = panel;
            }
        }

        /// <summary>
        /// ボトルがいるタイル番号を取得
        /// </summary>
        /// <param name="panel">調べたいボトル</param>
        /// <returns>タイル番号</returns>
        public static int GetPanelPos(AbstractBottleController panel)
        {
            var pos = _bottlePositions?[panel.gameObject] ?? default;
            return XYToTileNum(pos);
        }

        /// <summary>
        /// ボード上の格子単位のデータを格納するクラス
        /// </summary>
        private class Square
        {
            private AbstractBottleController _bottle = null;
            private AbstractTileController _tile = null;
            private readonly Vector2 _worldPosition;

            public AbstractBottleController Panel
            {
                get => _bottle;
                set {
                    if (_bottle == value)
                        return;

                    if (value == null && _bottle != null) {
                        // ボトルがこの格子から離れる
                        _tile.OnPanelExit(_bottle.gameObject);
                        _bottle = value;
                    } else {
                        // 新しいボトルがこの格子に入る
                        _bottle = value;

                        // 移動する
                        _bottle.transform.position = _worldPosition;

                        // ボトルがタイルに入る時ボトルの処理
                        if (_bottle is IEnterTileHandler handler) {
                            handler.OnEnterTile(_tile.gameObject);
                        }

                        // ボトルがタイルに入る時タイルの処理
                        _tile.OnPanelEnter(_bottle.gameObject);
                    }
                }
            }

            public AbstractTileController Tile
            {
                get => _tile;
                set {
                    _tile = value;
                    if (_tile != null)
                        _tile.transform.position = _worldPosition;
                }
            }

            public Square(float x, float y)
            {
                _worldPosition = new Vector2(x, y);
            }
        }
    }
}

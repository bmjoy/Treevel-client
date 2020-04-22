using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{

    public static class BoardManager
    {
        /// <summary>
        /// タイル、パネルとそれぞれのワールド座標を保持する「ボード」
        /// </summary>
        private static readonly Square[,] _board = new Square[StageSize.ROW, StageSize.COLUMN];

        /// <summary>
        /// パネルの現在位置を保存するパネルから参照できる辞書
        /// </summary>
        /// <typeparam name="GameObject">パネルのゲームオブジェクト</typeparam>
        /// <typeparam name="Vector2">現在位置`(x, y)=>（row, column）`</typeparam>
        private static readonly Dictionary<GameObject, Vector2Int> _panelPositions = new Dictionary<GameObject, Vector2Int>();

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
                    _board[row, col] = new Square(new Vector2(x, y));
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
            return _board?[x, y]?.Tile.gameObject;
        }

        /// <summary>
        /// タイル番号が`tileNum`のタイルの上にパネルを取得
        /// </summary>
        /// <param name="tileNum">タイル番号</param>
        /// <returns>対象パネルのゲームオブジェクト | null</returns>
        public static GameObject GetPanel(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);
            return _board?[x, y]?.Panel.gameObject;
        }

        /// <summary>
        /// パネルをフリックする方向に移動する
        /// </summary>
        /// <param name="panel"> フリックするパネル </param>
        /// <param name="direction"> フリックする方向 </param>
        public static void Move(GameObject panel, Vector2 direction)
        {
            var panelController = panel?.GetComponent<AbstractPanelController>();
            if (!(panelController is DynamicPanelController))
                return;

            // 移動方向を正規化
            // 行列におけるX,Yとワールド座標型のX,Yはちょうど90度違うので直角を取る
            var directionInt = Vector2Int.RoundToInt(Vector2.Perpendicular(direction.Direction()));

            // 該当パネルの現在位置
            var currPos = _panelPositions[panel];

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
        /// パネルをタイル番号`tileNum`の位置に設置する
        /// </summary>
        /// <param name="panel">設置するパネル</param>
        /// <param name="tileNum">目標タイル番号</param>
        public static void SetPanel(AbstractPanelController panel, int tileNum)
        {
            lock (_board) {
                // 目標の格子を取得
                var(targetX, targetY) = TileNumToXY(tileNum);
                var targetSquare = _board[targetX, targetY];

                // 目標位置にすでにパネルがある
                if (targetSquare.Panel != null)
                    return;

                // パネルの元の位置が保存されたらその位置のパネルを消す
                if (_panelPositions.ContainsKey(panel.gameObject)) {
                    var from = _panelPositions[panel.gameObject];
                    _board[from.x, from.y].Panel = null;
                }

                // 新しい格子に設定
                _panelPositions[panel.gameObject] = new Vector2Int(targetX, targetY);
                targetSquare.Panel = panel;
            }
        }

        /// <summary>
        /// パネルがいるタイル番号を取得
        /// </summary>
        /// <param name="panel">調べたいパネル</param>
        /// <returns>タイル番号</returns>
        public static int GetPanelPos(AbstractPanelController panel)
        {
            var pos = _panelPositions?[panel.gameObject] ?? default;
            return XYToTileNum(pos);
        }

        /// <summary>
        /// ボード上の格子単位のデータを格納するクラス
        /// </summary>
        private class Square
        {
            private AbstractPanelController _panel = null;
            private AbstractTileController _tile = null;
            private readonly Vector2 _worldPosition;

            public AbstractPanelController Panel
            {
                get => _panel;
                set {
                    if (_panel == value)
                        return;

                    if (value == null && _panel != null) {
                        // パネルがこの格子から離れる
                        _tile.OnPanelExit(_panel.gameObject);
                        _panel = value;
                    } else {
                        // 新しいパネルがこの格子に入る
                        _panel = value;

                        // 移動する
                        _panel.transform.position = _worldPosition;

                        // 成功判定
                        if ((_panel is IPanelSuccessHandler handler) && (handler.DoWhenSuccess())) {
                            GameObject.FindObjectOfType<GamePlayDirector>().CheckClear();
                        }

                        // タイルに乗っかる時の処理
                        _tile.OnPanelEnter(_panel.gameObject);
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

            public Square(Vector2 pos)
            {
                _worldPosition = pos;
            }
        }
    }
}

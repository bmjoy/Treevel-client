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
        private static Square[,] _board = new Square[StageSize.ROW, StageSize.COLUMN];

        /// <summary>
        /// パネルの現在位置を保存するパネルから参照できる辞書
        /// </summary>
        /// <typeparam name="GameObject">パネルのゲームオブジェクト</typeparam>
        /// <typeparam name="Vector2">現在位置`(x, y)=>（row, column）`</typeparam>
        private static Dictionary<GameObject, Vector2> _panelPositions = new Dictionary<GameObject, Vector2>();

        /// <summary>
        /// ボードを初期化、行数×列数分の格子（`Square`）を用意し、
        /// 格子毎のワールド座標をタイルのサイズを基づき計算する
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
        /// 行、列から座標を取得
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        public static Vector2 GetPosition(int row, int column)
        {
            return _board[row, column].WorldPosition;
        }

        /// <summary>
        /// 1から始まるタイル番号から座標を取得
        /// </summary>
        /// <param name="num">タイル番号</param>
        /// <returns></returns>
        public static Vector2 GetPosition(int num)
        {
            var(x, y) = TileNumToXY(num);
            return GetPosition(x, y);
        }

        public static GameObject GetTile(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);
            return _board?[x, y]?.Tile?.gameObject;
        }

        public static GameObject GetPanel(int tileNum)
        {
            var(x, y) = TileNumToXY(tileNum);
            return _board?[x, y]?.Panel?.gameObject;
        }

        /// <summary>
        /// パネルをフリックする方向に移動する
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="direction"></param>
        public static void Move(GameObject panel, Vector2 direction)
        {
            var panelController = panel?.GetComponent<PanelController>();
            if (!(panelController is DynamicPanelController))
                return;

            // 移動方向を正規化
            // 行列におけるX,Yとワールド座標型のX,Yはちょうど90度違うので直角を取る
            direction = Vector2.Perpendicular(direction.Direction());

            // 該当パネルの現在位置
            var currPos = _panelPositions[panel];

            var targetPos = currPos + direction;
            // 移動目標地をボードの範囲内に収める
            targetPos.x = Mathf.Clamp(0, targetPos.x, StageSize.ROW);
            targetPos.y = Mathf.Clamp(0, targetPos.y, StageSize.COLUMN);

            var targetTileNum = XYToTileNum((int)targetPos.x, (int)targetPos.y);

            SetPanel(panelController, targetTileNum);
        }

        private static int XYToTileNum(Vector2 vec)
        {
            return XYToTileNum((int)vec.x, (int)vec.y);
        }

        private static int XYToTileNum(int x, int y)
        {
            return (x * _board.GetLength(1)) + y + 1;
        }

        private static(int, int) TileNumToXY(int num)
        {
            return ((num - 1) / _board.GetLength(1), (num - 1) % _board.GetLength(1));
        }

        public static void SetTile(AbstractTile tile, int tileNum)
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
        /// パネルをタイル番号`pos`の位置に設定する
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool SetPanel(PanelController panel, int pos)
        {
            lock (_board) {
                // 目標の格子を取得
                var(targetX, targetY) = TileNumToXY(pos);
                var targetSquare = _board[targetX, targetY];

                // 目標位置にすでにパネルがある
                if (targetSquare.Panel != null)
                    return false;

                // パネルの元の位置が保存されたらその位置のパネルを消す
                if (_panelPositions.ContainsKey(panel.gameObject)) {
                    var from = _panelPositions[panel.gameObject];
                    _board[(int)from.x, (int)from.y].Panel = null;
                }

                // 新しい格子に設定
                _panelPositions[panel.gameObject] = new Vector2(targetX, targetY);
                targetSquare.Panel = panel;
                return true;
            }
        }

        /// <summary>
        /// パネルがいるタイル番号を取得
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static int GetPanelPos(PanelController panel)
        {
            var pos = _panelPositions?[panel.gameObject] ?? default;
            return XYToTileNum(pos);
        }

        /// <summary>
        /// ボード上の格子単位のデータを格納するクラス
        /// </summary>
        private class Square
        {
            private PanelController _panel = null;
            private AbstractTile _tile = null;
            public readonly Vector2 WorldPosition;

            public PanelController Panel
            {
                get => _panel;
                set {
                    // パネルが移動する時タイルから離れる処理をする
                    if (value == null && _panel != null) {
                        _tile.OnPanelExit(_panel.gameObject);
                    }

                    _panel = value;
                    if (_panel == null)
                        return;

                    // 移動する
                    _panel.transform.position = WorldPosition;

                    // 成功判定
                    if ((_panel is ITileAdaptHandler) && ((_panel as ITileAdaptHandler).Adapt())) {
                        GameObject.FindObjectOfType<GamePlayDirector>().CheckClear();
                    }

                    // タイルに乗っかる時の処理
                    _tile.OnPanelEnter(_panel.gameObject);
                }
            }

            public AbstractTile Tile
            {
                get => _tile;
                set {
                    _tile = value;
                    if (_tile != null)
                        _tile.transform.position = WorldPosition;
                }
            }

            public Square(Vector2 pos)
            {
                WorldPosition = pos;
            }
        }
    }
}

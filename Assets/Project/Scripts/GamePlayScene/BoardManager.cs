using System.Collections;
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

            return _squares[x, y].tile != null ? _squares[x, y].tile.gameObject : null;
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

            return _squares[x, y].bottle != null ? _squares[x, y].bottle.gameObject : null;
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
        /// <param name="bottle"> 移動するボトル </param>
        /// <param name="direction"> フリックする方向 </param>
        public void HandleFlickedBottle(DynamicBottleController bottle, Vector2 direction)
        {
            // 移動方向を正規化
            // ワールド座標型のX,Yを時計回りに90度回転させ行列におけるX,Yを求める
            var directionInt = Vector2Int.RoundToInt(Vector2.Perpendicular(direction.Direction()));

            // 該当ボトルの現在位置
            var currPos = _bottlePositions[bottle.gameObject];

            var targetPos = currPos + directionInt;
            // 移動目標地をボードの範囲内に収める
            targetPos.x = Mathf.Clamp(targetPos.x, 0, StageSize.ROW - 1);
            targetPos.y = Mathf.Clamp(targetPos.y, 0, StageSize.COLUMN - 1);

            var targetTileNum = XYToTileNum(targetPos.x, targetPos.y);

            Move(bottle, targetTileNum);
        }

        /// <summary>
        /// ボトルを特定のタイルに移動する
        /// </summary>
        /// <param name="bottle"> 移動するボトル </param>
        /// <param name="tileNum"> 移動先のタイル番号 </param>
        /// <param name="isAnimation"> 移動にアニメーションをつけるか </param>
        public void Move(DynamicBottleController bottle, int tileNum)
        {
            // 移動するボトルが null の場合は移動しない
            if (bottle == null) return;

            var(x, y) = TileNumToXY(tileNum);
            var targetSquare = _squares[x, y];

            // 移動先に既にボトルがある場合は移動しない
            if (targetSquare.bottle != null) return;

            var bottleObject = bottle.gameObject;

            // 移動元からボトルを無くす
            var from = _bottlePositions[bottleObject];
            bottle.OnExitTile(_squares[from.x, from.y].tile.gameObject);
            _squares[from.x, from.y].bottle = null;
            _squares[from.x, from.y].tile.OnBottleExit(bottleObject);

            // ボトルを移動する
            StartCoroutine(bottle.Move(targetSquare.worldPosition, () => {
                // 移動先へボトルを登録する
                _bottlePositions[bottleObject] = new Vector2Int(x, y);
                targetSquare.bottle = bottle;

                targetSquare.bottle.OnEnterTile(targetSquare.tile.gameObject);
                targetSquare.tile.OnBottleEnter(bottleObject);
            }));
        }

        /// <summary>
        /// [初期化用] タイル`tile`をタイル番号`tileNum`の格子に設置する
        /// </summary>
        /// <param name="tile">設置するタイル</param>
        /// <param name="tileNum">タイル番号</param>
        public void SetTile(AbstractTileController tile, int tileNum)
        {
            lock (_squares) {
                var(x, y) = TileNumToXY(tileNum);
                var targetSquare = _squares[x, y];

                // 既にタイルが置いてあった場合、disabledにする(ノーマルタイルは重複利用されるため、消したくない)
                if (targetSquare.tile != null) {
                    targetSquare.tile.gameObject.SetActive(false);
                }

                // 格子に設定
                targetSquare.tile = tile;
                tile.gameObject.SetActive(true);

                // 適切な場所に設置
                targetSquare.tile.transform.position = targetSquare.worldPosition;
            }
        }

        /// <summary>
        /// [初期化用] ボトルをタイル番号`tileNum`の位置に設置する
        /// </summary>
        /// <param name="bottle">設置するボトル</param>
        /// <param name="tileNum">目標タイル番号</param>
        public void SetBottle(AbstractBottleController bottle, int tileNum)
        {
            lock (_squares) {
                // 目標の格子を取得
                var(targetX, targetY) = TileNumToXY(tileNum);
                var targetSquare = _squares[targetX, targetY];

                // 格子に設定
                _bottlePositions[bottle.gameObject] = new Vector2Int(targetX, targetY);
                targetSquare.bottle = bottle;

                // 適切な場所に設置
                targetSquare.bottle.transform.position = targetSquare.worldPosition;

                // ボトルがタイルに配置された場合の処理を行う
                // `tile.OnBottleEnter` は仕様上呼ばない方が良いと判断
                targetSquare.bottle.OnEnterTile(targetSquare.tile.gameObject);
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
            public AbstractTileController tile;

            /// <summary>
            /// 格子にあるボトル
            /// </summary>
            [CanBeNull] public AbstractBottleController bottle;

            /// <summary>
            /// 格子のワールド座標
            /// </summary>
            public readonly Vector2 worldPosition;


            public Square(float x, float y)
            {
                worldPosition = new Vector2(x, y);
            }
        }
    }
}

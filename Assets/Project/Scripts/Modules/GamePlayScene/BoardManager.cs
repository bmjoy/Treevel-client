using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using Treevel.Modules.GamePlayScene.Tile;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    [DefaultExecutionOrder(1)]
    public class BoardManager : SingletonObject<BoardManager>
    {
        /// <summary>
        /// タイル、ボトルとそれぞれのワールド座標を保持する square の二次元配列
        /// </summary>
        private readonly Square[,] _squares = new Square[Constants.StageSize.COLUMN, Constants.StageSize.ROW];

        /// <summary>
        /// key: ボトル (GameObject)，value: ボトルの現在位置 (Vector2Int)
        /// </summary>
        private readonly Dictionary<GameObject, Vector2Int> _bottlePositions = new Dictionary<GameObject, Vector2Int>();

        private IDisposable _disposable;
        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            // `squares` の初期化
            for (var col = 0; col < Constants.StageSize.COLUMN; ++col) {
                for (var row = 0; row < Constants.StageSize.ROW; ++row) {
                    // ワールド座標を求める
                    var x = GameWindowController.Instance.GetTileWidth() * (col - Constants.StageSize.COLUMN / 2);
                    var y = GameWindowController.Instance.GetTileHeight() * (Constants.StageSize.ROW / 2 - row);

                    _squares[col, row] = new Square(x, y);
                }
            }
        }

        public void Initialize()
        {
            _tokenSource = new CancellationTokenSource();
            _disposable = Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
                .Subscribe(_ => {
                    EndProcess();
                }).AddTo(this);
        }

        private void EndProcess()
        {
            _tokenSource.Cancel();
            _disposable.Dispose();
        }

        /// <summary>
        /// タイル番号からタイルを取得
        /// </summary>
        /// <param name="tileNum"> タイル番号 </param>
        /// <returns> タイル </returns>
        [CanBeNull]
        public GameObject GetTile(int tileNum)
        {
            var xy = TileNumToXY(tileNum);
            // 範囲外のタイル番号が指定された場合には何もしない
            if (xy == null) return null;

            var (x, y) = xy.Value;

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
            var xy = TileNumToXY(tileNum);
            // 範囲外のタイル番号が指定された場合には何もしない
            if (xy == null) return null;

            var (x, y) = xy.Value;

            return _squares[x, y].bottle != null ? _squares[x, y].bottle.gameObject : null;
        }

        /// <summary>
        /// 指定した行上の全てのボトルオブジェクトを取得
        /// </summary>
        public GameObject[] GetBottlesOnRow(ERow row)
        {
            List<GameObject> ret = new List<GameObject>();

            var r = (int)row;
            for (var c = 0; c < Constants.StageSize.COLUMN; c++) {
                if (_squares[c, r].bottle) {
                    ret.Add(_squares[c, r].bottle.gameObject);
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// 指定した列上の全てのボトルオブジェクトを取得する
        /// </summary>
        public GameObject[] GetBottlesOnColumn(EColumn column)
        {
            List<GameObject> ret = new List<GameObject>();

            var c = (int)column;
            for (var r = 0; r < Constants.StageSize.ROW; r++) {
                if (_squares[c, r].bottle) {
                    ret.Add(_squares[c, r].bottle.gameObject);
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// ボトルIDからボトルの現在位置を取得
        /// </summary>
        /// <param name="bottleId"></param>
        public Vector2 GetBottlePosById(int bottleId)
        {
            foreach (var item in _squares) {
                if (item.bottle != null && item.bottle.Id == bottleId) {
                    return item.worldPosition;
                }
            }

            Debug.LogError($"Cannot find bottle of ID[{bottleId}]");
            UIManager.Instance.ShowErrorMessage(EErrorCode.InvalidBottleID);
            return Vector2.zero;
        }

        /// <summary>
        /// 行 (`vec.x`)、列 `(vec.y)` からタイル番号を取得
        /// </summary>
        /// <param name="vec"> 行、列の二次元ベクトル </param>
        /// <returns> タイル番号 </returns>
        private int? XYToTileNum(Vector2Int vec)
        {
            return XYToTileNum(vec.x, vec.y);
        }

        /// <summary>
        /// 行 (`x`)、列 (`y`) からタイル番号を取得
        /// </summary>
        /// <param name="x"> 行 </param>
        /// <param name="y"> 列 </param>
        /// <returns> タイル番号 </returns>
        private int? XYToTileNum(int x, int y)
        {
            if (x < 0 || Constants.StageSize.COLUMN - 1 < x || y < 0 || Constants.StageSize.ROW - 1 < y) return null;

            return (y * _squares.GetLength(0)) + (x + 1);
        }

        /// <summary>
        /// タイル番号から行、列に変換する
        /// </summary>
        /// <param name="tileNum"> タイル番号 </param>
        /// <returns> (行, 列) </returns>
        public (int, int)? TileNumToXY(int tileNum)
        {
            if (tileNum < 1 || 15 < tileNum) return null;

            var x = (tileNum - 1) % _squares.GetLength(0);
            var y = (tileNum - 1) / _squares.GetLength(0);

            return (x, y);
        }

        /// <summary>
        /// x行y列のタイル上が空かどうか
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsEmptyTile(int x, int y)
        {
            if (x < 0 || Constants.StageSize.COLUMN - 1 < x || y < 0 || Constants.StageSize.ROW - 1 < y) return false;

            // ボトルが存在するかどうか
            return _squares[x, y].bottle == null;
        }

        /// <summary>
        /// ボトルをフリックする方向に移動する
        /// </summary>
        /// <param name="bottle"> 移動するボトル </param>
        /// <param name="directionInt"> フリックする方向 </param>
        /// <returns> フリックした結果，ボトルが移動したかどうか </returns>
        public async UniTask<bool> HandleFlickedBottle(DynamicBottleController bottle, Vector2Int directionInt)
        {
            // tileNum は原点が左上だが，方向ベクトルは原点が左下なので，加工する
            directionInt.y = -directionInt.y;

            // 該当ボトルの現在位置
            var currPos = _bottlePositions[bottle.gameObject];
            // 移動先の位置を特定
            var targetPos = currPos + directionInt;
            // 移動先の位置をタイル番号に変換
            var targetTileNum = XYToTileNum(targetPos.x, targetPos.y);

            if (targetTileNum == null) return false;

            return await Move(bottle, targetTileNum.Value, directionInt);
        }

        /// <summary>
        /// ボトルを特定のタイルに移動する
        /// </summary>
        /// <param name="bottle"> 移動するボトル </param>
        /// <param name="tileNum"> 移動先のタイル番号 </param>
        /// <param name="direction"> どちら方向から移動してきたか (単位ベクトル) </param>
        /// <param name="immediately"> 瞬間移動か </param>
        /// <returns> ボトルが移動したかどうか </returns>
        public async UniTask<bool> Move(DynamicBottleController bottle, int tileNum, Vector2Int? direction = null)
        {
            // 移動するボトルが null の場合は移動しない
            if (bottle == null) return false;

            var xy = TileNumToXY(tileNum);
            // 範囲外のタイル番号が指定された場合には何もしない
            if (xy == null) return false;

            var (x, y) = xy.Value;

            var targetSquare = _squares[x, y];

            // すでにボトルが置かれているタイルが指定された場合には何もしない
            if (targetSquare.bottle != null) return false;


            var bottleObject = bottle.gameObject;

            lock (targetSquare) {
                // 移動先に既にボトルがある場合は移動しない
                if (targetSquare.bottle != null) return false;

                // 移動元からボトルを無くす
                var from = _bottlePositions[bottleObject];
                bottle.OnExitTile(_squares[from.x, from.y].tile.gameObject);
                _squares[from.x, from.y].bottle = null;
                _squares[from.x, from.y].tile.OnBottleExit(bottleObject);

                // 移動先へボトルを登録する
                _bottlePositions[bottleObject] = new Vector2Int(x, y);
                targetSquare.bottle = bottle;
            }

            if (direction != null) {
                // ボトルを移動する
                await bottle.Move(targetSquare.worldPosition, _tokenSource.Token)
                    .ContinueWith(() => {
                        targetSquare.bottle.OnEnterTile(targetSquare.tile.gameObject);
                        targetSquare.tile.OnBottleEnter(bottleObject, direction);
                    });
            } else {
                // ボトルを瞬間移動させる
                bottle.transform.position = targetSquare.worldPosition;
                targetSquare.bottle.OnEnterTile(targetSquare.tile.gameObject);
                targetSquare.tile.OnBottleEnter(bottleObject, null);
            }

            return true;
        }

        /// <summary>
        /// [初期化用] タイル`tile`をタイル番号`tileNum`の格子に設置する
        /// </summary>
        /// <param name="tile">設置するタイル</param>
        /// <param name="tileNum">タイル番号</param>
        public void SetTile(AbstractTileController tile, int tileNum)
        {
            lock (_squares) {
                var xy = TileNumToXY(tileNum);
                // 範囲外のタイル番号が指定された場合には何もしない
                if (xy == null) return;

                var (x, y) = xy.Value;
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
        public void InitializeBottle(AbstractBottleController bottle, int tileNum)
        {
            lock (_squares) {
                var xy = TileNumToXY(tileNum);
                // 範囲外のタイル番号が指定された場合には何もしない
                if (xy == null) return;

                // 目標の格子を取得
                var (targetX, targetY) = xy.Value;
                var targetSquare = _squares[targetX, targetY];

                // 格子に設定
                _bottlePositions[bottle.gameObject] = new Vector2Int(targetX, targetY);
                targetSquare.bottle = bottle;

                // 適切な場所に設置
                targetSquare.bottle.transform.position = targetSquare.worldPosition;

                // ボトルがタイルに配置された場合の処理を行う
                targetSquare.bottle.OnEnterTile(targetSquare.tile.gameObject);
                if (targetSquare.tile.RunOnBottleEnterAtInit) {
                    targetSquare.tile.OnBottleEnter(targetSquare.bottle.gameObject, null);
                }
            }
        }

        /// <summary>
        /// ボトルがいるタイル番号を取得
        /// </summary>
        /// <param name="bottle"> ボトル </param>
        /// <returns> タイル番号 </returns>
        public int GetBottlePos(AbstractBottleController bottle)
        {
            // ボトルは必ず盤面内に収まっているので，強制アンラップ
            return XYToTileNum(_bottlePositions[bottle.gameObject]).Value;
        }

        public Vector2 GetTilePos(int tileNum)
        {
            var xy = TileNumToXY(tileNum);
            if (xy == null) {
                throw new System.InvalidOperationException($"Invalid Tile Num {tileNum}");
            }

            var (x, y) = xy.Value;
            return _squares[x, y].worldPosition;
        }

        public Vector2 GetTilePos(ERow row, EColumn column)
        {
            return GetTilePos((int)column, (int)row);
        }

        public Vector2 GetTilePos(int x, int y)
        {
            var tileNum = XYToTileNum(x, y);
            if (tileNum == null) {
                throw new System.InvalidOperationException($"invalid (x, y) = ({x}, {y})");
            }

            return GetTilePos(tileNum.Value);
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

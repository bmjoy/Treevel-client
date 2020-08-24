using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public interface ISelfishHandler
    {
        /// <summary>
        /// ゲーム開始時の挙動
        /// </summary>
        void OnStartGame();

        /// <summary>
        /// FixedUpdate内の挙動
        /// </summary>
        void DoWhenFixedUpdate();

        /// <summary>
        /// フリック開始時の挙動
        /// </summary>
        void OnStartMove();

        /// <summary>
        /// フリック終了時の挙動
        /// </summary>
        void OnEndMove();

        /// <summary>
        /// ゲーム終了時の挙動
        /// </summary>
        void EndProcess();
    }

    internal class SelfishMoveHandler : ISelfishHandler
    {
        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly DynamicBottleController _bottle;

        /// <summary>
        /// 勝手に移動するまでのフレーム数
        /// </summary>
        private const int _LIMIT_TO_MOVE = 60;

        /// <summary>
        /// 何もしていない時の累計フレーム数
        /// </summary>
        private int _selfishTime = 0;

        /// <summary>
        /// フリックまたはホールド中かどうか
        /// </summary>
        private bool _isWatching = true;

        internal SelfishMoveHandler(DynamicBottleController bottle)
        {
            _bottle = bottle;
        }

        void ISelfishHandler.OnStartGame()
        {
            _isWatching = false;
        }

        void ISelfishHandler.DoWhenFixedUpdate()
        {
            if (!_isWatching) {
                _selfishTime++;
                if (_selfishTime == _LIMIT_TO_MOVE) {
                    // 空いている方向に移動させる
                    MoveToFreeDirection();
                    _selfishTime = 0;
                }
            }
        }

        void ISelfishHandler.OnStartMove()
        {
            // フリック中は勝手に移動するまでのフレーム数を計上しない
            _isWatching = true;
        }

        void ISelfishHandler.OnEndMove()
        {
            _isWatching = false;
        }

        void ISelfishHandler.EndProcess()
        {
            _isWatching = true;
            _selfishTime = 0;
        }


        // ホールド中は勝手に移動するまでのフレーム数を計上しない

        /// <summary>
        /// 空いている方向に移動させる
        /// </summary>
        private void MoveToFreeDirection()
        {
            // ボトルの位置を取得する
            var tileNum = BoardManager.Instance.GetBottlePos(_bottle);
            var(x, y) = BoardManager.Instance.TileNumToXY(tileNum).Value;
            var probabilityArray = new int[Enum.GetNames(typeof(EDirection)).Length];
            // 空いている方向を確認する
            // 左
            if (BoardManager.Instance.IsEmpty(x - 1, y)) probabilityArray[(int)EDirection.ToLeft] = 1;
            // 右
            if (BoardManager.Instance.IsEmpty(x + 1, y)) probabilityArray[(int)EDirection.ToRight] = 1;
            // 上
            if (BoardManager.Instance.IsEmpty(x, y - 1)) probabilityArray[(int)EDirection.ToUp] = 1;
            // 下
            if (BoardManager.Instance.IsEmpty(x, y + 1)) probabilityArray[(int)EDirection.ToBottom] = 1;
            // 空いている方向からランダムに1方向を選択する
            if (probabilityArray.Sum() == 0) return;
            var directionIndex = GimmickLibrary.SamplingArrayIndex(probabilityArray);
            switch (directionIndex) {
                case (int) EDirection.ToLeft:
                    BoardManager.Instance.Move(_bottle, tileNum - 1, Vector2Int.left);
                    break;
                case (int) EDirection.ToRight:
                    BoardManager.Instance.Move(_bottle, tileNum + 1, Vector2Int.right);
                    break;
                case (int) EDirection.ToUp:
                    BoardManager.Instance.Move(_bottle, tileNum - StageSize.COLUMN, Vector2Int.up);
                    break;
                case (int) EDirection.ToBottom:
                    BoardManager.Instance.Move(_bottle, tileNum + StageSize.COLUMN, Vector2Int.down);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}

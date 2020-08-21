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
    public class SelfishBottleController : NormalBottleController
    {
        /// <summary>
        /// 勝手に移動するまでのフレーム数
        /// </summary>
        private const int _LIMIT_TO_MOVE = 60;

        /// <summary>
        /// 何もしていない時の累計フレーム数
        /// </summary>
        private float _selfishTime = 0f;

        /// <summary>
        /// フリックまたはホールド中かどうか
        /// </summary>
        private bool _isWatching = true;

        public void OnStartGame()
        {
            _isWatching = false;
        }

        private void FixedUpdate()
        {
            // TODO: gamestartと同時に処理を走らせる
            // TODO: retry時の挙動のために_selfishTimeを0にする?
            if (!_isWatching) 
            {
                _selfishTime++;
                if (_selfishTime == _LIMIT_TO_MOVE)
                {
                    // 空いている方向に移動させる
                    MoveToFreeDirection();
                    _selfishTime = 0f;
                }
            }
        }

        protected override void EndProcess()
        {
            base.EndProcess();
            _isWatching = true;
        }

        protected override void HandleFlick(object sender, EventArgs s)
        {
            // フリック中は勝手に移動するまでのフレーム数を計上しない
            _isWatching = true;
            base.HandleFlick(sender, s);
            _isWatching = false;
        }

        // ホールド中は勝手に移動するまでのフレーム数を計上しない

        /// <summary>
        /// 空いている方向に移動させる
        /// </summary>
        private void MoveToFreeDirection()
        {
            // ボトルの位置を取得する
            var tileNum = BoardManager.Instance.GetBottlePos(this);
            var (x, y) = BoardManager.Instance.TileNumToXY(tileNum).Value;
            var probabilityArray = new int[Enum.GetNames(typeof(EDirection)).Length];
            // 空いている方向を確認する
            // 左
            if (BoardManager.Instance.IsEmpty(x-1, y)) probabilityArray[(int)EDirection.ToLeft] = 1;
            // 右
            if (BoardManager.Instance.IsEmpty(x+1, y)) probabilityArray[(int)EDirection.ToRight] = 1;
            // 上
            if (BoardManager.Instance.IsEmpty(x, y-1)) probabilityArray[(int)EDirection.ToUp] = 1;
            // 下
            if (BoardManager.Instance.IsEmpty(x, y+1)) probabilityArray[(int)EDirection.ToBottom] = 1;
            // 空いている方向からランダムに1方向を選択する
            if (probabilityArray.Sum() == 0) return;
            var directionIndex = GimmickLibrary.SamplingArrayIndex(probabilityArray);
            switch (directionIndex) {
                case (int) EDirection.ToLeft:
                    BoardManager.Instance.Move(this, tileNum - 1);
                    break;
                case (int) EDirection.ToRight:
                    BoardManager.Instance.Move(this, tileNum + 1);
                    break;
                case (int) EDirection.ToUp:
                    BoardManager.Instance.Move(this, tileNum - StageSize.COLUMN);
                    break;
                case (int) EDirection.ToBottom:
                    BoardManager.Instance.Move(this, tileNum + StageSize.COLUMN);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}

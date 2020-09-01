using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class SelfishEffectController : MonoBehaviour
    {
        private DynamicBottleController _bottleController;

        /// <summary>
        /// 勝手に移動するまでのフレーム数
        /// </summary>
        private const int _LIMIT_TO_MOVE = 120;

        /// <summary>
        /// 何もしていない時の累計フレーム数
        /// </summary>
        private int _selfishTime = 0;

        /// <summary>
        /// フリックまたはホールド中かどうか
        /// </summary>
        private bool _isWatching = true;

        private Animator _animator;
        private const string _SELFISH_TIME= "SelfishTime";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _isWatching = false;
        }

        public void Initialize(GameObject bottle)
        {
            transform.parent = bottle.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottle.GetComponent<DynamicBottleController>();
        }

        private void FixedUpdate()
        {
            if (!_isWatching) {
                _selfishTime++;
                if (_selfishTime == 36) {
                    _animator.SetInteger(_SELFISH_TIME, _selfishTime);
                } else if (_selfishTime == 78) {
                    _animator.SetInteger(_SELFISH_TIME, _selfishTime);
                } else if (_selfishTime == _LIMIT_TO_MOVE) {
                    // 空いている方向に移動させる
                    MoveToFreeDirection();
                    _animator.SetInteger(_SELFISH_TIME, _selfishTime);
                    _selfishTime = 0;
                }
            }
        }

        /// <summary>
        /// 空いている方向に移動させる
        /// </summary>
        private void MoveToFreeDirection()
        {
            // ボトルの位置を取得する
            var tileNum = BoardManager.Instance.GetBottlePos(_bottleController);
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
                    BoardManager.Instance.Move(_bottleController, tileNum - 1, Vector2Int.left);
                    break;
                case (int) EDirection.ToRight:
                    BoardManager.Instance.Move(_bottleController, tileNum + 1, Vector2Int.right);
                    break;
                case (int) EDirection.ToUp:
                    BoardManager.Instance.Move(_bottleController, tileNum - StageSize.COLUMN, Vector2Int.up);
                    break;
                case (int) EDirection.ToBottom:
                    BoardManager.Instance.Move(_bottleController, tileNum + StageSize.COLUMN, Vector2Int.down);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void SetIsWatching(bool _isWatching)
        {
            this._isWatching = _isWatching;
        }

        public void SetSelfishTime(int _selfishTime)
        {
            this._selfishTime = _selfishTime;
        }
    }

    public interface ISelfishHandler
    {
        /// <summary>
        /// フリック開始時の挙動
        /// </summary>
        void OnStartMove();

        /// <summary>
        /// フリック終了時の挙動
        /// </summary>
        void OnEndMove();

        /// <summary>
        /// プレス開始時の挙動
        /// </summary>
        void OnPressed();

        /// <summary>
        /// プレス終了時の挙動
        /// </summary>
        void OnReleased();

        /// <summary>
        /// ゲーム終了時の挙動
        /// </summary>
        void EndProcess();
    }

    internal class SelfishMoveHandler : ISelfishHandler
    {
        /// <summary>
        /// SelfishBottleのEffectインスタンス
        /// </summary>
        private SelfishEffectController _selfishEffectController;

        internal SelfishMoveHandler(GameObject bottle)
        {
            Initialize(bottle);
        }

        private async void Initialize(GameObject bottle)
        {
            var selfishEffect = await AddressableAssetManager.Instantiate(Address.SELFISH_EFFECT_PREFAB).Task;
            _selfishEffectController = selfishEffect.GetComponent<SelfishEffectController>();
            Debug.Log(_selfishEffectController);
            _selfishEffectController.Initialize(bottle);
            Debug.Log(_selfishEffectController);
        }

        void ISelfishHandler.OnStartMove()
        {
            // フリック中は勝手に移動するまでのフレーム数を計上しない
            _selfishEffectController.SetIsWatching(true);
        }

        void ISelfishHandler.OnEndMove()
        {
            _selfishEffectController.SetIsWatching(false);
        }

        void ISelfishHandler.OnPressed()
        {
            // ホールド中は勝手に移動するまでのフレーム数を計上しない
            _selfishEffectController.SetIsWatching(true);
        }

        void ISelfishHandler.OnReleased()
        {
            _selfishEffectController.SetIsWatching(false);
        }

        void ISelfishHandler.EndProcess()
        {
            _selfishEffectController.SetIsWatching(true);
            _selfishEffectController.SetSelfishTime(0);
        }
    }
}

using System;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class SelfishEffectController : GameObjectControllerBase
    {
        private DynamicBottleController _bottleController;

        /// <summary>
        /// 勝手に移動するまでの秒数
        /// </summary>
        private const float _SECONDS_TO_MOVE = 2.4f;

        /// <summary>
        /// 勝手に移動するまでのフレーム数
        /// </summary>
        private int _framesToMove;

        /// <summary>
        /// 勝手に移動していないフレーム数
        /// </summary>
        private int _calmFrames = 0;

        /// <summary>
        /// "勝手に移動していないフレーム数"を数えるかどうか
        /// ジェスチャー検知時は"勝手に移動していないフレーム数"を数えない
        /// 初期化が終了するまでは数えないように"false"とする
        /// </summary>
        private bool _countCalmFrames = false;

        private Animator _animator;
        private Animator _bottleAnimator;
        private const string _ANIMATOR_PARAM_INT_SELFISH_TIME = "SelfishTime";
        private const string _ANIMATOR_PARAM_TRIGGER_IDLE = "SelfishIdle";
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "SelfishSpeed";

        private void Awake()
        {
            _framesToMove = (int)(GamePlayDirector.FRAME_RATE * _SECONDS_TO_MOVE);
            _animator = GetComponent<Animator>();
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
            _bottleController.StartMove.Subscribe(_ => SetIsStopping(false)).AddTo(this);
            _bottleController.EndMove.Subscribe(_ => SetIsStopping(true)).AddTo(this);
            _bottleController.pressGesture.OnPress.AsObservable().Subscribe(_ => SetIsStopping(false)).AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.releaseGesture.OnRelease.AsObservable().Subscribe(_ => SetIsStopping(true)).AddTo(compositeDisposableOnGameEnd, this);

            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                _countCalmFrames = false;
                _calmFrames = 0;
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }).AddTo(this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => {
                // 移動していないフレーム数を数え始める
                _countCalmFrames = true;
            }).AddTo(this);

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleEffectType.Selfish.GetOrderInLayer();
        }

        private void FixedUpdate()
        {
            if (!_countCalmFrames) return;

            _calmFrames++;
            if (_calmFrames == _framesToMove) {
                // 空いている方向にBottleを移動させる
                MoveToFreeDirection();
                _calmFrames = 0;
                // 通常時アニメーションの起動
                _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_IDLE);
                _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_IDLE);
            }

            _animator.SetInteger(_ANIMATOR_PARAM_INT_SELFISH_TIME, _calmFrames);
            _bottleAnimator.SetInteger(_ANIMATOR_PARAM_INT_SELFISH_TIME, _calmFrames);
        }

        /// <summary>
        /// 空いている方向にBottleを移動させる
        /// </summary>
        private void MoveToFreeDirection()
        {
            // ボトルの位置を取得する
            var tileNum = BoardManager.Instance.GetBottlePos(_bottleController);
            var (x, y) = BoardManager.Instance.TileNumToXY(tileNum).Value;

            var canMoveDirections = new int[Enum.GetNames(typeof(EDirection)).Length];
            // 空いている方向を確認する
            // 左
            if (BoardManager.Instance.IsEmptyTile(x - 1, y)) canMoveDirections[(int)EDirection.ToLeft] = 1;
            // 右
            if (BoardManager.Instance.IsEmptyTile(x + 1, y)) canMoveDirections[(int)EDirection.ToRight] = 1;
            // 上
            if (BoardManager.Instance.IsEmptyTile(x, y - 1)) canMoveDirections[(int)EDirection.ToUp] = 1;
            // 下
            if (BoardManager.Instance.IsEmptyTile(x, y + 1)) canMoveDirections[(int)EDirection.ToDown] = 1;

            // 空いている方向からランダムに1方向を選択する
            if (canMoveDirections.Sum() == 0) return;
            var direction =
                (EDirection)Enum.ToObject(typeof(EDirection), GimmickLibrary.SamplingArrayIndex(canMoveDirections));
            switch (direction) {
                case EDirection.ToLeft:
                    BoardManager.Instance.Move(_bottleController, tileNum - 1, Vector2Int.left);
                    break;
                case EDirection.ToRight:
                    BoardManager.Instance.Move(_bottleController, tileNum + 1, Vector2Int.right);
                    break;
                case EDirection.ToUp:
                    BoardManager.Instance.Move(_bottleController, tileNum - Constants.StageSize.COLUMN, Vector2Int.up);
                    break;
                case EDirection.ToDown:
                    BoardManager.Instance.Move(_bottleController, tileNum + Constants.StageSize.COLUMN,
                                               Vector2Int.down);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 動き出すまでの時間の計測を状態に合わせてアニメーションの再開・停止させる
        /// </summary>
        /// <param name="_isStopping"></param>
        private void SetIsStopping(bool countCalmFrames)
        {
            _countCalmFrames = countCalmFrames;
            if (_countCalmFrames) {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
            } else {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }
        }
    }
}

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
        private const int _FRAMES_TO_MOVE = 120;

        /// <summary>
        /// 勝手に移動していないフレーム数
        /// </summary>
        private int _calmFrames = 0;

        /// <summary>
        /// "勝手に移動していないフレーム数"を数えるかどうか
        /// ジェスチャー検知時は"勝手に移動していないフレーム数"を数えない
        /// </summary>
        private bool _countCalmFrames = false;

        private Animator _animator;
        private Animator _bottleAnimator;
        private const string _ANIMATOR_PARAM_INT_SELFISH_TIME = "SelfishTime";
        private const string _ANIMATOR_PARAM_TRIGGER_IDLE = "SelfishIdle";
        private const string _ANIMATOR_PARAM_SPEED = "SelfishSpeed";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(GameObject bottle)
        {
            transform.parent = bottle.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottle.GetComponent<DynamicBottleController>();
            if (_bottleController == null) {
                Debug.LogError("There is no DynamicBottleController");
                return;
            }
            _bottleAnimator = bottle.GetComponent<Animator>();

            _countCalmFrames = true;
        }

        private void FixedUpdate()
        {
            if (!_countCalmFrames) return;

            _calmFrames++;
            if (_calmFrames == _FRAMES_TO_MOVE) {
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
            var(x, y) = BoardManager.Instance.TileNumToXY(tileNum).Value;

            var canMoveDirections = new int[Enum.GetNames(typeof(EDirection)).Length];
            // 空いている方向を確認する
            // 左
            if (BoardManager.Instance.IsEmptyTile(x - 1, y)) canMoveDirections[(int)EDirection.ToLeft] = 1;
            // 右
            if (BoardManager.Instance.IsEmptyTile(x + 1, y)) canMoveDirections[(int)EDirection.ToRight] = 1;
            // 上
            if (BoardManager.Instance.IsEmptyTile(x, y - 1)) canMoveDirections[(int)EDirection.ToUp] = 1;
            // 下
            if (BoardManager.Instance.IsEmptyTile(x, y + 1)) canMoveDirections[(int)EDirection.ToBottom] = 1;

            // 空いている方向からランダムに1方向を選択する
            if (canMoveDirections.Sum() == 0) return;
            var direction = (EDirection)Enum.ToObject(typeof(EDirection), GimmickLibrary.SamplingArrayIndex(canMoveDirections));
            switch (direction) {
                case EDirection.ToLeft:
                    BoardManager.Instance.Move(_bottleController, tileNum - 1, Vector2Int.left);
                    break;
                case EDirection.ToRight:
                    BoardManager.Instance.Move(_bottleController, tileNum + 1, Vector2Int.right);
                    break;
                case EDirection.ToUp:
                    BoardManager.Instance.Move(_bottleController, tileNum - StageSize.COLUMN, Vector2Int.up);
                    break;
                case EDirection.ToBottom:
                    BoardManager.Instance.Move(_bottleController, tileNum + StageSize.COLUMN, Vector2Int.down);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 動き出すまでの時間の計測を状態に合わせてアニメーションの再開・停止させる
        /// </summary>
        /// <param name="_isStopping"></param>
        public void SetIsStopping(bool countCalmFrames)
        {
            _countCalmFrames = countCalmFrames;
            if (_countCalmFrames) {
                _animator.SetFloat(_ANIMATOR_PARAM_SPEED, 1f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_SPEED, 1f);
            } else {
                _animator.SetFloat(_ANIMATOR_PARAM_SPEED, 0f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_SPEED, 0f);
            }
        }


        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        public void EndProcess()
        {
            _countCalmFrames = false;
            _calmFrames = 0;
            _animator.SetFloat(_ANIMATOR_PARAM_SPEED, 0f);
            _bottleAnimator.SetFloat(_ANIMATOR_PARAM_SPEED, 0f);
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
            _selfishEffectController.Initialize(bottle);
        }

        void ISelfishHandler.OnStartMove()
        {
            // フリック中は勝手に移動するまでのフレーム数を計上しない
            _selfishEffectController.SetIsStopping(false);
        }

        void ISelfishHandler.OnEndMove()
        {
            _selfishEffectController.SetIsStopping(true);
        }

        void ISelfishHandler.OnPressed()
        {
            // ホールド中は勝手に移動するまでのフレーム数を計上しない
            _selfishEffectController.SetIsStopping(false);
        }

        void ISelfishHandler.OnReleased()
        {
            _selfishEffectController.SetIsStopping(true);
        }

        void ISelfishHandler.EndProcess()
        {
            _selfishEffectController.EndProcess();
        }
    }
}

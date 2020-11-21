using System.Collections;
using Treevel.Common.Entities;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick.Powder
{
    public class PiledUpPowderController : AbstractGimmickController
    {
        private NormalBottleController _bottleController;

        private Animator _animator;
        private Animator _bottleAnimator;
        private bool _isPiledUp = false;
        private const string _ANIMATOR_PARAM_BOOL_TRIGGER = "PiledUpTrigger";
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "PiledUpSpeed";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NormalBottleController bottleController)
        {
            var preLocalScale = transform.localScale;
            transform.parent = bottleController.transform;
            transform.localScale = new Vector2(1f, 1f);
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
            _bottleController.StartMove += HandleStartMove;
            _bottleController.EndMove += HandleEndMove;
        }

        private void OnDestroy()
        {
            _bottleController.StartMove -= HandleStartMove;
            _bottleController.EndMove -= HandleEndMove;
        }

        public override IEnumerator Trigger()
        {
            _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, true);
            yield return null;
        }

        private void HandleStartMove()
        {
            _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, false);
        }

        private void HandleEndMove()
        {
            _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, true);
        }

        /// <summary>
        /// ボトルを失敗扱いにする(Animationから呼び出し)
        /// </summary>
        private void SetBottleFailed()
        {
            _isPiledUp = true;
            _bottleAnimator.SetTrigger(LifeEffectController.ANIMATOR_PARAM_TRIGGER_DEAD);
            // 失敗原因を保持する
            GamePlayDirector.Instance.failureReason = EGimmickType.Powder.GetFailureReason();
            // 失敗状態に移行する
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }

        protected override void OnEndGame()
        {
            _bottleController.StartMove -= HandleStartMove;
            _bottleController.EndMove -= HandleEndMove;
            // 自身が失敗原因でない場合はアニメーションを止める
            if (!_isPiledUp) {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }
        }
    }
}

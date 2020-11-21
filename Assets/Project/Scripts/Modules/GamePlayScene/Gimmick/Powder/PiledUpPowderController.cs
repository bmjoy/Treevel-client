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
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
        }

        private void OnDestroy()
        {
        }

        public override IEnumerator Trigger()
        {
            _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, true);
            yield return null;
        }

        /// <summary>
        /// ボトルを失敗扱いにする
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
            // 自身が失敗原因でない場合はアニメーションを止める
            if (!_isPiledUp) {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }
        }
    }
}

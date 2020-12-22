using System;
using System.Collections;
using Treevel.Common.Entities;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick.Powder
{
    public class PiledUpPowderController : AbstractGimmickController
    {
        private NormalBottleController _bottleController;

        private Animator _animator;
        private Animator _bottleAnimator;

        /// <summary>
        /// 堆積して失敗したかどうか
        /// </summary>
        private bool _isPiledUp = false;

        private const string _ANIMATOR_PARAM_BOOL_TRIGGER = "PiledUpTrigger";
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "PiledUpSpeed";

        /// <summary>
        /// 購読解除クラス
        /// </summary>
        private CompositeDisposable _eventDisposable = new CompositeDisposable();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NormalBottleController bottleController)
        {
            // ボトル上に配置
            transform.parent = bottleController.transform;
            // TODO: ボトルの容器部分に砂が積もるように調整する
            transform.localScale = new Vector2(1.2f, 1.32f);
            transform.localPosition = Vector3.zero;

            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
            _bottleController.StartMove.Subscribe(_ => {
                _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, false);
            }).AddTo(_eventDisposable, this);
            _bottleController.EndMove.Subscribe(_ => {
                _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, true);
            }).AddTo(_eventDisposable, this);
        }

        public override IEnumerator Trigger()
        {
            _animator.SetBool(_ANIMATOR_PARAM_BOOL_TRIGGER, true);
            yield return null;
        }

        /// <summary>
        /// ボトルを失敗扱いにする(Animationから呼び出し)
        /// </summary>
        private void SetBottleFailed()
        {
            if (GamePlayDirector.Instance.State != GamePlayDirector.EGameState.Playing)
                return;
            _isPiledUp = true;
            _bottleAnimator.SetTrigger(LifeEffectController.ANIMATOR_PARAM_TRIGGER_DEAD);
            // 失敗原因を保持する
            GamePlayDirector.Instance.failureReason = EGimmickType.Powder.GetFailureReason();
            // 失敗状態に移行する
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }

        protected override void HandleGameSucceeded()
        {
            base.HandleGameSucceeded();
            Destroy(gameObject);
        }

        protected override void OnEndGame()
        {
            _eventDisposable.Dispose();
            // 自身が失敗原因でない場合はアニメーションを止める
            if (!_isPiledUp) {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }
        }
    }
}

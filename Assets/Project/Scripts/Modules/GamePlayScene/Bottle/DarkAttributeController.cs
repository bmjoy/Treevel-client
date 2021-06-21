using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class DarkAttributeController : BottleAttributeControllerBase
    {
        private GoalBottleController _bottleController;

        /// <summary>
        /// 成功状態かどうか
        /// </summary>
        private bool _isSuccess;

        private static readonly int _ANIMATOR_IS_DARK = Animator.StringToHash("IsDark");

        protected override void Awake()
        {
            base.Awake();
            GamePlayDirector.Instance.StagePrepared.Subscribe(_ => spriteRenderer.enabled = true).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => animator.enabled = true).AddTo(compositeDisposableOnGameEnd,this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => animator.enabled = false).AddTo(this);
            // 描画順序の設定
            spriteRenderer.sortingOrder = EBottleAttributeType.Dark.GetOrderInLayer();
        }

        public void Initialize(GoalBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;

            // イベントに処理を登録する
            _bottleController.isSuccess.Subscribe(value => {
                _isSuccess = value;
                animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
            }).AddTo(this);
            _bottleController.longPressGesture.OnLongPress.AsObservable().Subscribe(_ => animator.SetBool(_ANIMATOR_IS_DARK, false)).AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.releaseGesture.OnRelease.AsObservable().Subscribe(_ => animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess)).AddTo(compositeDisposableOnGameEnd, this);
        }
    }
}

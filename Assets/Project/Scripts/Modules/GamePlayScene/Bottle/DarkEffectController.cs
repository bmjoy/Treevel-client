using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class DarkEffectController : GameObjectControllerBase
    {
        private GoalBottleController _bottleController;

        /// <summary>
        /// 成功状態かどうか
        /// </summary>
        private bool _isSuccess;

        private Animator _animator;
        private const string _ANIMATOR_IS_DARK = "IsDark";
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "DarkSpeed";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(GoalBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;

            // イベントに処理を登録する
            _bottleController.EnterTile.Merge(_bottleController.ExitTile)
                .Subscribe(_ => {
                    _isSuccess = _bottleController.IsSuccess();
                    _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
                }).AddTo(this);
            _bottleController.longPressGesture.OnLongPress.AsObservable().Subscribe(_ => _animator.SetBool(_ANIMATOR_IS_DARK, false)).AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.releaseGesture.OnRelease.AsObservable().Subscribe(_ => _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess)).AddTo(compositeDisposableOnGameEnd, this);

            GamePlayDirector.Instance.GameEnd.Subscribe(_ => _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f)).AddTo(this);

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleEffectType.Dark.GetOrderInLayer();

            // 初期状態の登録
            _isSuccess = _bottleController.IsSuccess();
            _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
        }
    }
}

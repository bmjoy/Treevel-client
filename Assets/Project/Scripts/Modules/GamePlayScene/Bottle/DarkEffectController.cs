using System;
using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class DarkEffectController : AbstractGameObjectController
    {
        private NormalBottleController _bottleController;

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

        public void Initialize(NormalBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;

            // イベントに処理を登録する
            Observable.Merge(_bottleController.EnterTile, _bottleController.ExitTile)
                .Subscribe(_ => {
                    _isSuccess = _bottleController.IsSuccess();
                    _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
                }).AddTo(compositeDisposable, this);
            _bottleController.longPressGesture.LongPressed += HandleLongPressed;
            _bottleController.releaseGesture.Released += HandleReleased;
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
                .Subscribe(_ => {
                    _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                    _bottleController.longPressGesture.LongPressed -= HandleLongPressed;
                    _bottleController.releaseGesture.Released -= HandleReleased;
                }).AddTo(this);

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleEffectType.Dark.GetOrderInLayer();

            // 初期状態の登録
            _isSuccess = _bottleController.IsSuccess();
            _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
        }

        private void OnDestroy()
        {
            _bottleController.longPressGesture.LongPressed -= HandleLongPressed;
            _bottleController.releaseGesture.Released -= HandleReleased;
        }

        /// <summary>
        /// ホールド開始時の処理
        /// </summary>
        private void HandleLongPressed(object sender, EventArgs e)
        {
            _animator.SetBool(_ANIMATOR_IS_DARK, false);
        }

        /// <summary>
        /// ホールド終了時の処理
        /// </summary>
        private void HandleReleased(object sender, EventArgs e)
        {
            _animator.SetBool(_ANIMATOR_IS_DARK, !_isSuccess);
        }
    }
}

﻿using Treevel.Common.Entities;
using Treevel.Modules.GamePlayScene.Gimmick;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class LifeEffectController : MonoBehaviour
    {
        private DynamicBottleController _bottleController;

        /// <summary>
        /// 自身のライフが0になったかどうか
        /// </summary>
        private bool _isDead;

        /// <summary>
        /// 自身のライフ
        /// </summary>
        private int _life;

        private Animator _animator;
        private Animator _bottleAnimator;
        private const string _ANIMATOR_PARAM_TRIGGER_ATTACKED = "LifeAttacked";
        public const string ANIMATOR_PARAM_TRIGGER_DEAD = "LifeDead";
        private const string _ANIMATOR_PARAM_FLOAT_SPPED = "LifeSpeed";
        private const string _ANIMATOR_PARAM_BOOL_ATTACKED_LOOP = "LifeAttackedLoop";

        private void Awake()
        {
            // 現状、LifeEffectについてのアニメーション演出はない
            _animator = GetComponent<Animator>();
        }

        public void Initialize(DynamicBottleController bottleController, int life)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
            _bottleController.GetDamaged += HandleGetDamaged;
            _bottleController.EndGame += HandleEndGame;

            _life = life;
        }

        private void OnDestroy()
        {
            _bottleController.GetDamaged -= HandleGetDamaged;
            _bottleController.EndGame -= HandleEndGame;
        }

        /// <summary>
        /// ギミックに攻撃されたときの処理
        /// </summary>
        /// <param name="gimmick"></param>
        private void HandleGetDamaged(GameObject gimmick)
        {
            _life--;
            if (_life < 0) {
                Debug.LogError("_currentLife が負の値になっている");
            } else if (_life == 0) {
                // 失敗演出
                _bottleAnimator.SetTrigger(ANIMATOR_PARAM_TRIGGER_DEAD);

                // ボトルを死んだ状態にする
                _isDead = true;

                // 失敗原因を保持する
                var controller = gimmick.GetComponent<AbstractGimmickController>();
                if (controller == null)
                    controller = gimmick.GetComponentInParent<AbstractGimmickController>();

                var gimmickType = controller.GimmickType;
                GamePlayDirector.Instance.failureReason = gimmickType.GetFailureReason();

                // 失敗状態に移行する
                GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
            } else if (_life == 1) {
                // ループさせて危機感っぽい
                _bottleAnimator.SetBool(_ANIMATOR_PARAM_BOOL_ATTACKED_LOOP, true);
                _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
            } else {
                _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
            }
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        private void HandleEndGame()
        {
            // 自身が破壊されていない場合はアニメーションを止める
            if (!_isDead) {
                _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPPED, 0f);
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPPED, 0f);
            }

            _bottleController.GetDamaged -= HandleGetDamaged;
            _bottleController.EndGame -= HandleEndGame;
        }
    }
}
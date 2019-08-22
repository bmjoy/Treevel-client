﻿using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    /// <summary>
    /// ライフ付きのナンバーパネル
    /// </summary>
    public class LifeNumberPanelController : NumberPanelController
    {
        /// <summary>
        /// 攻撃されたときのアニメーション
        /// </summary>
        [SerializeField] private AnimationClip _attackedAnimation;
        /// <summary>
        /// パネルが銃弾の攻撃に耐えられる回数
        /// </summary>
        [SerializeField] private int _maxLife = 3;
        /// <summary>
        /// 残ってる回数
        /// </summary>
        private int _currentLife;

        protected override void Awake()
        {
            base.Awake();
            _maxLife = Mathf.Max(_maxLife, 1); // MaxLifeの最小値を１にする
            _currentLife = _maxLife;

            _anim.AddClip(_attackedAnimation, _attackedAnimation.name);
        }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == TagName.BULLET) {
                --_currentLife;
                if (_currentLife <= 0) {
                    _anim.Play(_deadAnimation.name, PlayMode.StopAll);

                    // 失敗状態に移行する
                    gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
                } else if (_currentLife == 1) {
                    // ループさせて危機感っぽい
                    _anim.wrapMode = WrapMode.Loop;
                    _anim.Play(_attackedAnimation.name, PlayMode.StopAll);
                } else {
                    _anim.clip = _attackedAnimation;
                    _anim.Play(_attackedAnimation.name, PlayMode.StopAll);
                }
            }
        }
    }
}

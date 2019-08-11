using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    /// <summary>
    /// ライフ付きのナンバーパネル
    /// </summary>
    public class LifeNumberPanelController : NumberPanelController
    {
        /// <summary>
        /// パネルが銃弾の攻撃に耐えられる回数
        /// </summary>
        [SerializeField] private int _maxLife = 3;

        /// <summary>
        /// 残ってる回数
        /// </summary>
        private int _currentLife;

        Animation _anim;
        protected override void Awake()
        {
            base.Awake();
            _currentLife = MaxLife;
            _anim = GetComponent<Animation>();
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
                if (_currentLife == 0) {
                    _anim.Play("NumberPanelDead", PlayMode.StopAll);

                    // 失敗状態に移行する
                    gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
                    return;
                } else if (_currentLife == 1) {
                    // ループさせて危機感っぽい
                    _anim.wrapMode = WrapMode.Loop;
                    _anim.Play();
                } else {
                    _anim.Play();
                }
            }
        }

        #region getters / setters
        public int MaxLife => _maxLife;
        #endregion
    }
}

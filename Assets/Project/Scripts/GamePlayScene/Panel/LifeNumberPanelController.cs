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
            if (other.tag == TagName.BULLET && other.gameObject.transform.position.z >= 0) {
                --_currentLife;
                if (_currentLife <= 0) {
                    // 失敗演出
                    _anim.Play(_deadAnimation.name, PlayMode.StopAll);

                    // 自身が破壊された
                    _dead = true;

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

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected override void OnSucceed()
        {
            base.OnSucceed();
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected override void OnFail()
        {
            base.OnFail();
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の共通処理
        /// </summary>
        private void EndProcess()
        {
            // 自身が破壊されてない場合には，自身のアニメーションの繰り返しを停止
            if (!_dead) {
                _anim.wrapMode = WrapMode.Default;
            }
        }
    }
}

using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付きのナンバーボトル
    /// </summary>
    public class LifeBottleController : NormalBottleController
    {
        /// <summary>
        /// 攻撃されたときのアニメーション
        /// </summary>
        [SerializeField] private AnimationClip _attackedAnimation;

        /// <summary>
        /// ボトルが銃弾の攻撃に耐えられる回数
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

            anim.AddClip(_attackedAnimation, AnimationClipName.LIFE_BOTTLE_GET_ATTACKED);
        }

        public override void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);
            _maxLife = bottleData.life;
            _currentLife = _maxLife;
        }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(TagName.BULLET) || other.gameObject.transform.position.z < 0) return;

            --_currentLife;
            if (_currentLife <= 0) {
                // 失敗演出
                anim.Play(AnimationClipName.NORMAL_BOTTLE_DEAD, PlayMode.StopAll);

                // 自身が破壊された
                dead = true;

                // 失敗状態に移行する
                GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
            } else if (_currentLife == 1) {
                // ループさせて危機感っぽい
                anim.wrapMode = WrapMode.Loop;
                anim.Play(AnimationClipName.LIFE_BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            } else {
                anim.clip = _attackedAnimation;
                anim.Play(AnimationClipName.LIFE_BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            }
        }
    }
}

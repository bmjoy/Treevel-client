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

        protected override void Awake()
        {
            base.Awake();

            // アニメーション設定
            anim.AddClip(_attackedAnimation, AnimationClipName.LIFE_BOTTLE_GET_ATTACKED);
        }

        public override void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);

            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
        }
    }
}

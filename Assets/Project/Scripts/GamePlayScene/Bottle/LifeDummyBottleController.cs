using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public class LifeDummyBottleController : DynamicBottleController
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

        public override void Initialize(GameDatas.BottleData bottleData)
        {
            base.Initialize(bottleData);

            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
        }
    }
}

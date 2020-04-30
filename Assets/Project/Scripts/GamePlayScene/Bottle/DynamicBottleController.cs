using System;
using Project.Scripts.Utils.Definitions;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(FlickGesture))]
    public class DynamicBottleController : AbstractBottleController
    {
        protected Animation anim;

        /// <summary>
        /// ワープタイルでワープする時のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpAnimation;

        /// <summary>
        /// ワープタイルでワープした後のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpReverseAnimation;

        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = BottleName.DYNAMIC_DUMMY_BOTTLE;
            #endif
            // FlickGesture の設定
            GetComponent<FlickGesture>().MinDistance = 0.2f;
            GetComponent<FlickGesture>().FlickTime = 0.2f;

            // アニメーションの追加
            anim = GetComponent<Animation>();
            anim.AddClip(warpAnimation, AnimationClipName.BOTTLE_WARP);
            anim.AddClip(warpReverseAnimation, AnimationClipName.BOTTLE_WARP_REVERSE);
        }

        private void OnEnable()
        {
            GetComponent<FlickGesture>().Flicked += HandleFlick;
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        /// <summary>
        /// フリックイベントを処理する
        /// </summary>
        private void HandleFlick(object sender, EventArgs e)
        {
            var gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // ボトルを移動する
            BoardManager.Move(gameObject, gesture.ScreenFlickVector);
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected virtual void OnSucceed()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            // アニメーションを止める
            anim[AnimationClipName.BOTTLE_WARP].speed = 0.0f;
            anim[AnimationClipName.BOTTLE_WARP_REVERSE].speed = 0.0f;
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected virtual void OnFail()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            anim[AnimationClipName.BOTTLE_WARP].speed = 0.0f;
            anim[AnimationClipName.BOTTLE_WARP_REVERSE].speed = 0.0f;
        }
    }
}

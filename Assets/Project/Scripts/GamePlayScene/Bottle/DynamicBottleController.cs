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
        private Animation _anim;

        /// <summary>
        /// ワープタイルでワープする時のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpAnimation;

        /// <summary>
        /// ワープタイルでワープした後のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpReverseAnimation;

        /// <summary>
        /// 動くことができる状態か
        /// </summary>
        public bool IsMovable = true;

        /// <summary>
        /// ボトルがいるべき場所
        /// </summary>
        public Vector3? targetPosition = null;

        /// <summary>
        /// フリック 時のパネルの移動速度
        /// </summary>
        private const float _SPEED = 0.2f;

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
            _anim = GetComponent<Animation>();
            _anim.AddClip(warpAnimation, AnimationClipName.BOTTLE_WARP);
            _anim.AddClip(warpReverseAnimation, AnimationClipName.BOTTLE_WARP_REVERSE);
        }

        protected void Update()
        {
            if (targetPosition == null) return;

            if (transform.position != targetPosition.Value) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition.Value, _SPEED);
            }
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
            if (!IsMovable) return;

            var gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // ボトルのフリック情報を伝える
            BoardManager.Instance.HandleFlickedBottle(gameObject.GetComponent<DynamicBottleController>(), gesture.ScreenFlickVector);
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected virtual void OnSucceed()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected virtual void OnFail()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        private void EndProcess()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            _anim[AnimationClipName.BOTTLE_WARP].speed = 0.0f;
            _anim[AnimationClipName.BOTTLE_WARP_REVERSE].speed = 0.0f;

            // 自身が破壊されてない場合には，自身のアニメーションの繰り返しを停止
            if (!IsDead) {
                _anim.wrapMode = WrapMode.Default;
            }
        }
    }
}

using System;
using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(FlickGesture))]
    [RequireComponent(typeof(PressGesture))]
    [RequireComponent(typeof(ReleaseGesture))]
    public class DynamicBottleController : AbstractBottleController
    {
        private Animation _anim;

        private FlickGesture _flickGesture;
        private PressGesture _pressGesture;
        private ReleaseGesture _releaseGesture;

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
        /// ボトルが移動中かどうか
        /// </summary>
        private bool _moving = false;

        /// <summary>
        /// フリック 時のパネルの移動速度
        /// </summary>
        private const float _SPEED = 0.3f;

        public int FlickNum
        {
            get;
            private set;
        } = 0;

        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = BottleName.DYNAMIC_DUMMY_BOTTLE;
            #endif
            // FlickGesture の設定
            _flickGesture = GetComponent<FlickGesture>();
            _flickGesture.MinDistance = 0.2f;
            _flickGesture.FlickTime = 0.2f;
            // PressGesture の設定
            _pressGesture = GetComponent<PressGesture>();
            // ReleaseGesture の設定
            _releaseGesture = GetComponent<ReleaseGesture>();

            // アニメーションの追加
            _anim = GetComponent<Animation>();
            _anim.AddClip(warpAnimation, AnimationClipName.BOTTLE_WARP);
            _anim.AddClip(warpReverseAnimation, AnimationClipName.BOTTLE_WARP_REVERSE);
        }

        public override void Initialize(BottleData bottleData)
        {
            // set handlers
            if (bottleData.isSelfish) {
                selfishHandler = new SelfishMoveHandler(this);
            }

            base.Initialize(bottleData);
        }

        private void OnEnable()
        {
            _flickGesture.Flicked += HandleFlick;
            _pressGesture.Pressed += HandlePress;
            _releaseGesture.Released += HandleRelease;
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            _flickGesture.Flicked -= HandleFlick;
            _pressGesture.Pressed -= HandlePress;
            _releaseGesture.Released -= HandleRelease;
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

            // 移動方向を単一方向の単位ベクトルに変換する ex) (0, 1)
            var directionInt = Vector2Int.RoundToInt(ExtensionVector2.Normalize(gesture.ScreenFlickVector));

            // ボトルのフリック情報を伝える
            if (BoardManager.Instance.HandleFlickedBottle(this, directionInt)) FlickNum++;
        }

        /// <summary>
        /// プレス開始イベントを処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void HandlePress(object sender, EventArgs e)
        {
            selfishHandler?.OnPressed();
        }

        /// <summary>
        /// プレス終了イベントを処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRelease(object sender, EventArgs e)
        {
            selfishHandler?.OnReleased();
        }

        /// <summary>
        /// アタッチされているTouchScriptイベントの状態を変更する
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetGesturesEnabled(bool isEnable)
        {
            _flickGesture.enabled = isEnable;
            _pressGesture.enabled = isEnable;
            _releaseGesture.enabled = isEnable;
        }

        public IEnumerator Move(Vector3 targetPosition, UnityAction callback)
        {
            SetGesturesEnabled(false);
            selfishHandler?.OnStartMove();

            while (transform.position != targetPosition) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                yield return new WaitForFixedUpdate();
            }

            selfishHandler?.OnEndMove();
            SetGesturesEnabled(true);

            callback();
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
        protected virtual void EndProcess()
        {
            selfishHandler?.EndProcess();

            _flickGesture.Flicked -= HandleFlick;
            _pressGesture.Pressed -= HandlePress;
            _releaseGesture.Released -= HandleRelease;
            _anim[AnimationClipName.BOTTLE_WARP].speed = 0.0f;
            _anim[AnimationClipName.BOTTLE_WARP_REVERSE].speed = 0.0f;

            // 自身が破壊されてない場合には，自身のアニメーションの繰り返しを停止
            if (!IsDead) {
                _anim.wrapMode = WrapMode.Default;
            }
        }
    }
}

﻿using System;
using System.Collections;
using Project.Scripts.Utils;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(FlickGesture))]
    [RequireComponent(typeof(PressGesture))]
    [RequireComponent(typeof(ReleaseGesture))]
    public class DynamicBottleController : AbstractBottleController
    {
        private FlickGesture _flickGesture;
        private PressGesture _pressGesture;
        private ReleaseGesture _releaseGesture;

        /// <summary>
        /// 動くことができる状態か
        /// </summary>
        public bool IsMovable = true;

        /// <summary>
        /// 移動開始時の処理
        /// </summary>
        public event Action HandleOnStartMove;

        /// <summary>
        /// 移動終了時の処理
        /// </summary>
        public event Action HandleOnEndMove;

        /// <summary>
        /// ホールド開始時の処理
        /// </summary>
        public event Action HandleOnPressed;

        /// <summary>
        /// ホールド終了時の処理
        /// </summary>
        public event Action HandleOnReleased;

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        public event Action HandleOnEndProcess;

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
        }

        public override async void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);
            
            // set handlers
            if (bottleData.isSelfish) {
                var selfishEffect = await AddressableAssetManager.Instantiate(Address.SELFISH_EFFECT_PREFAB).Task;
                selfishEffect.GetComponent<SelfishEffectController>().Initialize(this);
            }
        }

        private void OnEnable()
        {
            _flickGesture.Flicked += Flicked;
            _pressGesture.Pressed += Pressed;
            _releaseGesture.Released += Released;
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            _flickGesture.Flicked -= Flicked;
            _pressGesture.Pressed -= Pressed;
            _releaseGesture.Released -= Released;
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        /// <summary>
        /// フリックイベントを処理する
        /// </summary>
        private void Flicked(object sender, EventArgs e)
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
        private void Pressed(object sender, EventArgs e)
        {
            HandleOnPressed?.Invoke();
        }

        /// <summary>
        /// プレス終了イベントを処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Released(object sender, EventArgs e)
        {
            HandleOnReleased?.Invoke();
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
            HandleOnStartMove?.Invoke();

            while (transform.position != targetPosition) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                yield return new WaitForFixedUpdate();
            }

            HandleOnEndMove?.Invoke();
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
            HandleOnEndProcess?.Invoke();

            _flickGesture.Flicked -= Flicked;
            _pressGesture.Pressed -= Pressed;
            _releaseGesture.Released -= Released;
        }
    }
}

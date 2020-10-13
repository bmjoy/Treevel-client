using System;
using System.Collections;
using TouchScript.Gestures;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Extensions;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Treevel.Modules.GamePlayScene.Bottle
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
        public event Action OnStartMove
        {
            add {
                _onStartMove += value;
            }
            remove {
                _onStartMove -= value;
            }
        }
        private event Action _onStartMove;

        /// <summary>
        /// 移動終了時の処理
        /// </summary>
        public event Action OnEndMove
        {
            add {
                _onEndMove += value;
            }
            remove {
                _onEndMove -= value;
            }
        }
        private event Action _onEndMove;

        /// <summary>
        /// ホールド開始時の処理
        /// </summary>
        public event Action OnPressed
        {
            add {
                _onPressed += value;
            }
            remove {
                _onPressed -= value;
            }
        }
        private event Action _onPressed;

        /// <summary>
        /// ホールド終了時の処理
        /// </summary>
        public event Action OnReleased
        {
            add {
                _onReleased += value;
            }
            remove {
                _onReleased -= value;
            }
        }
        private event Action _onReleased;

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        public event Action OnEndProcess
        {
            add {
                _onEndProcess += value;
            }
            remove {
                _onEndProcess -= value;
            }
        }
        private event Action _onEndProcess;

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
            name = Constants.BottleName.DYNAMIC_DUMMY_BOTTLE;
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
                var selfishEffect = await AddressableAssetManager.Instantiate(Constants.Address.SELFISH_EFFECT_PREFAB).Task;
                selfishEffect.GetComponent<SelfishEffectController>().Initialize(this);
            }
        }

        private void OnEnable()
        {
            _flickGesture.Flicked += HandleFlicked;
            _pressGesture.Pressed += HandlePressed;
            _releaseGesture.Released += HandleReleased;
            GamePlayDirector.OnSucceed += HandleOnSucceed;
            GamePlayDirector.OnFail += HandleOnFail;
        }

        private void OnDisable()
        {
            _flickGesture.Flicked -= HandleFlicked;
            _pressGesture.Pressed -= HandlePressed;
            _releaseGesture.Released -= HandleReleased;
            GamePlayDirector.OnSucceed -= HandleOnSucceed;
            GamePlayDirector.OnFail -= HandleOnFail;
        }

        /// <summary>
        /// フリックイベントを処理する
        /// </summary>
        private void HandleFlicked(object sender, EventArgs e)
        {
            if (!IsMovable) return;

            var gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // 移動方向を単一方向の単位ベクトルに変換する ex) (0, 1)
            var directionInt = Vector2Int.RoundToInt(Vector2Extension.Normalize(gesture.ScreenFlickVector));

            // ボトルのフリック情報を伝える
            if (BoardManager.Instance.HandleFlickedBottle(this, directionInt)) FlickNum++;
        }

        /// <summary>
        /// プレス開始イベントを処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void HandlePressed(object sender, EventArgs e)
        {
            _onPressed?.Invoke();
        }

        /// <summary>
        /// プレス終了イベントを処理する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleReleased(object sender, EventArgs e)
        {
            _onReleased?.Invoke();
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
            _onStartMove?.Invoke();

            while (transform.position != targetPosition) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                yield return new WaitForFixedUpdate();
            }

            _onEndMove?.Invoke();
            SetGesturesEnabled(true);

            callback();
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected virtual void HandleOnSucceed()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected virtual void HandleOnFail()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        protected virtual void EndProcess()
        {
            _onEndProcess?.Invoke();
        }
    }
}

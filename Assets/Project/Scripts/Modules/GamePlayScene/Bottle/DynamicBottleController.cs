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
        public PressGesture pressGesture;
        public ReleaseGesture releaseGesture;

        /// <summary>
        /// 動くことができる状態か
        /// </summary>
        public bool IsMovable = true;

        /// <summary>
        /// フリックが反転するかどうか
        /// </summary>
        private bool _isReverse = false;

        /// <summary>
        /// 移動開始時の処理
        /// </summary>
        public event Action StartMove
        {
            add => _startMoveInvoker += value;
            remove => _startMoveInvoker -= value;
        }
        private event Action _startMoveInvoker;

        /// <summary>
        /// 移動終了時の処理
        /// </summary>
        public event Action EndMove
        {
            add => _endMoveInvoker += value;
            remove => _endMoveInvoker -= value;
        }
        private event Action _endMoveInvoker;

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        public event Action EndGame
        {
            add => _endGameInvoker += value;
            remove => _endGameInvoker -= value;
        }
        private event Action _endGameInvoker;

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
            pressGesture = GetComponent<PressGesture>();
            // ReleaseGesture の設定
            releaseGesture = GetComponent<ReleaseGesture>();
        }

        public override async void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);

            // set handlers
            if (bottleData.isSelfish) {
                var selfishEffect = await AddressableAssetManager.Instantiate(Constants.Address.SELFISH_EFFECT_PREFAB).Task;
                selfishEffect.GetComponent<SelfishEffectController>().Initialize(this);
            }
            if (bottleData.isReverse) {
                var reverseEffect = await AddressableAssetManager.Instantiate(Constants.Address.REVERSE_EFFECT_PREFAB).Task;
                reverseEffect.GetComponent<ReverseEffectController>().Initialize(this);
                _isReverse = true;
            }
        }

        protected virtual void OnEnable()
        {
            _flickGesture.Flicked += HandleFlicked;
            GamePlayDirector.GameSucceeded += HandleGameSucceeded;
            GamePlayDirector.GameFailed += HandleGameFailed;
        }

        protected virtual void OnDisable()
        {
            _flickGesture.Flicked -= HandleFlicked;
            GamePlayDirector.GameSucceeded -= HandleGameSucceeded;
            GamePlayDirector.GameFailed -= HandleGameFailed;
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
            var directionInt = Vector2Int.RoundToInt(gesture.ScreenFlickVector.NormalizeDirection());
            if (_isReverse) directionInt *= (-1);

            // ボトルのフリック情報を伝える
            if (BoardManager.Instance.HandleFlickedBottle(this, directionInt)) FlickNum++;
        }

        /// <summary>
        /// アタッチされているTouchScriptイベントの状態を変更する
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetGesturesEnabled(bool isEnable)
        {
            _flickGesture.enabled = isEnable;
            pressGesture.enabled = isEnable;
            releaseGesture.enabled = isEnable;
        }

        public IEnumerator Move(Vector3 targetPosition, UnityAction callback)
        {
            SetGesturesEnabled(false);
            _startMoveInvoker?.Invoke();

            while (transform.position != targetPosition) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                yield return new WaitForFixedUpdate();
            }

            _endMoveInvoker?.Invoke();
            SetGesturesEnabled(true);

            callback();
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected virtual void HandleGameSucceeded()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected virtual void HandleGameFailed()
        {
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        protected virtual void EndProcess()
        {
            _endGameInvoker?.Invoke();
            _flickGesture.Flicked -= HandleFlicked;
            GamePlayDirector.GameSucceeded -= HandleGameSucceeded;
            GamePlayDirector.GameFailed -= HandleGameFailed;
        }
    }
}

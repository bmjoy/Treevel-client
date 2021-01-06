using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TouchScript.Gestures;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Extensions;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
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
        public IObservable<Unit> StartMove => _startMoveSubject;
        private readonly Subject<Unit> _startMoveSubject = new Subject<Unit>();

        /// <summary>
        /// 移動終了時の処理
        /// </summary>
        public IObservable<Unit> EndMove => _endMoveSubject;
        private readonly Subject<Unit> _endMoveSubject = new Subject<Unit>();

        /// <summary>
        /// フリック 時のパネルの移動速度
        /// </summary>
        private const float _SPEED = 30f;

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
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
            .Subscribe(_ => {
                _flickGesture.Flicked -= HandleFlicked;
            }).AddTo(this);
        }

        protected void OnDisable()
        {
            _flickGesture.Flicked -= HandleFlicked;
        }

        /// <summary>
        /// フリックイベントを処理する
        /// </summary>
        private async void HandleFlicked(object sender, EventArgs e)
        {
            if (!IsMovable) return;

            var gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // 移動方向を単一方向の単位ベクトルに変換する ex) (0, 1)
            var directionInt = Vector2Int.RoundToInt(gesture.ScreenFlickVector.NormalizeDirection());
            if (_isReverse) directionInt *= -1;

            // ボトルのフリック情報を伝える
            if (await BoardManager.Instance.HandleFlickedBottle(this, directionInt)) FlickNum++;
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

        public async UniTask Move(Vector3 targetPosition, CancellationToken token)
        {
            try
            {
                SetGesturesEnabled(false);
                _startMoveSubject.OnNext(Unit.Default);

                while (transform.position != targetPosition)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                }

                _endMoveSubject.OnNext(Unit.Default);
                SetGesturesEnabled(true);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}

using TouchScript.Gestures;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(TapGesture))]
    [RequireComponent(typeof(Animator))]
    public class ErasableBottleController : StaticBottleController
    {
        /// <summary>
        /// タップジェスチャー
        /// </summary>
        private TapGesture _tapGesture;

        /// <summary>
        /// せりあがり演出のためのマスク
        /// </summary>
        private SpriteMask _spriteMask;

        /// <summary>
        /// 子オブジェクトのSpriteRenderer
        /// </summary>
        [SerializeField] private SpriteRenderer _childSpriteRenderer;

        /// <summary>
        /// アニメーター
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// In ステート
        /// </summary>
        private static readonly int _ANIMATOR_STATE_IN = Animator.StringToHash("Erasable@in");

        /// <summary>
        /// Out パラメーター
        /// </summary>
        private static readonly int _ANIMATOR_PARAM_OUT = Animator.StringToHash("Out");

        /// <summary>
        /// Out ステート
        /// </summary>
        private static readonly int _ANIMATOR_STATE_OUT = Animator.StringToHash("Erasable@out");

        protected override void Awake()
        {
            base.Awake();

            #if UNITY_EDITOR
            name = Constants.BottleName.ERASABLE_BOTTLE;
            #endif

            _spriteMask = GetComponent<SpriteMask>();
            _animator = GetComponent<Animator>();

            // TapGesture の設定
            _tapGesture = GetComponent<TapGesture>();
            _tapGesture.UseUnityEvents = true;
            _tapGesture.OnTap.AsObservable()
                .Subscribe(_ => {
                    // 退出させる
                    _animator.SetTrigger(_ANIMATOR_PARAM_OUT);
                })
                .AddTo(compositeDisposableOnGameEnd, this);

            _animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_STATE_IN)
                .Subscribe(_ => {
                    // 出現が終わった後の処理
                    _spriteMask.enabled = false;
                    _childSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                    SoundManager.Instance.StopSE(ESEKey.ErasableBottle_In);
                }).AddTo(this);
            _animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_STATE_OUT)
                .Subscribe(_ => {
                    // 退出が終わった後の処理
                    SoundManager.Instance.StopSE(ESEKey.ErasableBottle_Out);
                    Destroy(gameObject);
                })
                .AddTo(this);

            GamePlayDirector.Instance.GameEnd
                .Subscribe(_ => {
                    _animator.enabled = false;
                    SoundManager.Instance.StopSE(ESEKey.ErasableBottle_In);
                    SoundManager.Instance.StopSE(ESEKey.ErasableBottle_Out);
                })
                .AddTo(this);
        }

        /// <summary>
        /// 出現する（アニメーションイベントから呼び出す）
        /// </summary>
        private void In()
        {
            SoundManager.Instance.PlaySE(ESEKey.ErasableBottle_In);
        }

        /// <summary>
        /// 退出する（アニメーションイベントから呼び出す）
        /// </summary>
        private void Out()
        {
            SoundManager.Instance.PlaySE(ESEKey.ErasableBottle_Out);
        }
    }
}

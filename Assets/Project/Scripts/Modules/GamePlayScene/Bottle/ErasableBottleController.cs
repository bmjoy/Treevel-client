using TouchScript.Gestures;
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
        /// アニメーター
        /// </summary>
        private Animator _animator;

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

            _animator = GetComponent<Animator>();

            // TapGesture の設定
            _tapGesture = GetComponent<TapGesture>();
            _tapGesture.UseUnityEvents = true;
            _tapGesture.OnTap.AsObservable()
                .Subscribe(_ => {
                    // 退出させる
                    _animator.SetBool(_ANIMATOR_PARAM_OUT, true);
                })
                .AddTo(compositeDisposable, this);

            _animator.GetBehaviour<ObservableStateMachineTrigger>()
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_STATE_OUT)
                .Subscribe(_ => {
                    // 退出が終わった後の処理
                    Destroy(gameObject);
                })
                .AddTo(this);
        }
    }
}

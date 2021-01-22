using TouchScript.Gestures;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(TapGesture))]
    public class ErasableBottleController : StaticBottleController
    {
        /// <summary>
        /// タップジェスチャー
        /// </summary>
        private TapGesture _tapGesture;

        protected override void Awake()
        {
            base.Awake();

            #if UNITY_EDITOR
            name = Constants.BottleName.ERASABLE_BOTTLE;
            #endif

            // TapGesture の設定
            _tapGesture = GetComponent<TapGesture>();
            _tapGesture.UseUnityEvents = true;
            _tapGesture.OnTap.AsObservable()
                .Subscribe(_ => {
                    Destroy(gameObject);
                })
                .AddTo(this);
        }
    }
}

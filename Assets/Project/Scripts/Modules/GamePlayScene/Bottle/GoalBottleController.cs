using Cysharp.Threading.Tasks;
using SpriteGlow;
using TouchScript.Gestures;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(PostProcessVolume))]
    [RequireComponent(typeof(SpriteGlowEffect))]
    [RequireComponent(typeof(LongPressGesture))]
    public class GoalBottleController : DynamicBottleController
    {
        public LongPressGesture longPressGesture;

        /// <summary>
        /// 目標の色
        /// </summary>
        public EGoalColor GoalColor { get; private set; }

        /// <summary>
        /// 光らせるエフェクト
        /// </summary>
        private SpriteGlowEffect _spriteGlowEffect;

        public SpriteRendererUnifier spriteRendererUnifier;

        protected override void Awake()
        {
            base.Awake();
            longPressGesture = GetComponent<LongPressGesture>();
            longPressGesture.UseUnityEvents = true;
            longPressGesture.TimeToPress = 0.15f;
            spriteRendererUnifier = GetComponent<SpriteRendererUnifier>();
            EnterTile.Where(_ => IsSuccess()).Subscribe(_ => DoWhenSuccess()).AddTo(this);
            ExitTile.Subscribe(_ => _spriteGlowEffect.enabled = false).AddTo(this);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override async UniTask InitializeAsync(BottleData bottleData)
        {
            _spriteGlowEffect = GetComponent<SpriteGlowEffect>();
            _spriteGlowEffect.enabled = false;

            // parse data
            GoalColor = bottleData.goalColor;
            // GoalColorがNoneではないことを保証する
            if (GoalColor == EGoalColor.None) UIManager.Instance.ShowErrorMessageAsync(EErrorCode.InvalidBottleColor).Forget();

            await base.InitializeAsync(bottleData);

            // ボトルのスプライトを設定
            spriteRendererUnifier.SetSprite(AddressableAssetManager.GetAsset<Sprite>(bottleData.goalColor.GetBottleAddress()));

            // set handler
            var lifeEffect = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_EFFECT_PREFAB);
            lifeEffect.GetComponent<LifeEffectController>().Initialize(this, bottleData.life);
            if (bottleData.isDark) {
                var darkEffect = await AddressableAssetManager.Instantiate(Constants.Address.DARK_EFFECT_PREFAB);
                darkEffect.GetComponent<DarkEffectController>().Initialize(this);
            }

            #if UNITY_EDITOR
            name = Constants.BottleName.NORMAL_BOTTLE + Id;
            #endif
        }

        /// <summary>
        /// 目標タイルにいる時の処理
        /// </summary>
        private void DoWhenSuccess()
        {
            // 光らせる
            _spriteGlowEffect.enabled = true;
            // ステージの成功判定
            GamePlayDirector.Instance.CheckClear();
        }

        /// <summary>
        /// 目標タイルにいるかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return GoalColor != EGoalColor.None && BoardManager.Instance.GetTileColor(this) == GoalColor;
        }
    }
}

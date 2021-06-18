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
        /// <summary>
        /// 成功状態かどうか
        /// </summary>
        public readonly ReactiveProperty<bool> isSuccess = new ReactiveProperty<bool>(false);

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
            // 成功判定
            EnterTile.Where(_ => GoalColor != EGoalColor.None).Subscribe(_ => isSuccess.Value = BoardManager.Instance.GetTileColor(this) == GoalColor).AddTo(this);
            // 移動開始時は非成功に遷移
            ExitTile.Subscribe(_ => isSuccess.Value = false).AddTo(this);
            // 状態変化時の処理
            isSuccess.SkipLatestValueOnSubscribe().Subscribe(value => {
                _spriteGlowEffect.enabled = value;
                BoardManager.Instance.UpdateNumOfSuccessBottles(value);
            }).AddTo(this);
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
            var lifeAttribute = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_ATTRIBUTE_PREFAB);
            lifeAttribute.GetComponent<LifeAttributeController>().Initialize(this, bottleData.life);
            if (bottleData.isDark) {
                var darkAttribute = await AddressableAssetManager.Instantiate(Constants.Address.DARK_ATTRIBUTE_PREFAB);
                darkAttribute.GetComponent<DarkAttributeController>().Initialize(this);
            }

            #if UNITY_EDITOR
            name = Constants.BottleName.NORMAL_BOTTLE + Id;
            #endif
        }
    }
}

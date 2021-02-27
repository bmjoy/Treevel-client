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
    public class NormalBottleController : DynamicBottleController
    {
        public LongPressGesture longPressGesture;

        /// <summary>
        /// 目標の色
        /// </summary>
        private EGoalColor _goalColor;

        /// <summary>
        /// 光らせるエフェクト
        /// </summary>
        private SpriteGlowEffect _spriteGlowEffect;

        protected override void Awake()
        {
            base.Awake();
            longPressGesture = GetComponent<LongPressGesture>();
            longPressGesture.UseUnityEvents = true;
            longPressGesture.TimeToPress = 0.15f;
            EnterTile.Where(_ => IsSuccess()).Subscribe(_ => DoWhenSuccess()).AddTo(this);
            ExitTile.Subscribe(_ => _spriteGlowEffect.enabled = false).AddTo(this);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override async UniTask Initialize(BottleData bottleData)
        {
            _spriteGlowEffect = GetComponent<SpriteGlowEffect>();
            _spriteGlowEffect.enabled = false;

            // parse data
            _goalColor = bottleData.goalColor;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.goalColor.GetTileAddress());

            // GoalColorがNoneではないことを保証する
            if (_goalColor == EGoalColor.None) UIManager.Instance.ShowErrorMessage(EErrorCode.InvalidBottleColor);
            
            await base.Initialize(bottleData);

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

            // ボトルのスプライトを設定
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(bottleData.goalColor.GetBottleAddress()));
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
            return _goalColor != EGoalColor.None && BoardManager.Instance.GetTileColor(this) == _goalColor;
        }
    }
}

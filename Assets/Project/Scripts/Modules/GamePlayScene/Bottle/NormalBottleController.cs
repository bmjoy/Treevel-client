using SpriteGlow;
using System;
using TouchScript.Gestures;
using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Tile;
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
        /// 目標位置
        /// </summary>
        private int _targetPos;

        /// <summary>
        /// 光らせるエフェクト
        /// </summary>
        private SpriteGlowEffect _spriteGlowEffect;

        private CompositeDisposable eventDisposable = new CompositeDisposable();

        protected override void Awake()
        {
            base.Awake();
            longPressGesture = GetComponent<LongPressGesture>();
            longPressGesture.TimeToPress = 0.15f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnterTile.Subscribe(_ => {
                if (IsSuccess()) {
                    DoWhenSuccess();
                }
            }).AddTo(eventDisposable, this);
            ExitTile.Subscribe(_ => {
                _spriteGlowEffect.enabled = false;
            }).AddTo(eventDisposable, this);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override async void Initialize(BottleData bottleData)
        {
            _spriteGlowEffect = GetComponent<SpriteGlowEffect>();
            _spriteGlowEffect.enabled = false;

            // parse data
            var finalPos = bottleData.targetPos;
            _targetPos = finalPos;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);

            base.Initialize(bottleData);

            // set handler
            var lifeEffect = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_EFFECT_PREFAB).Task;
            lifeEffect.GetComponent<LifeEffectController>().Initialize(this, bottleData.life);
            if (bottleData.isDark) {
                var darkEffect = await AddressableAssetManager.Instantiate(Constants.Address.DARK_EFFECT_PREFAB).Task;
                darkEffect.GetComponent<DarkEffectController>().Initialize(this);
            }

            #if UNITY_EDITOR
            name = Constants.BottleName.NORMAL_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.Instance.GetTile(finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
            finalTile.GetComponent<SpriteRendererUnifier>().Unify();
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
            return _targetPos != 0 && BoardManager.Instance.GetBottlePos(this) == _targetPos;
        }

        protected override void EndProcess()
        {
            base.EndProcess();
            eventDisposable.Dispose();
        }
    }
}

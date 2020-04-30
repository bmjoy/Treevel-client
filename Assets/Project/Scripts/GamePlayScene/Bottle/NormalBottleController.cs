using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using SpriteGlow;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(PostProcessVolume))]
    [RequireComponent(typeof(SpriteGlowEffect))]
    public class NormalBottleController : DynamicBottleController
    {
        /// <summary>
        /// ボトルの初期位置
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override void Initialize(BottleData bottleData)
        {
            // parse data
            Id = bottleData.initPos;
            var finalPos = bottleData.targetPos;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);

            // set handlers
            _getDamagedHandler = new NormalGetDamagedHandler(this);
            _successHandler = new NormalBottleSuccessHandler(this, finalPos);
            _enterTileHandler = new NormalEnterTileHandler(this, _successHandler);

            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = BottleName.NUMBER_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.GetTile(finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected override void OnSucceed()
        {
            base.OnSucceed();
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected override void OnFail()
        {
            base.OnFail();
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の共通処理
        /// </summary>
        private void EndProcess()
        {
            // 自身が破壊されてない場合には，自身のアニメーションの繰り返しを停止
            if (!IsDead) {
                anim.wrapMode = WrapMode.Default;
            }
        }
    }
}

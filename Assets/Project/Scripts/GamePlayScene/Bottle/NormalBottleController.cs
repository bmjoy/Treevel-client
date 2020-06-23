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
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override void Initialize(BottleData bottleData)
        {
            // parse data
            var finalPos = bottleData.targetPos;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);

            // set handlers
            if (bottleData.life <= 1) {
                getDamagedHandler = new OneLifeBottleGetDamagedHandler(this);
            } else {
                getDamagedHandler = new MultiLifeBottleGetDamagedHandler(this, bottleData.life);
            }
            successHandler = new NormalBottleSuccessHandler(this, finalPos);
            bottleEnterTileHandler = new NormalBottleEnterTileHandler(this, successHandler);

            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = BottleName.NORMAL_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.GetTile(finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
        }
    }

    internal class NormalBottleEnterTileHandler : IBottleEnterTileHandler
    {
        private readonly AbstractBottleController _bottle;
        private readonly IBottleSuccessHandler _successHandler;

        internal NormalBottleEnterTileHandler(AbstractBottleController bottle, IBottleSuccessHandler successHandler)
        {
            if (successHandler == null)
                throw new System.NullReferenceException("SuccessHandler can not be null");

            _bottle = bottle;
            _successHandler = successHandler;
        }

        public void OnEnterTile(GameObject tile)
        {
            if (_successHandler.IsSuccess()) {
                // 最終タイルにいるかどうかで，光らせるかを決める
                _bottle.GetComponent<SpriteGlowEffect>().enabled = true;

                _successHandler.DoWhenSuccess();
            } else {
                _bottle.GetComponent<SpriteGlowEffect>().enabled = false;
            }
        }
    }

    internal class NormalBottleSuccessHandler : IBottleSuccessHandler
    {
        /// <summary>
        /// 目標位置
        /// </summary>
        private readonly int _targetPos;

        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        internal NormalBottleSuccessHandler(AbstractBottleController bottle, int targetPos)
        {
            _bottle = bottle;
            _targetPos = targetPos;
        }

        public void DoWhenSuccess()
        {
            // ステージの成功判定
            GameObject.FindObjectOfType<GamePlayDirector>().CheckClear();
        }

        public bool IsSuccess()
        {
            var currPos = BoardManager.GetBottlePos(_bottle);
            return currPos == _targetPos;
        }
    }
}

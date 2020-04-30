using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付きのナンバーボトル
    /// </summary>
    public class LifeBottleController : DynamicBottleController
    {
        public override void Initialize(BottleData bottleData)
        {
            // parse data
            var finalPos = bottleData.targetPos;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);

            // set handlers
            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
            _successHandler = new NormalBottleSuccessHandler(this, finalPos);
            _enterTileHandler = new NormalEnterTileHandler(this, _successHandler);

            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = BottleName.LIFE_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.GetTile(finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
        }
    }
}

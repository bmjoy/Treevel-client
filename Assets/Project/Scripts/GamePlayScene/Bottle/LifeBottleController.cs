using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付きのナンバーボトル
    /// </summary>
    public class LifeBottleController : NormalBottleController
    {

        public override void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);

            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
        }
    }
}

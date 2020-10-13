using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付き、成功判定なし、フリック可能なボトル
    /// </summary>
    public class AttackableDummyBottleController : DynamicBottleController
    {
        public override void Initialize(BottleData bottleData)
        {
            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = Constants.BottleName.ATTACKABLE_DUMMY_BOTTLE;
            #endif

            if (bottleData.life <= 1) {
                getDamagedHandler = new OneLifeBottleGetDamagedHandler(this);
            } else {
                getDamagedHandler = new MultiLifeBottleGetDamagedHandler(this, bottleData.life);
            }
        }
    }
}

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付き、成功判定なし、フリック可能なボトル
    /// </summary>
    public class LifeDummyBottleController : DynamicBottleController
    {
        public override void Initialize(GameDatas.BottleData bottleData)
        {
            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = Utils.Definitions.BottleName.LIFE_DUMMY_BOTTLE;
            #endif

            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
        }
    }
}

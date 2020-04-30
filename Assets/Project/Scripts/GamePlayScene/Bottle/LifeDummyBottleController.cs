namespace Project.Scripts.GamePlayScene.Bottle
{
    public class LifeDummyBottleController : DynamicBottleController
    {
        public override void Initialize(GameDatas.BottleData bottleData)
        {
            base.Initialize(bottleData);

            _getDamagedHandler = new LifeBottleGetDamagedHandler(this, bottleData.life);
        }
    }
}

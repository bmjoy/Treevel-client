using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class StaticBottleController : AbstractBottleController
    {
        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = BottleName.STATIC_DUMMY_BOTTLE;
            #endif
        }
    }
}

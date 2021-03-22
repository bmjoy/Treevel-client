using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class StaticBottleController : BottleControllerBase
    {
        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = Constants.BottleName.STATIC_DUMMY_BOTTLE;
            #endif
        }
    }
}

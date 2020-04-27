using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class StaticPanelController : AbstractPanelController
    {
        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = PanelName.STATIC_DUMMY_PANEL;
            #endif
        }
    }
}

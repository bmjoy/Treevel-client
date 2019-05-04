using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class StaticPanelController : PanelController
	{
		protected override void Awake()
		{
			base.Awake();
			name = PanelName.STATIC_DUMMY_PANEL;
		}
	}
}

using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeController : CartridgeController
	{
		// コンストラクタがわりのメソッド
		public override void Initialize(CartridgeDirection direction, int line)
		{
			speed = 0.1f;
			localScale = (float) (WindowSize.WIDTH * 0.15);

			SetInitialPosition(direction, line);
		}
	}
}

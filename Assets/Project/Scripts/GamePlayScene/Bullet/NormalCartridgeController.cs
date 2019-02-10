using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeController : CartridgeController
	{
		// コンストラクタがわりのメソッド
		public override void Initialize(CartridgeDirection direction, int line)
		{
			SetInitialPosition(direction, line);
		}
	}
}

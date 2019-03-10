using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeController : CartridgeController
	{
		// コンストラクタがわりのメソッド
		public override void Initialize(CartridgeDirection direction, int line, Vector2 motionVector)
		{
			this.motionVector = motionVector;
			SetInitialPosition(direction, line);
		}
	}
}

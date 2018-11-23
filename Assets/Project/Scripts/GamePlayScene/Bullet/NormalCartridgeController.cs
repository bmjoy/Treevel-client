using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeController : CartridgeController
	{
		// コンストラクタがわりのメソッド
		public void Initialize(Vector2 position, Vector2 motionVector)
		{
			speed = 0.1f;
			originalWidth = GetComponent<Renderer>().bounds.size.x;
			originalHeight = GetComponent<Renderer>().bounds.size.y;
			localScale = (float) (WindowSize.WIDTH * 0.15);

			SetInitialPosition(position, motionVector);
		}
	}
}

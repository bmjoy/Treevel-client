using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class NormalCartridgeWarningController : CartridgeWarningController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale =
				new Vector2(CartridgeWarningSize.WIDTH / originalWidth, CartridgeWarningSize.HEIGHT / originalHeight);
		}

		public override void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale,
			float bulletWidth, float bulletHeight)
		{
			transform.position = bulletPosition + Vector2.Scale(bulletMotionVector,
				                     new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y));
		}
	}
}

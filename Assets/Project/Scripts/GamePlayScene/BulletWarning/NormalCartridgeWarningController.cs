using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class NormalCartridgeWarningController : CartridgeWarningController
	{
		public override void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale,
			float bulletWidth, float bulletHeight)
		{
			localScale = (float) (WindowSize.WIDTH * 0.15);
			originalWidth = GetComponent<SpriteRenderer>().bounds.size.x;
			originalHeight = GetComponent<SpriteRenderer>().bounds.size.y;
			transform.localScale *= new Vector2(localScale, localScale);
			transform.position = bulletPosition + Vector2.Scale(bulletMotionVector,
				                     new Vector2((bulletLocalScale * bulletWidth + localScale * originalWidth) / 2,
					                     (bulletLocalScale * bulletHeight + localScale * originalHeight) / 2));
		}
	}
}

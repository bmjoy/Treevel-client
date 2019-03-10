using System.Collections;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class CartridgeWarningController : BulletWarningController
	{
		public abstract Vector2 Initialize(CartridgeDirection direction, int line);

		public IEnumerator DeleteWarning()
		{
			yield return new WaitForSeconds(WARNING_DISPLAYED_TIME);
			Destroy(gameObject);
		}
	}
}

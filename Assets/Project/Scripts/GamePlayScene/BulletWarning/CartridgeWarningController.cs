using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class CartridgeWarningController : BulletWarningController
	{
		public abstract Vector2 Initialize(CartridgeDirection direction, int line);
	}
}

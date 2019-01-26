using System.Collections;
using Project.Scripts.GamePlayScene.Bullet;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class HoleWarningController : BulletWarningController
	{
		public virtual void Initialize(int row, int column)
		{
			gameObject.GetComponent<Renderer>().sortingLayerName = "BulletWarning";
		}

		public abstract void DeleteWarning(GameObject hole);

	}
}

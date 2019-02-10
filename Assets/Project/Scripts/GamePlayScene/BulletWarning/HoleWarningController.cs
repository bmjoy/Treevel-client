using System.Collections;
using Project.Scripts.GamePlayScene.Bullet;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class HoleWarningController : BulletWarningController
	{
		public abstract void Initialize(int row, int column);

		public void DeleteWarning(GameObject hole)
		{
			StartCoroutine(Delete(hole));
		}

		private IEnumerator Delete(GameObject hole)
		{
			yield return new WaitForSeconds(WARNING_DISPLAYED_TIME);
			Destroy(gameObject);
			hole.SetActive(true);
		}
	}
}

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

		public void DeleteWarning(GameObject hole)
		{
			StartCoroutine(Delete(hole));
		}

		private IEnumerator Delete(GameObject hole)
		{
			yield return new WaitForSeconds(BulletGenerator.warningDisplayedTime);
			hole.SetActive(true);
			Destroy(gameObject);
		}
	}
}

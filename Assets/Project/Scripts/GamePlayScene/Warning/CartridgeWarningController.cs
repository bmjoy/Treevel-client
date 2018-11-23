using System.Collections;
using GamePlayScene.Bullet;
using UnityEngine;

namespace GamePlayScene.Warning
{
	public class CartridgeWarningController : BulletWarningController
	{
		public float localScale;

		// 元画像のサイズ
		public float originalHeight;
		public float originalWidth;

		private void OnEnable()
		{
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;
		}

		private void OnDisable()
		{
			GamePlayDirector.OnSucceed -= OnSucceed;
			GamePlayDirector.OnFail -= OnFail;
		}

		private void OnFail()
		{
			Destroy(gameObject);
		}

		private void OnSucceed()
		{
			Destroy(gameObject);
		}


		public void DeleteWarning(GameObject bullet)
		{
			var tempSpeed = bullet.GetComponent<CartridgeController>().speed;
			// the bullet does not move while warning is existing
			bullet.GetComponent<CartridgeController>().speed = 0.0f;
			StartCoroutine(Delete(bullet, tempSpeed));
		}

		private IEnumerator Delete(GameObject bullet, float speed)
		{
			yield return new WaitForSeconds(1.0f);
			bullet.GetComponent<CartridgeController>().speed = speed;
			Destroy(gameObject);
		}
	}
}

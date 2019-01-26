using System.Collections;
using Project.Scripts.GamePlayScene.Bullet;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class CartridgeWarningController : BulletWarningController
	{

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

		public virtual void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale,
			float bulletWidth, float bulletHeight)
		{
			gameObject.GetComponent<Renderer>().sortingLayerName = "BulletWarning";
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

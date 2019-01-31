using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class BulletWarningController : MonoBehaviour
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
	}
}

using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class BulletWarningController : MonoBehaviour
	{
		// 警告画像の表示時間
		public const float WARNING_DISPLAYED_TIME = 1.0f;

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

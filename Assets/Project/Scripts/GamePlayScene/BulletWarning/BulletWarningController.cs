using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class BulletWarningController : MonoBehaviour
	{
		// 警告画像の表示時間
		public const float WARNING_DISPLAYED_TIME = 1.0f;

		// 元画像のサイズ
		protected float originalHeight;
		protected float originalWidth;

		protected virtual void Awake()
		{
			originalWidth = GetComponent<SpriteRenderer>().size.x;
			originalHeight = GetComponent<SpriteRenderer>().size.y;
		}

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

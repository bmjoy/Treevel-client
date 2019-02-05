using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	// Bulletに共通したフィールド、メソッドの定義
	public abstract class BulletController : MonoBehaviour
	{
		public float localScale;

		// 元画像のサイズ
		public float originalWidth;
		public float originalHeight;

		protected abstract void OnEnable();

		protected void OnDisable()
		{
			GamePlayDirector.OnSucceed -= OnSucceed;
			GamePlayDirector.OnFail -= OnFail;
		}

		protected abstract void OnFail();

		protected void OnSucceed()
		{
			Destroy(gameObject);
		}
	}
}

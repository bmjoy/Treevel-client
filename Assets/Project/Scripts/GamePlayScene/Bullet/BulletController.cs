using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	// Bulletに共通したフィールド、メソッドの定義
	public abstract class BulletController : MonoBehaviour
	{
		[System.NonSerialized] public float localScale = 1.0f;

		// 元画像のサイズ
		public float originalWidth;
		public float originalHeight;

		protected virtual void Awake()
		{
			originalWidth = GetComponent<SpriteRenderer>().size.x;
			originalHeight = GetComponent<SpriteRenderer>().size.y;
		}

		protected abstract void OnEnable();

		private void OnDisable()
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

using UnityEngine;
using System;

namespace Project.Scripts.GamePlayScene.Bullet
{
	// Bulletに共通したフィールド、メソッドの定義
	public abstract class BulletController : MonoBehaviour
	{
		[NonSerialized] public const float LOCAL_SCALE = 1.0f;

		// 元画像のサイズ
		public float originalWidth;
		public float originalHeight;

		protected static Vector2 Abs(Vector2 v)
		{
			return new Vector2(Math.Abs(v.x), Math.Abs(v.y));
		}

		protected static Vector2 Transposition(Vector2 v)
		{
			return new Vector2(v.y, v.x);
		}

		protected virtual void Awake()
		{
			originalWidth = GetComponent<SpriteRenderer>().size.x;
			originalHeight = GetComponent<SpriteRenderer>().size.y;
			// sortingLayerの設定
			gameObject.GetComponent<Renderer>().sortingLayerName = "Bullet";
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

		private void OnSucceed()
		{
			Destroy(gameObject);
		}

		protected abstract void OnFail();
	}
}

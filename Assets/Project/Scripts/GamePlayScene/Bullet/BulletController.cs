using UnityEngine;
using System;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
	// Bulletに共通したフィールド、メソッドの定義
	public abstract class BulletController : MonoBehaviour
	{
		[NonSerialized] protected const float LOCAL_SCALE = 1.0f;

		// 元画像のサイズ
		public float originalWidth;
		public float originalHeight;

		// ベクトルの絶対値を返す関数
		protected static Vector2 Abs(Vector2 v)
		{
			return new Vector2(Math.Abs(v.x), Math.Abs(v.y));
		}

		// ベクトルの転置を返す関数
		protected static Vector2 Transposition(Vector2 v)
		{
			return new Vector2(v.y, v.x);
		}

		// 座標から行(row)と列(column)を返す関数
		protected static void GetRowAndColumn(out int row, out int column, Vector2 position)
		{
			// 最上タイルのy座標
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			row = (int) ((topTilePositionY - position.y) / TileSize.HEIGHT) + 1;
			column = (int) ((position.x / TileSize.WIDTH) + 1) + 1;
		}

		protected static Vector2 Rotate(Vector2 v, float angle)
		{
			return new Vector2((float) (Math.Cos(angle) * v.x - Math.Sin(angle) * v.y),
				(float) (Math.Sin(angle) * v.x + Math.Cos(angle) * v.y));
		}

		protected virtual void Awake()
		{
			originalWidth = GetComponent<SpriteRenderer>().size.x;
			originalHeight = GetComponent<SpriteRenderer>().size.y;
			// sortingLayerの設定
			gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.BULLET;
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

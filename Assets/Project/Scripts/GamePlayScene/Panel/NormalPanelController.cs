using System;
using Project.Scripts.GamePlayScene.Tile;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class NormalPanelController : PanelController
	{
		// 最終タイル
		private GameObject finalTile;

		// パネルが最終タイルにいるかどうかの状態
		public bool adapted;

		protected override void Start()
		{
			base.Start();
			// 当たり判定をパネルサイズと同等にする
			Vector2 panelSize = transform.localScale * 2f;
			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			collider.size = panelSize;
			// 初期状態で最終タイルにいるかどうかの状態を変える
			adapted = transform.parent.gameObject == finalTile;
		}

		public override void Initialize(GameObject finalTile)
		{
			this.finalTile = finalTile;
		}

		private void OnEnable()
		{
			// 当たり判定と，フリック検知のアタッチ（いちいちUIで設定したくない）
			gameObject.AddComponent<BoxCollider2D>();
			gameObject.AddComponent<FlickGesture>();
			GetComponent<FlickGesture>().Flicked += HandleFlick;
			// フリックの検知感度を変えたい際に変更可能
			GetComponent<FlickGesture>().MinDistance = 0.5f;
			GetComponent<FlickGesture>().FlickTime = 0.05f;
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;
		}

		private void OnDisable()
		{
			GetComponent<FlickGesture>().Flicked -= HandleFlick;
			GamePlayDirector.OnSucceed -= OnSucceed;
			GamePlayDirector.OnFail -= OnFail;
		}

		private void HandleFlick(object sender, EventArgs e)
		{
			FlickGesture gesture = sender as FlickGesture;

			if (gesture.State != FlickGesture.GestureState.Recognized) return;

			// 親タイルオブジェクトのスクリプトを取得
			TileController parentTile = transform.parent.gameObject.GetComponent<TileController>();
			// フリック方向
			var x = gesture.ScreenFlickVector.x;
			var y = gesture.ScreenFlickVector.y;

			// 方向検知に加えて，上下と左右の変化量を比べることで，検知精度をあげる
			if (x > 0 && Math.Abs(x) >= Math.Abs(y))
			{
				// 右
				GameObject rightTile = parentTile.rightTile;
				UpdateTIle(rightTile);
			}
			else if (x < 0 && Math.Abs(x) >= Math.Abs(y))
			{
				// 左
				GameObject leftTile = parentTile.leftTile;
				UpdateTIle(leftTile);
			}
			else if (y > 0 && Math.Abs(y) >= Math.Abs(x))
			{
				// 上
				GameObject upperTile = parentTile.upperTile;
				UpdateTIle(upperTile);
			}
			else if (y < 0 && Math.Abs(y) >= Math.Abs(x))
			{
				// 下
				GameObject lowerTile = parentTile.lowerTile;
				UpdateTIle(lowerTile);
			}
		}

		private void UpdateTIle(GameObject targetTile)
		{
			// 移動先にタイルがなければ何もしない
			if (targetTile == null) return;
			// 移動先のタイルに子パネルがあれば何もしない
			if (targetTile.transform.childCount != 0) return;
			// 親タイルの更新
			transform.parent = targetTile.transform;
			// 親タイルへ移動
			transform.position = transform.parent.position;
			// 最終タイルにいるかどうかで状態を変える
			adapted = transform.parent.gameObject == finalTile;
			// 成功判定
			gamePlayDirector.CheckClear();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// 銃弾との衝突以外は考えない（現状は，パネル同士での衝突は起こりえない）
			if (!other.gameObject.CompareTag("Bullet")) return;
			gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
			// 失敗状態に移行する
			gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
		}

		private void OnSucceed()
		{
			GetComponent<FlickGesture>().Flicked -= HandleFlick;
		}

		private void OnFail()
		{
			GetComponent<FlickGesture>().Flicked -= HandleFlick;
		}
	}
}

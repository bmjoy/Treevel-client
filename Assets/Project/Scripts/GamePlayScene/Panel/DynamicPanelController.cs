using System;
using Project.Scripts.GamePlayScene.Tile;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class DynamicPanelController : PanelController
	{
		protected GamePlayDirector gamePlayDirector;

		protected override void Awake()
		{
			base.Awake();
			name = "DynamicDummyPanel";
			// 当たり判定と，フリック検知のアタッチ
			gameObject.AddComponent<BoxCollider2D>();
			gameObject.AddComponent<FlickGesture>();
			// フリックの検知感度を変えたい際に変更可能
			GetComponent<FlickGesture>().MinDistance = 0.2f;
			GetComponent<FlickGesture>().FlickTime = 0.2f;
		}

		protected virtual void Start()
		{
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
		}

		private void OnEnable()
		{
			GetComponent<FlickGesture>().Flicked += HandleFlick;
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
			var gesture = sender as FlickGesture;

			if (gesture.State != FlickGesture.GestureState.Recognized) return;

			// 親タイルオブジェクトのスクリプトを取得
			var parentTile = transform.parent.gameObject.GetComponent<NormalTileController>();
			// フリック方向
			var x = gesture.ScreenFlickVector.x;
			var y = gesture.ScreenFlickVector.y;

			// 方向検知に加えて，上下と左右の変化量を比べることで，検知精度をあげる
			if (x > 0 && Math.Abs(x) >= Math.Abs(y))
			{
				// 右
				var rightTile = parentTile.rightTile;
				UpdateTile(rightTile);
			}
			else if (x < 0 && Math.Abs(x) >= Math.Abs(y))
			{
				// 左
				var leftTile = parentTile.leftTile;
				UpdateTile(leftTile);
			}
			else if (y > 0 && Math.Abs(y) >= Math.Abs(x))
			{
				// 上
				var upperTile = parentTile.upperTile;
				UpdateTile(upperTile);
			}
			else if (y < 0 && Math.Abs(y) >= Math.Abs(x))
			{
				// 下
				var lowerTile = parentTile.lowerTile;
				UpdateTile(lowerTile);
			}
		}

		protected virtual void UpdateTile(GameObject targetTile)
		{
			// 移動先にタイルがなければ何もしない
			if (targetTile == null) return;
			// 移動先のタイルに子パネルがあれば何もしない
			if (targetTile.transform.childCount != 0) return;
			// 親タイルの更新
			transform.parent = targetTile.transform;
			// 親タイルへ移動
			transform.position = transform.parent.position;

			// ワープタイルの場合には追加処理を施す
			var warpTileController = targetTile.GetComponent<WarpTileController>();
			if (warpTileController != null)
			{
				// ワープ先が移動可能なタイルかどうか
				if (warpTileController.pairTile.transform.childCount != 0)
				{
					transform.parent = warpTileController.pairTile.transform;
					transform.position = transform.parent.position;
				}
			}
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

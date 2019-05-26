using Project.Scripts.Utils.Definitions;
using UnityEngine;
using System;
using Project.Scripts.GamePlayScene.Bullet;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class CartridgeWarningController : BulletWarningController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale =
				new Vector2(CartridgeWarningSize.WIDTH / originalWidth, CartridgeWarningSize.HEIGHT / originalHeight);
		}

		// 警告のpositionを計算する
		// 銃弾の移動方向(bulletMotionVector)が副次的に計算されるので、その値を返す
		public Vector2 Initialize(CartridgeType cartridgeType, ref CartridgeDirection direction, ref int line,
			BulletInfo bulletInfo)
		{
			switch (cartridgeType)
			{
				case CartridgeType.Normal:
					break;
				case CartridgeType.Turn:
					var sprite = Resources.Load<Sprite>("Textures/BulletWarning/turnWarning");
					gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
					break;
				default:
					throw new NotImplementedException();
			}

			if (direction == CartridgeDirection.Random)
			{
				direction = bulletInfo.GetCartridgeDirection();
			}

			Vector2 bulletMotionVector;
			Vector2 warningPosition;
			switch (direction)
			{
				case CartridgeDirection.ToLeft:
					if (line == (int) Row.Random)
					{
						line = bulletInfo.GetCartridgeRow();
					}

					warningPosition = new Vector2(WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.left;
					break;
				case CartridgeDirection.ToRight:
					if (line == (int) Row.Random)
					{
						line = bulletInfo.GetCartridgeRow();
					}

					warningPosition = new Vector2(-WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.right;
					break;
				case CartridgeDirection.ToUp:
					if (line == (int) Column.Random)
					{
						line = bulletInfo.GetCartridgeColumn();
					}

					warningPosition = new Vector2(TileSize.WIDTH * (line - 2),
						-WindowSize.HEIGHT / 2);
					bulletMotionVector = Vector2.up;
					break;
				case CartridgeDirection.ToBottom:
					if (line == (int) Column.Random)
					{
						line = bulletInfo.GetCartridgeColumn();
					}

					warningPosition = new Vector2(TileSize.WIDTH * (line - 2),
						WindowSize.HEIGHT / 2);
					bulletMotionVector = Vector2.down;
					break;
				default:
					throw new NotImplementedException();
			}

			warningPosition = warningPosition + Vector2.Scale(bulletMotionVector,
				                  new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y)) / 2;
			transform.position = warningPosition;
			return bulletMotionVector;
		}

		// 警告の座標と画像を引数に受け取る
		public void Initialize(Vector2 position, string imageName)
		{
			transform.position = position;
			var sprite = Resources.Load<Sprite>("Textures/BulletWarning/" + imageName);
			gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}
}

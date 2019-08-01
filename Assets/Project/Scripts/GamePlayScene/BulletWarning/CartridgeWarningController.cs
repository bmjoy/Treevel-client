using Project.Scripts.Utils.Definitions;
using UnityEngine;
using System;

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
		public Vector2 Initialize(ECartridgeType cartridgeType, ECartridgeDirection direction, int line)
		{
			switch (cartridgeType)
			{
				case ECartridgeType.Normal:
					break;
				case ECartridgeType.Turn:
					var sprite = Resources.Load<Sprite>("Textures/BulletWarning/turnWarning");
					gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
					break;
				default:
					throw new NotImplementedException();
			}

			Vector2 bulletMotionVector;
			Vector2 warningPosition;
			// Cartridgeの進行方向によってWarningの表示位置を求める
			switch (direction)
			{
				case ECartridgeDirection.ToLeft:
					warningPosition = new Vector2(WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.left;
					break;
				case ECartridgeDirection.ToRight:
					warningPosition = new Vector2(-WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.right;
					break;
				case ECartridgeDirection.ToUp:
					warningPosition = new Vector2(TileSize.WIDTH * (line - 2),
						-WindowSize.HEIGHT / 2);
					bulletMotionVector = Vector2.up;
					break;
				case ECartridgeDirection.ToBottom:
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

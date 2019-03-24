using System;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class TurnCartridgeController : NormalCartridgeController
	{
		// 回転方向
		private int[] turnDirection;

		// 回転する列(or行)
		private int[] turnLine;

		// 回転に関する警告を表示する座標
		private Vector2 turnPoint;

		// 回転角度
		private float turnAngle;

		// 回転に要するフレーム数
		private const int COUNT = 30;

		// 回転に要しているフレーム数
		private int rotateCount = -1;

		// 回転中の銃弾の速さ
		private float rotatingSpeed = (PanelSize.WIDTH - CartridgeSize.WIDTH) / COUNT;

		public GameObject normalCartridgeWarningPrefab;
		private GameObject warning;

		protected override void FixedUpdate()
		{
			// 警告を表示する
			if (rotateCount == -1 && turnPoint.x - (PanelSize.WIDTH - CartridgeSize.WIDTH) - speed * 60 <=
			    transform.position.x)
			{
				warning = Instantiate(normalCartridgeWarningPrefab);
				// 同レイヤーのオブジェクトの描画順序の制御
				warning.GetComponent<Renderer>().sortingOrder = gameObject.GetComponent<Renderer>().sortingOrder;
				// warningの位置・大きさ等の設定
				var warningScript = warning.GetComponent<NormalCartridgeWarningController>();
				warningScript.Initialize(turnPoint, "TurnBottom");
				rotateCount++;
			}
			// 回転はじめ
			else if (rotateCount == 0 &&
			    turnPoint.x - (PanelSize.WIDTH - CartridgeSize.WIDTH) <= transform.position.x)
			{
				Destroy(warning);
				rotateCount++;
				transform.Translate(motionVector * speed, Space.World);
			}
			// 回転中
			else if (0 < rotateCount && rotateCount <= COUNT)
			{
				rotateCount++;
				motionVector =
					new Vector2((float) (Math.Cos(turnAngle) * motionVector.x - Math.Sin(turnAngle) * motionVector.y),
						(float) (Math.Sin(turnAngle) * motionVector.x + Math.Cos(turnAngle) * motionVector.y));
				transform.Rotate(new Vector3(0, 0, turnAngle / Mathf.PI * 180f), Space.World);
				transform.Translate(motionVector * rotatingSpeed, Space.World);
			}
			// 回転おわり
			else if (rotateCount == COUNT + 1)
			{
				// 別のタイル上で再度回転する場合
				if (turnLine.Length >= 2)
				{
					Array.Copy(turnDirection, 1, turnDirection, 0, turnDirection.Length);
					Array.Copy(turnLine, 1, turnLine, 0, turnLine.Length);
					turnPoint = transform.position * orthogonalMotionVector + new Vector2(
						            TileSize.WIDTH * (turnLine[0] - 2),
						            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						            TileSize.HEIGHT * (turnLine[0] - 1)) * motionVector;
					turnAngle = turnDirection[0] == 1 ? -90 : 90;
					turnAngle = turnAngle / COUNT / 180.0f * Mathf.PI;
					rotateCount = 0;
				}
				// 回転せず直進するのみの場合
				else
				{
					rotateCount = -2;
				}
				transform.Translate(motionVector * speed, Space.World);
			}
			// 回転中以外
			else
			{
				transform.Translate(motionVector * speed, Space.World);
			}
		}

		public override void Initialize(CartridgeDirection direction, int line, Vector2 motionVector,
			int[,] additionalInfo)
		{
			Initialize(direction, line, motionVector);

			// どのタイル上でどの方向に曲がるかの引数を受け取る
			turnDirection = new int[additionalInfo.GetLength(0)];
			turnLine = new int[additionalInfo.GetLength(0)];
			for (var i = 0; i < additionalInfo.GetLength(0); i++)
			{
				turnDirection[i] = additionalInfo[i, 0];
				turnLine[i] = additionalInfo[i, 1];
			}

			// 銃弾が曲がるタイルの座標
			turnPoint = transform.position * orthogonalMotionVector + new Vector2(TileSize.WIDTH * (turnLine[0] - 2),
				            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
				            TileSize.HEIGHT * (turnLine[0] - 1)) * motionVector;
			// 回転角度
			turnAngle = turnDirection[0] == 1 ? -90 : 90;
			turnAngle = turnAngle / COUNT / 180.0f * Mathf.PI;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
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

		// 回転方向に応じて表示わけする警告画像の名前
		private readonly string[] warningList = {"turnLeft", "turnRight", "turnUp", "turnBottom"};

		// 警告
		public GameObject normalCartridgeWarningPrefab;
		private GameObject warning;

		// １フレームあたりの回転角度
		private float turnAngle;

		// 回転に要するフレーム数
		private const int COUNT = 50;

		// 回転の何フレーム目か
		private int rotateCount = -1;

		// 回転中の銃弾の速さ (円周(回転半径 * 2 * pi)の4分の1をフレーム数で割る)
		private float rotatingSpeed =
			((PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) * 2f * (float) Math.PI / 4f / COUNT;

		protected override void FixedUpdate()
		{
			// 回転する座標に近づいたら警告を表示する
			if (rotateCount == -1 && Vector2.Distance(turnPoint, transform.position) <=
			    (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f + speed * 50)
			{
				warning = Instantiate(normalCartridgeWarningPrefab);
				// 同レイヤーのオブジェクトの描画順序の制御
				warning.GetComponent<Renderer>().sortingOrder = gameObject.GetComponent<Renderer>().sortingOrder;
				// warningの位置・大きさ等の設定
				var warningScript = warning.GetComponent<CartridgeWarningController>();
				warningScript.Initialize(turnPoint, warningList[turnDirection[0]]);
				rotateCount++;
				transform.Translate(motionVector * speed, Space.World);
			}
			// 回転はじめのフレーム
			else if (rotateCount == 0 && Vector2.Distance(turnPoint, transform.position) <=
			         (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f)
			{
				Destroy(warning);
				rotateCount++;
				motionVector = Rotate(motionVector, turnAngle / 2f);
				transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
				transform.Translate(motionVector * rotatingSpeed, Space.World);
			}
			// 回転中のフレーム
			else if (0 < rotateCount && rotateCount <= COUNT - 1)
			{
				rotateCount++;
				motionVector = Rotate(motionVector, turnAngle);
				transform.Rotate(new Vector3(0, 0, turnAngle / Mathf.PI * 180f), Space.World);
				transform.Translate(motionVector * rotatingSpeed, Space.World);
			}
			// 回転おわりのフレーム
			else if (rotateCount == COUNT)
			{
				transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
				motionVector = Rotate(motionVector, turnAngle / 2f);
				// 別のタイル上でまだ回転する場合
				if (turnDirection.Length >= 2)
				{
					// 配列の先頭要素を除く部分配列を取得する
					turnDirection = turnDirection.Skip(1).Take(turnDirection.Length - 1).ToArray();
					turnLine = turnLine.Skip(1).Take(turnLine.Length - 1).ToArray();

					turnPoint = transform.position * Abs(Transposition(motionVector)) + new Vector2(
						            TileSize.WIDTH * (turnLine[0] - 2),
						            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						            TileSize.HEIGHT * (turnLine[0] - 1)) * Abs(motionVector);
					turnAngle = turnDirection[0] % 2 == 1 ? -90 : 90;
					turnAngle = (motionVector.x + motionVector.y) * turnAngle;
					turnAngle = turnAngle / COUNT / 180.0f * Mathf.PI;
					rotateCount = -1;
				}
				// 回転しない場合
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

		public void Initialize(CartridgeDirection direction, int line, Vector2 motionVector,
			Dictionary<string, int[]> additionalInfo)
		{
			Initialize(direction, line, motionVector);

			// どのタイル上でどの方向に曲がるかの引数を受け取る
			turnDirection = additionalInfo["TurnDirection"];
			turnLine = additionalInfo["TurnLine"];

			// 銃弾が曲がるタイルの座標
			turnPoint = transform.position * Abs(Transposition(motionVector)) + new Vector2(
				            TileSize.WIDTH * (turnLine[0] - 2),
				            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
				            TileSize.HEIGHT * (turnLine[0] - 1)) * Abs(motionVector);
			// 回転角度
			turnAngle = turnDirection[0] % 2 == 1 ? -90 : 90;
			turnAngle = (motionVector.x + motionVector.y) * turnAngle;
			turnAngle = turnAngle / COUNT / 180.0f * Mathf.PI;
		}

		protected override void OnFail()
		{
			base.OnFail();
			rotatingSpeed = 0;
			turnAngle = 0;
		}
	}
}

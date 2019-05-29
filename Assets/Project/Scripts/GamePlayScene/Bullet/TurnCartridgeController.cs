﻿using System;
using System.Linq;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class TurnCartridgeController : NormalCartridgeController
	{
		// 回転方向の配列
		private int[] turnDirection;
		// 次に曲がる方向
		private int nextTurnDirection;

		// 回転する行(or列)の配列
		private int[] turnLine;
		// 次に回転する行(or列)
		private int nextTurnLine;

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
				warningScript.Initialize(turnPoint, warningList[nextTurnDirection - 1]);
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
					nextTurnDirection = turnDirection[0];
					nextTurnLine = turnLine[0];
					turnPoint = transform.position * Abs(Transposition(motionVector)) + new Vector2(
						            TileSize.WIDTH * (nextTurnLine - 2),
						            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						            TileSize.HEIGHT * (nextTurnLine - 1)) * Abs(motionVector);
					turnAngle = nextTurnDirection % 2 == 1 ? 90 : -90;
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
			BulletInfo bulletInfo)
		{
			Initialize(direction, line, motionVector);

			// どのタイル上でどの方向に曲がるかの引数を受け取る
			turnDirection = bulletInfo.GetTurnDirection();
			if (turnDirection == null)
			{
				// 次に曲がる方向をランダムに決める場合
				nextTurnDirection = bulletInfo.GetRandomTurnDirection(direction: direction, line: line);
				turnDirection = new int[] {nextTurnDirection};
			}
			else
			{
				// 次に曲がる方向が引数として与えられている場合
				nextTurnDirection = turnDirection[0];
			}

			turnLine = bulletInfo.GetTurnLine();
			if (turnLine == null)
			{
				// 次に曲がる行(or列)をランダムに決める場合
				nextTurnLine = bulletInfo.GetRandomTurnLine(direction: direction);
				turnLine = new int[] {nextTurnLine};
			}
			else
			{
				// 次に曲がる行(or列)が引数として与えられている場合
				nextTurnLine = turnLine[0];
			}

			// 銃弾が曲がるタイルの座標
			turnPoint = transform.position * Abs(Transposition(motionVector)) + new Vector2(
				            TileSize.WIDTH * (nextTurnLine - 2),
				            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
				            TileSize.HEIGHT * (nextTurnLine - 1)) * Abs(motionVector);
			// 回転角度
			turnAngle = nextTurnDirection % 2 == 1 ? 90 : -90;
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

using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using UnityEngine;
using Project.Scripts.Library.Data;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGenerator : MonoBehaviour
	{
		// normalBullerのPrefab
		public GameObject normalBulletPrefab;
		public GameObject normalBulletWarningPrefab;

		// Generatorが作成された時刻
		public float startTime;

		public void CreateBullets(int stageId)
		{
			List<IEnumerator> coroutines = new List<IEnumerator>();
			startTime = Time.time;
			switch (stageId)
			{
				case 1:
					// 銃弾の初期位置
					const int positionNumber = 1;
					// 銃弾の移動ベクトル
					Vector2 motionVector = new Vector2(-1.0f, 0.0f);
					// 銃弾の出現時刻
					const float appearanceTiming = 1.0f;
					// 銃弾を作るインターバル
					const float createInterval = 1.0f;
					// coroutineのリスト
					coroutines.Add(CreateBullet(positionNumber, motionVector, appearanceTiming, createInterval));
					coroutines.Add(CreateBullet(positionNumber: 2,
						motionVector: new Vector2(1.0f, 0.0f),
						appearanceTiming: 2.0f, interval: 4.0f));
					break;
				case 2:
					coroutines.Add(CreateBullet(positionNumber: 5,
						motionVector: new Vector2(1.0f, 0.0f),
						appearanceTiming: 2.0f, interval: 4.0f));
					break;
				default:
					throw new NotImplementedException();
					break;
			}

			foreach (IEnumerator coroutine in coroutines) StartCoroutine(coroutine);
		}

		// 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
		private IEnumerator CreateBullet(int positionNumber, Vector2 motionVector, float appearanceTiming,
			float interval)
		{
			// タイルの位置に合わせて銃弾の初期位置を設定する
			Vector2 position = SetBulletPosition(positionNumber, motionVector);

			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTiming - 1.0f - (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			while (true)
			{
				sum++;
				// normalBulletPrefabのGameObjectを作成
				GameObject bullet = Instantiate(normalBulletPrefab);
				// SortingLayerを指定
				bullet.GetComponent<Renderer>().sortingLayerName = "Bullet";
				// 変数の初期設定
				NormalCartridgeController cartridgeScript = bullet.GetComponent<NormalCartridgeController>();
				cartridgeScript.Initialize(position, motionVector);

				// emerge a bullet warning
				GameObject warning = Instantiate(normalBulletWarningPrefab);
				warning.GetComponent<Renderer>().sortingLayerName = "Warning";
				NormalCartridgeWarningController warningScript =
					warning.GetComponent<NormalCartridgeWarningController>();
				warningScript.Initialize(bullet.transform.position, cartridgeScript.motionVector,
					cartridgeScript.localScale, cartridgeScript.originalWidth, cartridgeScript.originalHeight);

				// delete the bullet warning
				warningScript.DeleteWarning(bullet);

				currentTime = Time.time;
				// 一定時間(interval)待つ
				yield return new WaitForSeconds(appearanceTiming + interval * sum - (currentTime - startTime));
			}
		}

		// positionNumber行またはpositionNumber列目のタイルを通過するように銃弾の初期位置を指定する
		private static Vector2 SetBulletPosition(int positionNumber, Vector2 motionVector)
		{
			// 銃弾が左右方向に移動する場合
			// 入力が(1<=positionNumber<=5)行であるか調べる
			if ((motionVector.Equals(Vector2.left) || motionVector.Equals(Vector2.right)))
			{
				if (positionNumber < 1 || 5 < positionNumber)
				{
					throw new ArgumentOutOfRangeException("positionNumber", positionNumber, null);
				}
			}
			// 銃弾が上下方向に移動する場合
			// 入力が(1<=positionNumber<=3)列であるか調べる
			else if ((motionVector.Equals(Vector2.up) || motionVector.Equals(Vector2.down)))
			{
				if (positionNumber < 1 || 3 < positionNumber)
				{
					throw new ArgumentOutOfRangeException("positionNumber", positionNumber, null);
				}
			}

			return new Vector2(TileSize.WIDTH * (positionNumber - 2),
				WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
				TileSize.HEIGHT * (positionNumber - 1));
		}
	}
}

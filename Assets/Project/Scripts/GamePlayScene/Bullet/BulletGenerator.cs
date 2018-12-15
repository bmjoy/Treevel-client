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

		private enum Left
		{
			First = 1,
			Second,
			Third,
			Fourth,
			Fifth
		}

		private enum Right
		{
			First = 1,
			Second,
			Third,
			Fourth,
			Fifth
		}

		private enum Up
		{
			First = 1,
			Second,
			Third
		}

		private enum Down
		{
			First = 1,
			Second,
			Third
		}

		public void CreateBullets(int stageId)
		{
			List<IEnumerator> coroutines = new List<IEnumerator>();
			startTime = Time.time;
			switch (stageId)
			{
				case 1:
					// 銃弾の出現時刻
					const float appearanceTiming = 1.0f;
					// 銃弾を作るインターバル
					const float createInterval = 1.0f;
					// coroutineのリスト
					AddCoroutine(coroutines, Left.First, appearanceTiming, createInterval);
					AddCoroutine(coroutine: coroutines, row: Right.Second, appearanceTiming: 2.0f,
						createInterval: 4.0f);
					break;
				case 2:
					AddCoroutine(coroutine: coroutines, row: Right.Fifth, appearanceTiming: 2.0f, createInterval: 4.0f);
					break;
				default:
					throw new NotImplementedException();
					break;
			}

			foreach (IEnumerator coroutine in coroutines) StartCoroutine(coroutine);
		}

		// 銃弾の移動方向および出現させる位置を取得してCreatebullet()メソッドに引数を渡す
		private void AddCoroutine(List<IEnumerator> coroutine, Left row, float appearanceTiming, float createInterval)
		{
			coroutine.Add(CreateBullet("left", (int) row, appearanceTiming, createInterval));
		}

		private void AddCoroutine(List<IEnumerator> coroutine, Right row, float appearanceTiming, float createInterval)
		{
			coroutine.Add(CreateBullet("right", (int) row, appearanceTiming, createInterval));
		}

		private void AddCoroutine(List<IEnumerator> coroutine, Up column, float appearanceTiming, float createInterval)
		{
			coroutine.Add(CreateBullet("up", (int) column, appearanceTiming, createInterval));
		}

		private void AddCoroutine(List<IEnumerator> coroutine, Down column, float appearanceTiming,
			float createInterval)
		{
			coroutine.Add(CreateBullet("down", (int) column, appearanceTiming, createInterval));
		}

		// 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
		private IEnumerator CreateBullet(String direction, int position, float appearanceTiming,
			float interval)
		{
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
				cartridgeScript.Initialize(direction, position);

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
	}
}

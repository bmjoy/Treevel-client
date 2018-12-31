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

		public enum Direction
		{
			ToLeft,
			ToRight,
			ToUp,
			ToBottom
		}

		private enum ToLeft
		{
			First = 1,
			Second,
			Third,
			Fourth,
			Fifth
		}

		private enum ToRight
		{
			First = 1,
			Second,
			Third,
			Fourth,
			Fifth
		}

		private enum ToUp
		{
			First = 1,
			Second,
			Third
		}

		private enum ToDown
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
					const float appearanceTime = 1.0f;
					// 銃弾を作るインターバル
					const float createInterval = 1.0f;
					// coroutineのリスト
					coroutines.Add(CreateBullet(Direction.ToLeft, (int) ToLeft.First, appearanceTime, createInterval));
					coroutines.Add(CreateBullet(Direction.ToRight, (int) ToRight.Second, appearanceTime: 2.0f,
						interval: 4.0f));
					break;
				case 2:
					coroutines.Add(CreateBullet(Direction.ToRight, (int) ToRight.Fifth, appearanceTime: 2.0f, interval: 4.0f));
					break;
				default:
					throw new NotImplementedException();
			}

			foreach (var coroutine in coroutines) StartCoroutine(coroutine);
		}

		// 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
		private IEnumerator CreateBullet(Direction direction, int line, float appearanceTime,
			float interval)
		{
			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTime - 1.0f - (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			while (true)
			{
				sum++;
				// normalBulletPrefabのGameObjectを作成
				var bullet = Instantiate(normalBulletPrefab);
				// SortingLayerを指定
				bullet.GetComponent<Renderer>().sortingLayerName = "Bullet";
				// 変数の初期設定
				var cartridgeScript = bullet.GetComponent<NormalCartridgeController>();
				cartridgeScript.Initialize(direction, line);

				// emerge a bullet warning
				GameObject warning = Instantiate(normalBulletWarningPrefab);
				warning.GetComponent<Renderer>().sortingLayerName = "Warning";
				var warningScript =
					warning.GetComponent<NormalCartridgeWarningController>();
				warningScript.Initialize(bullet.transform.position, cartridgeScript.motionVector,
					cartridgeScript.localScale, cartridgeScript.originalWidth, cartridgeScript.originalHeight);

				// delete the bullet warning
				warningScript.DeleteWarning(bullet);

				currentTime = Time.time;
				// 一定時間(interval)待つ
				yield return new WaitForSeconds(appearanceTime + interval * sum - (currentTime - startTime));
			}
		}
	}
}

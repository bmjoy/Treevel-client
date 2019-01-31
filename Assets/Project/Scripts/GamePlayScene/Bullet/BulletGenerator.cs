using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Project.Scripts.GamePlayScene.BulletWarning;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGenerator : MonoBehaviour
	{
		// 警告画像の表示時間
		public static float warningDisplayedTime = 1.0f;

		// 銃弾および警告のprefab
		public GameObject normalCartridgePrefab;
		public GameObject normalCartridgeWarningPrefab;
		public GameObject normalHolePrefab;
		public GameObject normalHoleWarningPrefab;

		// Generatorが作成された時刻
		public float startTime;

		private enum BulletType
		{
			NormalCartridge,
			NormalHole
		}

		public enum BulletDirection
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

		private enum Row
		{
			First = 1,
			Second,
			Third,
			Fourth,
			Fifth
		}

		private enum Column
		{
			Left = 1,
			Center,
			Right
		}

		public void CreateBullets(int stageId)
		{
			List<IEnumerator> coroutines = new List<IEnumerator>();
			startTime = Time.time;
			switch (stageId)
			{
				case 1:
					// coroutineのリスト
					coroutines.Add(CreateBullet(BulletType.NormalCartridge, BulletDirection.ToLeft, (int) ToLeft.First,
						appearanceTime: 1.0f, interval: 1.0f));
					coroutines.Add(CreateBullet(BulletType.NormalCartridge, BulletDirection.ToRight,
						(int) ToRight.Second, appearanceTime: 2.0f, interval: 4.0f));
					break;
				case 2:
					coroutines.Add(CreateBullet(BulletType.NormalCartridge, BulletDirection.ToRight,
						(int) ToRight.Fifth, appearanceTime: 2.0f,
						interval: 4.0f));
					coroutines.Add(CreateHole(BulletType.NormalHole, appearanceTime: 1.0f, interval: 1.5f,
						row: (int) Row.Second, column: (int) Column.Left));
					break;
				default:
					throw new NotImplementedException();
			}

			foreach (var coroutine in coroutines) StartCoroutine(coroutine);
		}

		// 指定した行(or列)の端から一定の時間間隔(interval)で弾丸を作成するメソッド
		private IEnumerator CreateBullet(BulletType bulletType, BulletDirection direction, int line,
			float appearanceTime, float interval)
		{
			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTime - warningDisplayedTime - (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			while (true)
			{
				sum++;
				GameObject cartridge;
				GameObject warning;
				switch (bulletType)
				{
					case BulletType.NormalCartridge:
						cartridge = Instantiate(normalCartridgePrefab);
						warning = Instantiate(normalCartridgeWarningPrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				// 変数の初期設定
				var cartridgeScript = cartridge.GetComponent<CartridgeController>();
				cartridgeScript.Initialize(direction, line);

				// emerge a bullet warning
				var warningScript = warning.GetComponent<CartridgeWarningController>();
				warningScript.Initialize(cartridge.transform.position, cartridgeScript.motionVector,
					cartridgeScript.localScale, cartridgeScript.originalWidth, cartridgeScript.originalHeight);

				// delete the bullet warning
				warningScript.DeleteWarning(cartridge);

				// 一定時間(interval)待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime + interval * sum - (currentTime - startTime));
			}
		}

		// 指定したパネルに一定の時間間隔(interval)で撃ち抜く銃弾を作成するメソッド
		private IEnumerator CreateHole(BulletType bulletType, float appearanceTime, float interval, int row = 0,
			int column = 0)
		{
			var currentTime = Time.time;
			yield return new WaitForSeconds(appearanceTime - warningDisplayedTime - (currentTime - startTime));

			var sum = 0;

			while (true)
			{
				sum++;
				GameObject hole;
				GameObject warning;
				switch (bulletType)
				{
					case BulletType.NormalHole:
						hole = Instantiate(normalHolePrefab);
						warning = Instantiate(normalHoleWarningPrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				var holeScript = hole.GetComponent<HoleController>();
				holeScript.Initialize(row, column);

				var warningScript = warning.GetComponent<HoleWarningController>();
				warningScript.Initialize(row, column);

				// delete the bullet warning
				warningScript.DeleteWarning(hole);

				// 一定時間(interval)待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime + interval * sum - (currentTime - startTime));
			}
		}
	}
}

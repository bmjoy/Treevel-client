using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGenerator : MonoBehaviour
	{
		// 生成された銃弾のID(sortingOrder)
		private short bulletId = -32768;

		private List<IEnumerator> coroutines;

		private GamePlayDirector gamePlayDirector;

		// 銃弾および警告のprefab
		public GameObject normalCartridgePrefab;
		public GameObject normalCartridgeWarningPrefab;
		public GameObject turnCartridgePrefab;
		public GameObject normalHolePrefab;
		public GameObject normalHoleWarningPrefab;
		public GameObject aimingHolePrefab;
		public GameObject aimingHoleWarningPrefab;

		// Generatorが作成された時刻
		public float startTime;

		private void OnEnable()
		{
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;
		}

		private void OnDisable()
		{
			GamePlayDirector.OnSucceed -= OnSucceed;
			GamePlayDirector.OnFail -= OnFail;
		}

		public void CreateBullets(List<IEnumerator> coroutines)
		{
			this.coroutines = coroutines;
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			startTime = Time.time;

			foreach (var coroutine in coroutines) StartCoroutine(coroutine);
		}

		public IEnumerator CreateRandomBullet(float appearanceTime, float interval,
			CartridgeDirection direction = CartridgeDirection.Random, int line = (int) Row.Random, Row row = Row.Random,
			Column column = Column.Random, bool loop = true, BulletInfo bulletInfo = null)
		{
			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			if (bulletInfo == null)
			{
				bulletInfo = new BulletInfo();
			}

			do
			{
				sum++;
				var bulletType = bulletInfo.GetBulletType();
				switch (bulletType)
				{
					case BulletType.Cartridge:
						var cartridgeType = bulletInfo.GetCartridgeType();
						StartCoroutine(CreateOneCartridge(cartridgeType, bulletId, direction, line, bulletInfo));
						break;
					case BulletType.Hole:
						var holeType = bulletInfo.GetHoleType();
						StartCoroutine(CreateOneHole(holeType, bulletId, (int) row, (int) column, bulletInfo));
						break;
					default:
						throw new NotImplementedException();
				}

				// 作成する銃弾の個数の上限チェック
				try
				{
					bulletId = checked((short) (bulletId + 1));
				}
				catch (OverflowException)
				{
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}

				// 次の銃弾を作成する時刻まで待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - startTime));
			} while (loop);
		}

		{
		}

		{
		}

		// 指定した行(or列)の端から一定の時間間隔(interval)で弾丸を作成するメソッド
		public IEnumerator CreateCartridge(CartridgeType cartridgeType, float appearanceTime, float interval,
			CartridgeDirection direction, int line, bool loop = true, BulletInfo bulletInfo = null)
		{
			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			if (bulletInfo == null)
			{
				bulletInfo = new BulletInfo();
			}

			do
			{
				sum++;
				if (cartridgeType == CartridgeType.Random)
				{
					StartCoroutine(CreateOneCartridge(bulletInfo.GetCartridgeType(), bulletId, direction, line,
						bulletInfo));
				}
				else
				{
					StartCoroutine(CreateOneCartridge(cartridgeType, bulletId, direction, line, bulletInfo));
				}

				// 作成する銃弾の個数の上限チェック
				try
				{
					bulletId = checked((short) (bulletId + 1));
				}
				catch (OverflowException)
				{
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}

				// 次の銃弾を作成する時刻まで待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - startTime));
			} while (loop);
		}

		// warningの表示が終わる時刻を待ち、cartridgeを作成するメソッド
		private IEnumerator CreateOneCartridge(CartridgeType cartridgeType, short cartridgeId,
			CartridgeDirection direction, int line,
			BulletInfo bulletInfo)
		{
			// 作成するcartidgeの種類で分岐
			GameObject warning;
			switch (cartridgeType)
			{
				case CartridgeType.Normal:
					warning = Instantiate(normalCartridgeWarningPrefab);
					break;
				case CartridgeType.Turn:
					warning = Instantiate(normalCartridgeWarningPrefab);
					break;
				default:
					throw new NotImplementedException();
			}

			// 同レイヤーのオブジェクトの描画順序の制御
			warning.GetComponent<Renderer>().sortingOrder = cartridgeId;

			// warningの位置・大きさ等の設定
			var warningScript = warning.GetComponent<CartridgeWarningController>();
			var bulletMotionVector = warningScript.Initialize(cartridgeType, ref direction, ref line, bulletInfo);

			// 警告の表示時間だけ待つ
			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			// 警告を削除する
			Destroy(warning);

			// ゲームが続いているなら銃弾を作成する
			if (gamePlayDirector.state == GamePlayDirector.GameState.Playing)
			{
				GameObject cartridge;
				switch (cartridgeType)
				{
					case CartridgeType.Normal:
						cartridge = Instantiate(normalCartridgePrefab);
						cartridge.GetComponent<NormalCartridgeController>()
							.Initialize(direction, line, bulletMotionVector);
						break;
					case CartridgeType.Turn:
						cartridge = Instantiate(turnCartridgePrefab);
						cartridge.GetComponent<TurnCartridgeController>()
							.Initialize(direction, line, bulletMotionVector, bulletInfo);
						break;
					default:
						throw new NotImplementedException();
				}

				// 同レイヤーのオブジェクトの描画順序の制御
				cartridge.GetComponent<Renderer>().sortingOrder = cartridgeId;
			}
		}

		// 指定したパネルに一定の時間間隔(interval)で撃ち抜く銃弾を作成するメソッド
		public IEnumerator CreateHole(HoleType holeType, float appearanceTime, float interval, int row = 0,
			int column = 0, bool loop = true, BulletInfo bulletInfo = null)
		{
			var currentTime = Time.time;
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - startTime));

			var sum = 0;

			do
			{
				sum++;
				StartCoroutine(CreateOneHole(holeType, bulletId, row, column, bulletInfo));

				try
				{
					bulletId = checked((short) (bulletId + 1));
				}
				catch (OverflowException)
				{
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}

				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - startTime));
			} while (loop);
		}

		// warningの表示が終わる時刻を待ち、holeを作成するメソッド
		private IEnumerator CreateOneHole(HoleType holeType, short holeId, int row, int column,
			BulletInfo bulletInfo)
		{
			GameObject warning;
			switch (holeType)
			{
				case HoleType.Normal:
					warning = Instantiate(normalHoleWarningPrefab);
					warning.GetComponent<NormalHoleWarningController>().Initialize(row, column);
					break;
				case HoleType.Aiming:
					warning = Instantiate(aimingHoleWarningPrefab);
					warning.GetComponent<AimingHoleWarningController>().Initialize(bulletInfo);
					break;
				default:
					throw new NotImplementedException();
			}

			warning.GetComponent<Renderer>().sortingOrder = holeId;

			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			Destroy(warning);

			if (gamePlayDirector.state == GamePlayDirector.GameState.Playing)
			{
				GameObject hole;
				NormalHoleController holeScript;
				switch (holeType)
				{
					case HoleType.Normal:
						hole = Instantiate(normalHolePrefab);
						holeScript = hole.GetComponent<NormalHoleController>();
						holeScript.Initialize(row, column, warning.transform.position);
						break;
					case HoleType.Aiming:
						hole = Instantiate(aimingHolePrefab);
						holeScript = hole.GetComponent<AimingHoleController>();
						holeScript.Initialize(row, column, warning.transform.position);
						break;
					default:
						throw new NotImplementedException();
				}

				hole.GetComponent<Renderer>().sortingOrder = holeId;
				StartCoroutine(holeScript.Delete());
			}
		}

		private void OnSucceed()
		{
			GameFinish();
		}

		private void OnFail()
		{
			GameFinish();
		}

		private void GameFinish()
		{
			foreach (var coroutine in coroutines) StopCoroutine(coroutine);
		}
	}
}

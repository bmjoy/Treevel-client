using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGroupGenerator : MonoBehaviour
	{
		// 生成された銃弾のID(sortingOrder)
		public short bulletId = -32768;

		// 銃弾グループを制御するcoroutine
		private List<IEnumerator> coroutines;

		private GamePlayDirector gamePlayDirector;

		// 銃弾グループのprefab
		public GameObject bulletGroupControllerPrefab;

		// 各銃弾のGeneratorのprefab
		public GameObject normalCartridgeGeneratorPrefab;
		public GameObject turnCartridgeGeneratorPrefab;
		public GameObject normalHoleGeneratorPrefab;
		public GameObject aimingHoleGeneratorPrefab;

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

		/* 一連の銃弾の生成タイミングを管理するBulletGroupを作成する */
		public void CreateBulletGroups(List<IEnumerator> coroutines)
		{
			this.coroutines = coroutines;
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			startTime = Time.time;

			foreach (var coroutine in this.coroutines) StartCoroutine(coroutine);
		}

		/* BulletGroupを生成する */
		public IEnumerator CreateBulletGroup(float appearanceTime, float interval, bool loop,
			List<GameObject> bulletGenerators)
		{
			var bulletGroup = Instantiate(bulletGroupControllerPrefab);
			var bulletGroupScript = bulletGroup.GetComponent<BulletGroupController>();
			bulletGroupScript.Initialize(startTime: startTime, appearanceTime: appearanceTime, interval: interval,
				loop: loop, bulletGenerators: bulletGenerators);
			yield return StartCoroutine(bulletGroupScript.CreateBullets());
		}

		private void OnSucceed()
		{
			GameFinish();
		}

		private void OnFail()
		{
			GameFinish();
		}

		/* ゲーム終了時に全てのBulletGroupを停止させる */
		private void GameFinish()
		{
			StopAllCoroutines();
		}

		/* NormalCartridgeのGeneratorを生成する */
		public GameObject CreateNormalCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Row row)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
			return cartridgeGenerator;
		}

		public GameObject CreateNormalCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection,
			Column column)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
			return cartridgeGenerator;
		}

		public GameObject CreateNormalCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Row row,
			int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, randomCartridgeDirection, randomRow,
				randomColumn);
			return cartridgeGenerator;
		}

		public GameObject CreateNormalCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection,
			Column column, int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, randomCartridgeDirection, randomRow,
				randomColumn);
			return cartridgeGenerator;
		}

		/* TurnCartridgeのGeneratorを生成する*/
		public GameObject CreateTurnCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Row row,
			int[] turnDirection = null, int[] turnLine = null)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
			return cartridgeGenerator;
		}

		public GameObject CreateTurnCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Column column,
			int[] turnDirection = null, int[] turnLine = null)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
			return cartridgeGenerator;
		}

		public GameObject CreateTurnCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Row row,
			int[] turnDirection, int[] turnLine, int[] randomTurnDirections, int[] randomTurnRow,
			int[] randomTurnColumn)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, randomTurnDirections, randomTurnRow,
				randomTurnColumn);
			return cartridgeGenerator;
		}

		public GameObject CreateTurnCartridgeGenerator(int ratio, CartridgeDirection cartridgeDirection, Column column,
			int[] turnDirection, int[] turnLine, int[] randomTurnDirections, int[] randomTurnRow,
			int[] randomTurnColumn)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, randomTurnDirections, randomTurnRow,
				randomTurnColumn);
			return cartridgeGenerator;
		}

		/* NormalHoleのGeneratorを生成する */
		public GameObject CreateNormalHoleGenerator(int ratio, Row row, Column column)
		{
			var holeGenerator = Instantiate(normalHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, row, column);
			return holeGenerator;
		}

		public GameObject CreateNormalHoleGenerator(int ratio, Row row, Column column, int[] randomRow,
			int[] randomColumn)
		{
			var holeGenerator = Instantiate(normalHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, row, column, randomRow, randomColumn);
			return holeGenerator;
		}

		/* AimingHoleのGeneratorを生成する */
		public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel = null)
		{
			var holeGenerator = Instantiate(aimingHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, aimingPanel);
			return holeGenerator;
		}

		public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel, int[] randomNumberPanel)
		{
			var holeGenerator = Instantiate(aimingHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, aimingPanel, randomNumberPanel);
			return holeGenerator;
		}
	}
}

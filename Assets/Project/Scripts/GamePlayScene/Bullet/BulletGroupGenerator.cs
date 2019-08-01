using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGroupGenerator : SingletonObject<BulletGroupGenerator>
	{
		// 生成された銃弾のID(sortingOrder)
		public short bulletId;

		// 銃弾グループを制御するcoroutine
		private List<IEnumerator> coroutines;

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
			Initialize();

			this.coroutines = coroutines;
			foreach (var coroutine in this.coroutines) StartCoroutine(coroutine);
		}

		/* ゲーム開始時およびリトライ時に初期化が必要な変数を初期化する */
		private void Initialize()
		{
			bulletId = short.MinValue;
			startTime = Time.time;
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
		/* 横方向、特定の行を移動する銃弾を生成 */
		public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
			return cartridgeGenerator;
		}

		/* 縦方向、特定の列を移動する銃弾を生成 */
		public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
			Column column)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
			return cartridgeGenerator;
		}

		/* 横方向、ランダムな行を移動する銃弾を生成
		   進行方向もランダムな時はこのメソッドを使用する */
		public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
			int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, randomCartridgeDirection, randomRow,
				randomColumn);
			return cartridgeGenerator;
		}

		/* 縦方向、ランダムな列を移動する銃弾を生成 */
		public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
			Column column, int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			var cartridgeGenerator = Instantiate(normalCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, randomCartridgeDirection, randomRow,
				randomColumn);
			return cartridgeGenerator;
		}

		/* TurnCartridgeのGeneratorを生成する*/
		/* 横方向、特定の行を移動し、特定の場所で特定の方向に曲がる銃弾を生成 */
		public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
			int[] turnDirection = null, int[] turnLine = null)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
			return cartridgeGenerator;
		}

		/* 縦方向、特定の行を移動し、特定の場所で特定の方向に曲がる銃弾を生成 */
		public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, Column column,
			int[] turnDirection = null, int[] turnLine = null)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
			return cartridgeGenerator;
		}

		/* 横方向、ランダムな行を移動し、ランダムな場所でランダムな方向に曲がる銃弾を生成
		   進行方向もランダムな時はこのメソッドを使用する */
		public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
			int[] turnDirection, int[] turnLine, int[] randomCartridgedirection, int[] randomRow, int[] randomColumn,
			int[] randomTurnDirections, int[] randomTurnRow,
			int[] randomTurnColumn)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, turnDirection, turnLine,
				randomCartridgedirection, randomRow, randomColumn, randomTurnDirections, randomTurnRow,
				randomTurnColumn);
			return cartridgeGenerator;
		}

		/* 縦方向、ランダムな行を移動し、ランダムな場所でランダムな方向に曲がる銃弾を生成 */
		public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, Column column,
			int[] turnDirection, int[] turnLine, int[] randomCartridgedirection, int[] randomRow, int[] randomColumn,
			int[] randomTurnDirections, int[] randomTurnRow,
			int[] randomTurnColumn)
		{
			var cartridgeGenerator = Instantiate(turnCartridgeGeneratorPrefab);
			var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
			cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, turnDirection, turnLine,
				randomCartridgedirection, randomRow, randomColumn, randomTurnDirections, randomTurnRow,
				randomTurnColumn);
			return cartridgeGenerator;
		}

		/* NormalHoleのGeneratorを生成する */
		/* 特定の行、特定の列に銃弾を生成する */
		public GameObject CreateNormalHoleGenerator(int ratio, ERow row, Column column)
		{
			var holeGenerator = Instantiate(normalHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, row, column);
			return holeGenerator;
		}

		/* ランダムな行、ランダムな列に銃弾を生成する */
		public GameObject CreateNormalHoleGenerator(int ratio, ERow row, Column column, int[] randomRow,
			int[] randomColumn)
		{
			var holeGenerator = Instantiate(normalHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, row, column, randomRow, randomColumn);
			return holeGenerator;
		}

		/* AimingHoleのGeneratorを生成する */
		/* 特定の番号のパネルのあるタイルの場所に銃弾を生成する */
		public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel = null)
		{
			var holeGenerator = Instantiate(aimingHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, aimingPanel);
			return holeGenerator;
		}

		/* ランダムな番号のパネルのあるタイルの場所に銃弾を生成する*/
		public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel, int[] randomNumberPanel)
		{
			var holeGenerator = Instantiate(aimingHoleGeneratorPrefab);
			var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
			holeGeneratorScript.Initialize(ratio, aimingPanel, randomNumberPanel);
			return holeGenerator;
		}
	}
}

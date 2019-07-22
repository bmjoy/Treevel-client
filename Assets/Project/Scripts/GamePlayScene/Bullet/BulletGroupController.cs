using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGroupController : MonoBehaviour
	{
		private GamePlayDirector gamePlayDirector;

		private BulletGroupGenerator bulletGroupGenerator;

		// 銃弾の生成初めの時刻
		private float appearanceTime;

		// 銃弾の生成間隔
		private float interval;

		// 銃弾生成のループの有無
		private bool loop;

		// 乱数
		private System.Random random;

		// 銃弾グループ内で管理する各銃弾のGenerator
		private List<GameObject> bulletGenerators;

		// 各銃弾のGeneratorの出現割合
		private int[] bulletRatio;

		private void Awake()
		{
			bulletGroupGenerator = BulletGroupGenerator.Instance;
		}

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

		/* 銃弾グループに必要な引数を受け取る */
		public void Initialize(float startTime, float appearanceTime, float interval, bool loop,
			List<GameObject> bulletGenerators)
		{
			random = new System.Random();
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			this.appearanceTime = appearanceTime;
			this.interval = interval;
			this.loop = loop;
			this.bulletGenerators = bulletGenerators;
			bulletRatio = new int[bulletGenerators.Count];
			for (var index = 0; index < bulletGenerators.Count; index++)
			{
				bulletRatio[index] = bulletGenerators[index].GetComponent<BulletGenerator>().ratio;
			}
		}

		/* 銃弾生成時刻と、生成する銃弾を管理する */
		public IEnumerator CreateBullets()
		{
			var currentTime = Time.time;
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - bulletGroupGenerator.startTime));
			var sum = 0;

			do
			{
				sum++;
				// 出現させる銃弾を決定する
				var index = BulletGenerator.GetRandomParameter(bulletRatio);
				var bulletGeneratorScript = bulletGenerators[index].GetComponent<BulletGenerator>();
				// 銃弾のsortingOrderを管理するIDを決定する
				var nextBulletId = bulletGroupGenerator.bulletId;
				StartCoroutine(bulletGeneratorScript.CreateBullet(nextBulletId));

				// 作成する銃弾の個数の上限チェック
				try
				{
					bulletGroupGenerator.bulletId = checked((short) (nextBulletId + 1));
				}
				catch (OverflowException)
				{
					bulletGroupGenerator.bulletId = short.MinValue;
				}

				// 次の銃弾を作成する時刻まで待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - bulletGroupGenerator.startTime));
			} while (loop);
		}

		private void OnFail()
		{
			GameFinish();
		}

		private void OnSucceed()
		{
			GameFinish();
		}

		/* ゲーム終了時に銃弾グループを削除する */
		private void GameFinish()
		{
			Destroy(gameObject);
		}
	}
}

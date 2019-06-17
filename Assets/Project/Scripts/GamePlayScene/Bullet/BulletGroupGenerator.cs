using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGroupGenerator : MonoBehaviour
	{
		// 生成された銃弾のID(sortingOrder)
		private short bulletId = -32768;

		// 銃弾グループを制御するcoroutine
		private List<IEnumerator> coroutines;

		private GamePlayDirector gamePlayDirector;

		// 銃弾グループのprefab
		public GameObject bulletGroupControllerPrefab;

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

		{
		}

		{
		}

		{
		}

		{
		}

		{
		}

		{

		}

		{




		}

		{
		}

		{
		}

		{
		}
	}
}

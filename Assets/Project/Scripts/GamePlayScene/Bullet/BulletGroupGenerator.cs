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

		private List<IEnumerator> coroutines;

		private GamePlayDirector gamePlayDirector;


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

		{
			this.coroutines = coroutines;
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			startTime = Time.time;

			foreach (var coroutine in coroutines) StartCoroutine(coroutine);
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

		{
		}
	}
}

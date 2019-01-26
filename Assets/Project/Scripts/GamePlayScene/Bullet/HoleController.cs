﻿using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public abstract class HoleController : BulletController
	{
		private Boolean gameFail = false;

		private void Start()
		{
			StartCoroutine(Delete());
		}

		private IEnumerator Delete()
		{
			yield return new WaitForSeconds(0.1f);
			if (!gameFail)
			{
				Destroy(gameObject);
			}
		}

		private void Awake()
		{
			// BocCollider2Dのアタッチメント
			gameObject.AddComponent<BoxCollider2D>();
			gameObject.GetComponent<BoxCollider2D>().size = new Vector2(PanelSize.WIDTH / 2,PanelSize.HEIGHT / 2);
			gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
			// RigidBodyのアタッチメント
			gameObject.AddComponent<Rigidbody2D>();
			gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;

			gameObject.SetActive(false);
		}

		private void OnFail()
		{
			gameFail = true;
		}

		private void OnSucceed()
		{
			Destroy(gameObject);
		}


		// コンストラクタがわりのメソッド
		public virtual void Initialize(int row, int column)
		{
			gameObject.GetComponent<Renderer>().sortingLayerName = "Bullet";
		}
	}
}

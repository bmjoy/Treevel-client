﻿using System;
using System.Collections;
using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public abstract class CartridgeController : BulletController
	{
		public float additionalMargin = 0.00001f;
		public float localScale;
		public Vector2 motionVector;
		public float speed;

		// 元画像のサイズ
		public float originalWidth;
		public float originalHeight;


		protected void Update()
		{
			// Check if bullet goes out of window
			if (transform.position.x < -((WindowSize.WIDTH + localScale * originalWidth) / 2 + additionalMargin) ||
			    transform.position.x > (WindowSize.WIDTH + localScale * originalWidth) / 2 + additionalMargin ||
			    transform.position.y < -((WindowSize.HEIGHT + localScale * originalHeight) / 2 + additionalMargin) ||
			    transform.position.y > (WindowSize.HEIGHT + localScale * originalHeight) / 2 + additionalMargin)
				Destroy(gameObject);
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

		protected void FixedUpdate()
		{
			transform.Translate(motionVector * speed, Space.World);
		}

		// 銃弾の初期配置の設定
		protected void SetInitialPosition(BulletGenerator.BulletDirection direction, int line)
		{
			switch (direction)
			{
				case BulletGenerator.BulletDirection.ToLeft:
					transform.position = new Vector2((WindowSize.WIDTH + localScale * originalWidth) / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					motionVector = Vector2.left;
					break;
				case BulletGenerator.BulletDirection.ToRight:
					transform.position = new Vector2(-(WindowSize.WIDTH + localScale * originalWidth) / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					motionVector = Vector2.right;
					break;
				case BulletGenerator.BulletDirection.ToUp:
					transform.position = new Vector2(TileSize.WIDTH * (line - 2),
						-(WindowSize.HEIGHT + localScale * originalHeight) / 2);
					motionVector = Vector2.up;
					break;
				case BulletGenerator.BulletDirection.ToBottom:
					transform.position = new Vector2(TileSize.WIDTH * (line - 2),
						(WindowSize.HEIGHT + localScale * originalHeight) / 2);
					motionVector = Vector2.down;
					break;
			}

			// Check if a bullet should be flipped vertically
			transform.localScale *= new Vector2(localScale, -1 * Mathf.Sign(motionVector.x) * localScale);

			// Calculate rotation angle
			var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
			angle = (float) (Mathf.Acos(angle) * 180 / Math.PI);
			angle *= -1 * Mathf.Sign(motionVector.y);

			transform.Rotate(new Vector3(0, 0, angle), Space.World);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// パネルとの衝突以外は考えない
			if (!other.gameObject.CompareTag("Panel")) return;
			// 衝突したオブジェクトは赤色に変える
			gameObject.GetComponent<SpriteRenderer>().color = Color.red;
		}

		private void OnFail()
		{
			speed = 0;
		}

		private void OnSucceed()
		{
			Destroy(gameObject);
		}
	}
}

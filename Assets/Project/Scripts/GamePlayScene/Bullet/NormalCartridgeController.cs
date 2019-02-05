using System;
using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeController : CartridgeController
	{
		// コンストラクタがわりのメソッド
		public override void Initialize(BulletGenerator.CartridgeDirection direction, int line)
		{
			base.Initialize(direction, line);
			speed = 0.1f;
			localScale = (float) (WindowSize.WIDTH * 0.15);

			SetInitialPosition(direction, line);
		}
	}
}

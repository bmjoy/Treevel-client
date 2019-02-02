using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalHoleController : HoleController
	{
		public override void Initialize(int row, int column)
		{
			base.Initialize(row, column);
			localScale = (float) (WindowSize.WIDTH * 0.15);
			originalWidth = GetComponent<Renderer>().bounds.size.x;
			originalHeight = GetComponent<Renderer>().bounds.size.y;
			transform.localScale *= new Vector2(localScale, localScale);
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			transform.position =
				new Vector2(TileSize.WIDTH * (column - 2), topTilePositionY - TileSize.HEIGHT * (row - 1));
		}
	}
}

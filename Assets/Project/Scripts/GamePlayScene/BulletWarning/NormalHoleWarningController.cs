using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class NormalHoleWarningController : HoleWarningController
	{
		public override void Initialize(int row, int column)
		{
			base.Initialize(row, column);
			originalWidth = GetComponent<Renderer>().bounds.size.x;
			originalHeight = GetComponent<Renderer>().bounds.size.y;
			transform.localScale *= new Vector2(TileSize.WIDTH / 2, TileSize.HEIGHT / 2);
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			transform.position =
				new Vector2(TileSize.WIDTH * (column - 2), topTilePositionY - TileSize.HEIGHT * (row - 1));
		}
	}
}

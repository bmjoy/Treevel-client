using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalHoleController : HoleController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) *
			                       LOCAL_SCALE;
		}

		public override void Initialize(int row, int column)
		{
			base.Initialize(row, column);
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			transform.position =
				new Vector2(TileSize.WIDTH * (column - 2), topTilePositionY - TileSize.HEIGHT * (row - 1));
		}
	}
}

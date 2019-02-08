using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class NormalHoleWarningController : HoleWarningController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale =
				new Vector2(HoleWarningSize.WIDTH / originalWidth, HoleWarningSize.HEIGHT / originalHeight);
		}

		public override void Initialize(int row, int column)
		{
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			transform.position =
				new Vector2(TileSize.WIDTH * (column - 2), topTilePositionY - TileSize.HEIGHT * (row - 1));
		}
	}
}

using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
	public class WarpTileController : TileController
	{
		// 相方のwarpTile
		public GameObject pairTile;

		public void Initialize(Vector2 position, int tileNum, GameObject pairTile)
		{
			base.Initialize(position, tileNum);
			this.pairTile = pairTile;
		}
	}
}

using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
	public class WarpTileController : NormalTileController
	{
		// 相方のwarpTile
		public GameObject pairTile;

		public void Initialize(Vector2 position, int tileNum, GameObject pairTile)
		{
			base.Initialize(position, tileNum);
			name = TileName.WARP_TILE + tileNum;
			this.pairTile = pairTile;
		}

		public override void HandlePanel(GameObject panel)
		{
			if (pairTile.transform.childCount == 0)
			{
				panel.transform.parent = pairTile.transform;
				panel.transform.position = pairTile.transform.position;
			}
		}
	}
}

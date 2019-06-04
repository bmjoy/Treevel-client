using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
	public static class TileLibrary
	{
		/* タイルの番号を受け取り，タイルオブジェクトを返す */
		public static GameObject GetTile(int tileNum)
		{
			GameObject[] tiles = GameObject.FindGameObjectsWithTag(TagName.TILE);

			foreach (var tile in tiles)
			{
				var script = tile.GetComponent<NormalTileController>();

				if (script.GetTile(tileNum) != null)
				{
					return tile;
				}
			}

			return null;
		}

		/* タイルの行・列を受け取り，タイルオブジェクトを返す */
		public static GameObject GetTile(int row, int column)
		{
			return GetTile((row - 1) * StageSize.COLUMN + column);
		}
	}
}

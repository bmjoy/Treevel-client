using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
	public class TileGenerator : MonoBehaviour
	{
		public GameObject normalTilePrefab;
		public GameObject warpTilePrefab;

		private readonly GameObject[,] tiles = new GameObject[5, 3];

		/* 普通タイルの作成 */
		public void CreateNormalTiles()
		{
			for (var tileNum = 1; tileNum <= 15; tileNum++)
			{
				// 行 (0~4)
				var row = (tileNum - 1) / 3;
				// 列 (0~2)
				var column = (tileNum - 1) % 3;

				// 既に他タイルを作成している場合はスルー
				if (tiles[row, column] != null) continue;

				var tile = Instantiate(normalTilePrefab);

				var tilePosition = GetTilePosition(tileNum);

				tile.GetComponent<NormalTileController>().Initialize(tilePosition, tileNum);

				SetTile(tileNum, tile);
			}

			// タイルの位置関係を作成する
			MakeRelations(tiles);
		}

		/* ワープタイルの作成 */
		public void CreateWarpTiles(int firstTileNum, int secondTileNum)
		{
			var firstTile = Instantiate(warpTilePrefab);
			var secondTile = Instantiate(warpTilePrefab);

			var firstTilePosition = GetTilePosition(firstTileNum);
			var secondTilePosition = GetTilePosition(secondTileNum);

			firstTile.GetComponent<WarpTileController>().Initialize(firstTilePosition, firstTileNum, secondTile);
			secondTile.GetComponent<WarpTileController>().Initialize(secondTilePosition, secondTileNum, firstTile);

			SetTile(firstTileNum, firstTile);
			SetTile(secondTileNum, secondTile);
		}

		/* タイルの座標を取得 */
		private static Vector2 GetTilePosition(int tileNum)
		{
			// 最上タイルのy座標
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);

			// 処理計算では0から扱いたい
			tileNum -= 1;
			// 行 (0~4)
			var row = tileNum / 3;
			// 列 (0~2)
			var column = tileNum % 3;
			// 作成するタイルのx,y座標
			var positionX = TileSize.WIDTH * (column - 1);
			var positionY = topTilePositionY - TileSize.HEIGHT * row;

			return new Vector2(positionX, positionY);
		}

		/* タイルを配列に格納 */
		private void SetTile(int tileNum, GameObject tile)
		{
			// 行 (0~4)
			var row = (tileNum - 1) / 3;
			// 列 (0~2)
			var column = (tileNum - 1) % 3;

			tiles[row, column] = tile;
		}

		private static void MakeRelations(GameObject[,] tiles)
		{
			var row = tiles.GetLength(0);
			var column = tiles.GetLength(1);

			for (var i = 0; i < row; i++)
			{
				for (var j = 0; j < column; j++)
				{
					var rightTile = j + 1 == column ? null : tiles[i, j + 1];
					var leftTile = j == 0 ? null : tiles[i, j - 1];
					var upperTile = i == 0 ? null : tiles[i - 1, j];
					var lowerTile = i + 1 == row ? null : tiles[i + 1, j];
					// タイルオブジェクトのスクリプトに上下左右のタイルオブジェクトを格納する
					tiles[i, j].GetComponent<TileController>().MakeRelation(rightTile, leftTile, upperTile, lowerTile);
				}
			}
		}
	}
}

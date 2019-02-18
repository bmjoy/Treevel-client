﻿using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
	public class TileGenerator : MonoBehaviour
	{
		public GameObject normalTilePrefab;

		// 現段階では5行3列のタイル群
		// 現段階ではステージ番号に依存しない
		public void CreateTiles(int stageId)
		{
			// 最上タイルのy座標
			const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
			// タイルの識別番号（分かりやすさのため1から振り分け）
			var tileNum = 1;
			// タイルを格納する配列
			GameObject[,] tiles = new GameObject[5, 3];

			// タイルを作成し、配置する
			for (var row = 0; row <= 4; row++)
			{
				for (var column = -1; column <= 1; column++)
				{
					// 作成するタイルのx,y座標
					var positionX = TileSize.WIDTH * column;
					var positionY = topTilePositionY - TileSize.HEIGHT * row;
					// 15枚のタイルに0-14の名前を付ける
					tiles[row, column + 1] = CreateOneTile(new Vector2(positionX, positionY), tileNum);
					tileNum++;
				}
			}

			// タイルの位置関係を作成する
			MakeRelations(tiles);
		}

		private GameObject CreateOneTile(Vector2 position, int tileNum)
		{
			var tile = Instantiate(normalTilePrefab);
			var tileWidth = tile.GetComponent<SpriteRenderer>().size.x;
			var tileHeight = tile.GetComponent<SpriteRenderer>().size.y;
			tile.transform.localScale = new Vector2(TileSize.WIDTH / tileWidth, TileSize.HEIGHT / tileHeight);
			tile.transform.position = position;
			tile.name = "Tile" + tileNum.ToString();
			tile.GetComponent<Renderer>().sortingLayerName = "Tile";

			return tile;
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
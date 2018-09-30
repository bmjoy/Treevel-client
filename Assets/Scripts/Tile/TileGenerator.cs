using UnityEngine;

namespace Tile
{
    public class TileGenerator : MonoBehaviour
    {
        public GameObject normalTilePrefab;

        // 現段階では5行3列のタイル群
        public void CreateTiles()
        {
            // 最上タイルのy座標
            var topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
            // タイルの識別番号
            var tileNum = 0;
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
            GameObject tile = Instantiate(normalTilePrefab) as GameObject;
            tile.transform.localScale = new Vector2(TileSize.WIDTH * 0.5f, TileSize.HEIGHT * 0.5f);
            tile.transform.position = position;
            tile.name = "Tile" + tileNum.ToString();
            tile.GetComponent<Renderer>().sortingLayerName = "Tile";

            return tile;
        }

        private void MakeRelations(GameObject[,] tiles)
        {
            var row = tiles.GetLength(0);
            var column = tiles.GetLength(1);

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    GameObject rightTile = j + 1 == column ? null : tiles[i, j + 1];
                    GameObject leftTile = j == 0 ? null : tiles[i, j - 1];
                    GameObject upperTile = i == 0 ? null : tiles[i - 1, j];
                    GameObject lowerTile = i + 1 == row ? null : tiles[i + 1, j];
                    // タイルオブジェクトのスクリプトに上下左右のタイルオブジェクトを格納する
                    tiles[i, j].GetComponent<TileController>().MakeRelation(rightTile, leftTile, upperTile, lowerTile);
                }
            }
        }
    }
}

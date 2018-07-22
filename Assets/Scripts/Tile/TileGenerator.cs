using UnityEngine;

namespace Tile
{
    public class TileGenerator : MonoBehaviour
    {
        public GameObject normalTilePrefab;

        // Use this for initialization
        private void Start()
        {
            CreateTiles();
        }

        // 現段階では5行3列のタイル群
        private void CreateTiles()
        {
            // 最上タイルのy座標
            var topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
            // タイルを作成し、配置する
            for (var row = 0; row <= 4; row++)
            {
                for (var column = -1; column <= 1; column++)
                {
                    // 作成するタイルのx,y座標
                    var positionX = TileSize.WIDTH * column;
                    var positionY = topTilePositionY - TileSize.HEIGHT * row;
                    CreateOneTile(new Vector2(positionX, positionY));
                }
            }
        }

        private void CreateOneTile(Vector2 position)
        {
            GameObject tile = Instantiate(normalTilePrefab) as GameObject;
            tile.transform.localScale = new Vector2(TileSize.WIDTH * 0.5f, TileSize.HEIGHT * 0.5f);
            tile.transform.position = position;
        }
    }
}

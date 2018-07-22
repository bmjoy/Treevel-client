using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class TileGenerator : MonoBehaviour
{
    public GameObject normalTilePrefab;

    // Use this for initialization
    void Start()
    {
        createTiles();
    }

    // 現段階では5行3列のタイル群
    void createTiles()
    {
        // 最上タイルのy座標
        float top_tile_position_y = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
        // タイルを作成し、配置する
        for (int row = 0; row <= 4; row++)
        {
            for (int column = -1; column <= 1; column++)
            {
                // 作成するタイルのx,y座標
                float position_x = TileSize.WIDTH * column;
                float position_y = top_tile_position_y - TileSize.HEIGHT * row;
                createOneTile(new Vector2(position_x, position_y));
            }
        }
    }

    void createOneTile(Vector2 position)
    {
        GameObject tile = Instantiate(normalTilePrefab) as GameObject;
        tile.transform.localScale = new Vector2(TileSize.WIDTH * 0.5f, TileSize.HEIGHT * 0.5f);
        tile.transform.position = position;
    }
}

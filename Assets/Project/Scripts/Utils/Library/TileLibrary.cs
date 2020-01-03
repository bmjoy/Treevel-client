using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class TileLibrary
    {
        /* タイルの番号を受け取り，タイルオブジェクトを返す */
        /// <summary>
        /// タイルの番号から，タイルを返す
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <returns> タイルオブジェクト </returns>
        public static GameObject GetTile(int tileNum)
        {
            var tiles = GameObject.FindGameObjectsWithTag(TagName.TILE);

            foreach (var tile in tiles) {
                var script = tile.GetComponent<NormalTileController>();

                if (script.GetTile(tileNum) != null) {
                    return tile;
                }
            }

            return null;
        }

        /// <summary>
        /// タイルの行・列から，タイルを返す
        /// </summary>
        /// <param name="row"> 行 </param>
        /// <param name="column"> 列 </param>
        /// <returns> タイルオブジェクト </returns>
        public static GameObject GetTile(int row, int column)
        {
            return GetTile((row - 1) * StageSize.COLUMN + column);
        }
    }
}

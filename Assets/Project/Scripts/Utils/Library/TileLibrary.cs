using System.Linq;
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
            // タイルの番号がtileNumの唯一のタイルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return tiles.Single(tile => tile.GetComponent<NormalTileController>()?.GetTile(tileNum) != null);
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

using System;
using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObject<TileGenerator>
    {
        public void CreateTiles(ICollection<TileData> tileDatas)
        {
            // シーンに配置したノーマルタイルを初期化

            for (var tileNum = 1 ; tileNum <= StageSize.ROW * StageSize.COLUMN; ++tileNum) {
                var currTile = transform.Find($"NormalTile{tileNum}")?.gameObject?.GetComponent<NormalTileController>();

                // initialize tile
                BoardManager.SetTile(currTile, tileNum);
                currTile.GetComponent<SpriteRenderer>().enabled = true;

                // show sprite
                currTile.GetComponent<SpriteRenderer>().enabled = true;
            }

            foreach (var tileData in tileDatas) {
                switch (tileData.type) {
                    case ETileType.Normal:
                        break;
                    case ETileType.Warp:
                        CreateWarpTiles(tileData.number, tileData.pairNumber);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// ワープタイルの作成 (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        private async void CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;
            var secondTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;

            firstTile.GetComponent<WarpTileController>().Initialize(firstTileNum, secondTile);
            secondTile.GetComponent<WarpTileController>().Initialize(secondTileNum, firstTile);

            BoardManager.SetTile(firstTile.GetComponent<NormalTileController>(), firstTileNum);
            BoardManager.SetTile(secondTile.GetComponent<NormalTileController>(), secondTileNum);

            firstTile.GetComponent<SpriteRenderer>().enabled = true;
            secondTile.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

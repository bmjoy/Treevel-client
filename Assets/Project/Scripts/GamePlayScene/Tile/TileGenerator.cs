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
            for (var tileNum = 1; tileNum <= StageSize.ROW * StageSize.COLUMN; ++tileNum) {
                var currTileObj = transform.Find($"NormalTile{tileNum}");
                if (currTileObj == null)
                    continue;

                var currTile = currTileObj.GetComponent<NormalTileController>();
                if (currTile == null)
                    continue;

                // initialize tile
                BoardManager.SetTile(currTile, tileNum);
                currTile.Initialize(tileNum);

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
                    case ETileType.Holy:
                        CreateHolyTile(tileData.number);
                        break;
                    case ETileType.Spiderweb:
                        CreateSpiderwebTile(tileData.number);
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
        private static async void CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;
            var secondTile = await AddressableAssetManager.Instantiate(Address.WARP_TILE_PREFAB).Task;

            firstTile.GetComponent<WarpTileController>().Initialize(firstTileNum, secondTile);
            secondTile.GetComponent<WarpTileController>().Initialize(secondTileNum, firstTile);

            BoardManager.SetTile(firstTile.GetComponent<AbstractTileController>(), firstTileNum);
            BoardManager.SetTile(secondTile.GetComponent<AbstractTileController>(), secondTileNum);

            firstTile.GetComponent<SpriteRenderer>().enabled = true;
            secondTile.GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// 聖域タイルの作成
        /// </summary>
        private static async void CreateHolyTile(int tileNum)
        {
            var holyTile = await AddressableAssetManager.Instantiate(Address.HOLY_TILE_PREFAB).Task;

            holyTile.GetComponent<HolyTileController>().Initialize(tileNum);

            BoardManager.SetTile(holyTile.GetComponent<AbstractTileController>(), tileNum);

            holyTile.GetComponent<SpriteRenderer>().enabled = true;
        }

        private static async void CreateSpiderwebTile(int tileNum)
        {
            var spiderwebTile = await AddressableAssetManager.Instantiate(Address.SPIDERWEB_TILE_PREFAB).Task;

            spiderwebTile.GetComponent<SpiderwebTileController>().Initialize(tileNum);

            BoardManager.SetTile(spiderwebTile.GetComponent<AbstractTileController>(), tileNum);

            spiderwebTile.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

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
        private readonly Dictionary<ETileType, string> _prefabAddressableKeys = new Dictionary<ETileType, string>()
        {
            {ETileType.Normal, Address.NORMAL_TILE_PREFAB},
            {ETileType.Warp, Address.WARP_TILE_PREFAB},
            {ETileType.Ice, Address.ICE_TILE_PREFAB},
            {ETileType.Holy, Address.HOLY_TILE_PREFAB},
            {ETileType.Spiderweb, Address.SPIDERWEB_TILE_PREFAB},
        };

        public async void CreateTiles(ICollection<TileData> tileDatas)
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
                BoardManager.Instance.SetTile(currTile, tileNum);
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
                    case ETileType.Spiderweb:
                    case ETileType.Ice:
                        var tileObj = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[tileData.type]).Task;
                        tileObj.GetComponent<AbstractTileController>().Initialize(tileData.number);
                        BoardManager.Instance.SetTile(tileObj.GetComponent<AbstractTileController>(), tileData.number);
                        tileObj.GetComponent<SpriteRenderer>().enabled = true;
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

            BoardManager.Instance.SetTile(firstTile.GetComponent<AbstractTileController>(), firstTileNum);
            BoardManager.Instance.SetTile(secondTile.GetComponent<AbstractTileController>(), secondTileNum);

            firstTile.GetComponent<SpriteRenderer>().enabled = true;
            secondTile.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

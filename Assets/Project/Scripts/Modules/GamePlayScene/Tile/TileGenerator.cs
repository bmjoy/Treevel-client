using System;
using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObject<TileGenerator>
    {
        private readonly Dictionary<ETileType, string> _prefabAddressableKeys = new Dictionary<ETileType, string>() {
            { ETileType.Normal, Constants.Address.NORMAL_TILE_PREFAB },
            { ETileType.Warp, Constants.Address.WARP_TILE_PREFAB },
            { ETileType.Ice, Constants.Address.ICE_TILE_PREFAB },
            { ETileType.Holy, Constants.Address.HOLY_TILE_PREFAB },
            { ETileType.Spiderweb, Constants.Address.SPIDERWEB_TILE_PREFAB },
        };

        public UniTask CreateTiles(ICollection<TileData> tileDatas)
        {
            // シーンに配置したノーマルタイルを初期化
            for (var tileNum = 1; tileNum <= Constants.StageSize.ROW * Constants.StageSize.COLUMN; ++tileNum) {
                var currTileObj = transform.Find($"NormalTile{tileNum}");
                if (currTileObj == null) continue;

                var currTile = currTileObj.GetComponent<NormalTileController>();
                if (currTile == null) continue;

                // initialize tile
                BoardManager.Instance.SetTile(currTile, tileNum);
                currTile.Initialize(tileNum);

                // show sprite
                currTile.GetComponent<SpriteRenderer>().enabled = true;
            }

            var tasks = tileDatas
                .Where(tileData => tileData.type == ETileType.Warp)
                .Select(tileData => CreateWarpTiles(tileData.number, tileData.pairNumber))
                .Concat(
                    tileDatas
                        .Where(tileData => tileData.type == ETileType.Holy || tileData.type == ETileType.Spiderweb || tileData.type == ETileType.Ice)
                        .Select(tileData => AddressableAssetManager.Instantiate(_prefabAddressableKeys[tileData.type]).ToUniTask()
                                    .ContinueWith(tileObj => {
                                        tileObj.GetComponent<AbstractTileController>().Initialize(tileData.number);
                                        BoardManager.Instance.SetTile(tileObj.GetComponent<AbstractTileController>(), tileData.number);
                                        tileObj.GetComponent<SpriteRenderer>().enabled = true;
                                    })
                        )
                );

            return UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// ワープタイルの作成 (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        private static UniTask CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTask = AddressableAssetManager.Instantiate(Constants.Address.WARP_TILE_PREFAB).ToUniTask().ContinueWith(firstTile => {
                var secondTask = AddressableAssetManager.Instantiate(Constants.Address.WARP_TILE_PREFAB).ToUniTask();
                secondTask.ContinueWith(secondTile => {
                    firstTile.GetComponent<WarpTileController>().Initialize(firstTileNum, secondTile);
                    secondTile.GetComponent<WarpTileController>().Initialize(secondTileNum, firstTile);
                    BoardManager.Instance.SetTile(firstTile.GetComponent<AbstractTileController>(), firstTileNum);
                    BoardManager.Instance.SetTile(secondTile.GetComponent<AbstractTileController>(), secondTileNum);

                    firstTile.GetComponent<SpriteRenderer>().enabled = true;
                    secondTile.GetComponent<SpriteRenderer>().enabled = true;
                });
            });

            return firstTask;
        }
    }
}

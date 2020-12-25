using System;
using System.Collections.Generic;
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
        private readonly Dictionary<ETileType, string> _prefabAddressableKeys = new Dictionary<ETileType, string>()
        {
            {ETileType.Normal, Constants.Address.NORMAL_TILE_PREFAB},
            {ETileType.Warp, Constants.Address.WARP_TILE_PREFAB},
            {ETileType.Ice, Constants.Address.ICE_TILE_PREFAB},
            {ETileType.Holy, Constants.Address.HOLY_TILE_PREFAB},
            {ETileType.Spiderweb, Constants.Address.SPIDERWEB_TILE_PREFAB},
        };

        public List<UniTask> CreateTiles(ICollection<TileData> tileDatas)
        {
            // シーンに配置したノーマルタイルを初期化
            for (var tileNum = 1; tileNum <= Constants.StageSize.ROW * Constants.StageSize.COLUMN; ++tileNum) {
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

            var tasks = new List<UniTask>();
            foreach (var tileData in tileDatas) {
                switch (tileData.type) {
                    case ETileType.Normal:
                        break;
                    case ETileType.Warp:
                        tasks.Add(CreateWarpTiles(tileData.number, tileData.pairNumber));
                        break;
                    case ETileType.Holy:
                    case ETileType.Spiderweb:
                    case ETileType.Ice:
                        var task = AddressableAssetManager.Instantiate(_prefabAddressableKeys[tileData.type]).ToUniTask();
                        task.ContinueWith(tileObj => {
                            tileObj.GetComponent<AbstractTileController>().Initialize(tileData.number);
                            BoardManager.Instance.SetTile(tileObj.GetComponent<AbstractTileController>(), tileData.number);
                            tileObj.GetComponent<SpriteRenderer>().enabled = true;
                        });
                        tasks.Add(task);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return tasks;
        }

        /// <summary>
        /// ワープタイルの作成 (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        private static UniTask<GameObject> CreateWarpTiles(int firstTileNum, int secondTileNum)
        {
            var firstTask = AddressableAssetManager.Instantiate(Constants.Address.WARP_TILE_PREFAB).ToUniTask();

            firstTask.ContinueWith(firstTile => {
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

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class TileGenerator : SingletonObjectBase<TileGenerator>
    {
        private readonly Dictionary<ETileType, string> _prefabAddressableKeys = new Dictionary<ETileType, string> {
            { ETileType.Normal, Constants.Address.NORMAL_TILE_PREFAB },
            { ETileType.Goal, Constants.Address.GOAL_TILE_PREFAB },
            { ETileType.Warp, Constants.Address.WARP_TILE_PREFAB },
            { ETileType.Ice, Constants.Address.ICE_TILE_PREFAB },
            { ETileType.Holy, Constants.Address.HOLY_TILE_PREFAB },
            { ETileType.Spiderweb, Constants.Address.SPIDERWEB_TILE_PREFAB },
        };

        public UniTask CreateTilesAsync(ICollection<TileData> tileDatas)
        {
            // 生成する特殊Tileの番号リスト
            var tileNumList = tileDatas.Select(tileData => tileData.number).Concat(tileDatas.Select(tileData => tileData.pairNumber));

            var tasks = tileDatas
                .Where(tileData => tileData.type == ETileType.Warp)
                .Select(tileData => CreateWarpTilesAsync(tileData.number, tileData.pairNumber))
                .Concat(
                    // WarpTile以外の特殊Tileの生成
                    tileDatas.Where(tileData => tileData.type != ETileType.Warp)
                        .Select(tileData => CreateTileAsync(tileData))
                )
                .Concat(
                    // NormalTileの生成
                    Enumerable.Range(1, Constants.StageSize.ROW * Constants.StageSize.COLUMN)
                        .Where(tileNum => !tileNumList.Contains((short)tileNum))
                        .Select(tileNum => CreateNormalTileAsync(tileNum))
                );
            return UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// WarpTileを生成する (2つで1組)
        /// </summary>
        /// <param name="firstTileNum"> ワープタイル1 </param>
        /// <param name="secondTileNum"> ワープタイル2 </param>
        private static UniTask CreateWarpTilesAsync(int firstTileNum, int secondTileNum)
        {
            var firstTask = AddressableAssetManager.Instantiate(Constants.Address.WARP_TILE_PREFAB).ToUniTask().ContinueWith(firstTile => {
                var secondTask = AddressableAssetManager.Instantiate(Constants.Address.WARP_TILE_PREFAB).ToUniTask();
                secondTask.ContinueWith(secondTile => {
                    firstTile.GetComponent<WarpTileController>().Initialize(firstTileNum, secondTile);
                    secondTile.GetComponent<WarpTileController>().Initialize(secondTileNum, firstTile);
                    BoardManager.Instance.SetTile(firstTile.GetComponent<TileControllerBase>(), firstTileNum);
                    BoardManager.Instance.SetTile(secondTile.GetComponent<TileControllerBase>(), secondTileNum);
                });
            });

            return firstTask;
        }

        /// <summary>
        /// WarpTile以外の特殊Tileを生成する
        /// </summary>
        /// <param name="type"> Tileの種類 </param>
        /// <param name="tileNum"> Tileの番号 </param>
        /// <returns></returns>
        private UniTask CreateTileAsync(TileData tileData)
        {
            return AddressableAssetManager.Instantiate(_prefabAddressableKeys[tileData.type]).ToUniTask()
                .ContinueWith(tileObj => {
                    tileObj.GetComponent<TileControllerBase>().Initialize(tileData);
                    BoardManager.Instance.SetTile(tileObj.GetComponent<TileControllerBase>(), tileData.number);
                });
        }

        /// <summary>
        /// NormalTileを生成する
        /// </summary>
        /// <param name="type"> Tileの種類 </param>
        /// <param name="tileNum"> Tileの番号 </param>
        /// <returns></returns>
        private UniTask CreateNormalTileAsync(int tileNum)
        {
            return AddressableAssetManager.Instantiate(_prefabAddressableKeys[ETileType.Normal]).ToUniTask()
                .ContinueWith(tileObj => {
                    tileObj.GetComponent<TileControllerBase>().Initialize(tileNum);
                    BoardManager.Instance.SetTile(tileObj.GetComponent<TileControllerBase>(), tileNum);
                });
        }
    }
}

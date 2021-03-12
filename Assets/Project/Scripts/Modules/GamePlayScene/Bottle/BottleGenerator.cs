using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public static class BottleGenerator
    {
        private static readonly Dictionary<EBottleType, string> _prefabAddressableKeys = new Dictionary<EBottleType, string> {
            { EBottleType.Dynamic, Constants.Address.DYNAMIC_DUMMY_BOTTLE_PREFAB },
            { EBottleType.Static, Constants.Address.STATIC_DUMMY_BOTTLE_PREFAB },
            { EBottleType.Normal, Constants.Address.GOAL_BOTTLE_PREFAB },
            { EBottleType.AttackableDummy, Constants.Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB },
            { EBottleType.Erasable, Constants.Address.ERASABLE_BOTTLE_PREFAB }
        };

        public static UniTask CreateBottles(List<BottleData> bottleDatas)
        {
            var tasks = bottleDatas
                .Where(bottleData => _prefabAddressableKeys.ContainsKey(bottleData.type))
                .Select(bottleData => AddressableAssetManager.Instantiate(_prefabAddressableKeys[bottleData.type]).ToUniTask()
                            .ContinueWith(async bottle => await bottle.GetComponent<AbstractBottleController>().Initialize(bottleData))
                );

            return UniTask.WhenAll(tasks);
        }
    }
}

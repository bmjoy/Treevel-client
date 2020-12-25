using System.Collections.Generic;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Cysharp.Threading.Tasks;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public static class BottleGenerator
    {
        private static Dictionary<EBottleType, string> _prefabAddressableKeys = new Dictionary<EBottleType, string>()
        {
            {EBottleType.Dynamic, Constants.Address.DYNAMIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Static, Constants.Address.STATIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Normal, Constants.Address.NORMAL_BOTTLE_PREFAB},
            {EBottleType.AttackableDummy, Constants.Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB},
        };

        public static List<UniTask> CreateBottles(List<BottleData> bottleDatas)
        {
            var tasks = new List<UniTask>();
            bottleDatas.ForEach(bottleData => {
                if (!_prefabAddressableKeys.ContainsKey(bottleData.type))
                    return;

                var task = AddressableAssetManager.Instantiate(_prefabAddressableKeys[bottleData.type]).ToUniTask();
                task.ContinueWith(bottle =>
                {
                    bottle.GetComponent<AbstractBottleController>().Initialize(bottleData);
                });
                tasks.Add(task);
            });
            return tasks;
        }
    }
}

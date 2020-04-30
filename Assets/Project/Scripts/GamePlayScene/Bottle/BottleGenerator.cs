using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public static class BottleGenerator
    {
        private static Dictionary<EBottleType, string> _prefabAddressableKeys = new Dictionary<EBottleType, string>()
        {
            {EBottleType.Dynamic, Address.DYNAMIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Static, Address.STATIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Number, Address.NORMAL_BOTTLE_PREFAB},
            {EBottleType.LifeNumber, Address.LIFE_BOTTLE_PREFAB},
            {EBottleType.LifeDummy, Address.LIFE_DUMMY_BOTTLE_PREFAB},
        };

        public static void CreateBottles(List<BottleData> bottleDatas)
        {
            bottleDatas.ForEach(async bottleData => {
                if (!_prefabAddressableKeys.ContainsKey(bottleData.type))
                    return;

                var bottle = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[bottleData.type]).Task;
                bottle.GetComponent<AbstractBottleController>().Initialize(bottleData);
            });
        }
    }
}

using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public class BottleGenerator : SingletonObject<BottleGenerator>
    {
        private Dictionary<EBottleType, string> _prefabAddressableKeys = new Dictionary<EBottleType, string>()
        {
            {EBottleType.Dynamic, Address.DYNAMIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Static, Address.STATIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Normal, Address.NORMAL_BOTTLE_PREFAB},
            {EBottleType.Life, Address.LIFE_BOTTLE_PREFAB},
            {EBottleType.AttackableDummy, Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB},
        };

        public void CreateBottles(List<BottleData> bottleDatas)
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

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
            {EBottleType.Number, Address.NUMBER_BOTTLE_PREFAB},
            {EBottleType.LifeNumber, Address.LIFE_NUMBER_BOTTLE_PREFAB}
        };

        public static void CreatePanels(List<BottleData> panelDatas)
        {
            panelDatas.ForEach(async panelData => {
                if (!_prefabAddressableKeys.ContainsKey(panelData.type))
                    return;

                var panel = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[panelData.type]).Task;
                panel.GetComponent<AbstractBottleController>().Initialize(panelData);
            });
        }
    }
}

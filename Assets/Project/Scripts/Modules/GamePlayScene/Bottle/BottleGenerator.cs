﻿using System.Collections.Generic;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public static class BottleGenerator
    {
        private static Dictionary<EBottleType, string> _prefabAddressableKeys = new Dictionary<EBottleType, string>()
        {
            {EBottleType.Dynamic, Constants.Address.DYNAMIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Static, Constants.Address.STATIC_DUMMY_BOTTLE_PREFAB},
            {EBottleType.Normal, Constants.Address.NORMAL_BOTTLE_PREFAB},
            {EBottleType.Life, Constants.Address.LIFE_BOTTLE_PREFAB},
            {EBottleType.AttackableDummy, Constants.Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB},
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

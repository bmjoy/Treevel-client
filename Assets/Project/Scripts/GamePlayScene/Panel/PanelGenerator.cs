using System;
using System.Linq;
using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public static class PanelGenerator
    {
        private static Dictionary<EPanelType, string> _prefabAddressableKeys = new Dictionary<EPanelType, string>(){
            {EPanelType.Dynamic, Address.DYNAMIC_DUMMY_PANEL_PREFAB},
            {EPanelType.Static, Address.STATIC_DUMMY_PANEL_PREFAB},
            {EPanelType.Number, Address.NUMBER_PANEL_PREFAB},
            {EPanelType.LifeNumber, Address.LIFE_NUMBER_PANEL_PREFAB}
        };

        public static void CreatePanels(ICollection<PanelData> panelDatas)
        {
            foreach (var panelData in panelDatas) {
                if (!_prefabAddressableKeys.ContainsKey(panelData.type))
                    continue;

                AddressableAssetManager.Instantiate(_prefabAddressableKeys[panelData.type]).Completed += (handle) => {
                    var panel = handle.Result;
                    panel.GetComponent<PanelController>().Initialize(panelData);
                };
            }
        }
    }
}

using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Panel
{
    public static class PanelGenerator
    {
        private static Dictionary<EPanelType, string> _prefabAddressableKeys = new Dictionary<EPanelType, string>()
        {
            {EPanelType.Dynamic, Address.DYNAMIC_DUMMY_PANEL_PREFAB},
            {EPanelType.Static, Address.STATIC_DUMMY_PANEL_PREFAB},
            {EPanelType.Number, Address.NUMBER_PANEL_PREFAB},
            {EPanelType.LifeNumber, Address.LIFE_NUMBER_PANEL_PREFAB}
        };

        public static void CreatePanels(List<PanelData> panelDatas)
        {
            panelDatas.ForEach(async panelData => {
                if (!_prefabAddressableKeys.ContainsKey(panelData.type))
                    return;

                var panel = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[panelData.type]).Task;
                panel.GetComponent<PanelController>().Initialize(panelData);
            });
        }
    }
}

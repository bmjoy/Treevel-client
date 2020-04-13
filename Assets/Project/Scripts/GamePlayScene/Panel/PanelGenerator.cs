using System;
using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public static class PanelGenerator
    {
        public static void CreatePanels(ICollection<PanelData> panelDatas)
        {
            foreach (var panelData in panelDatas) {
                switch (panelData.type) {
                    case EPanelType.Number:
                        AddressableAssetManager.Instantiate(Address.NUMBER_PANEL_PREFAB).Completed += (op) => {
                            var numberPanel = op.Result;
                            var panelSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.panelSprite);
                            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.targetTileSprite);
                            numberPanel.GetComponent<NumberPanelController>().Initialize(panelData.initPos, panelData.targetPos, panelSprite, targetTileSprite);
                        };
                        break;
                    case EPanelType.Dynamic:
                        CreateDynamicDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.Static:
                        CreateStaticDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.LifeNumber:
                        AddressableAssetManager.Instantiate(Address.LIFE_NUMBER_PANEL_PREFAB).Completed += (op) => {
                            var lifeNumberPanel = op.Result;
                            var panelSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.panelSprite);
                            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.targetTileSprite);
                            lifeNumberPanel.GetComponent<LifeNumberPanelController>().Initialize(panelData.initPos, panelData.targetPos, panelSprite, targetTileSprite, panelData.life);
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 動かないダミーパネルを作成する
        /// </summary>
        /// <param name="initialTileNum"> 配置するタイルの番号 </param>
        private static void CreateStaticDummyPanel(int initialTileNum)
        {
            AddressableAssetManager.Instantiate(Address.STATIC_DUMMY_PANEL_PREFAB).Completed += (op) => {
                var panel = op.Result;
                panel.GetComponent<StaticPanelController>().Initialize(initialTileNum);
            };
        }

        /// <summary>
        /// 動くダミーパネルを作成する
        /// </summary>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        private static void CreateDynamicDummyPanel(int initialTileNum)
        {
            AddressableAssetManager.Instantiate(Address.DYNAMIC_DUMMY_PANEL_PREFAB).Completed += (op) => {
                var panel = op.Result;
                panel.GetComponent<DynamicPanelController>().Initialize(initialTileNum);
            };
        }
    }
}

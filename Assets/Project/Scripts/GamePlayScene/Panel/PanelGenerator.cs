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
                        AddressableAssetManager.Instantiate("numberPanelPrefab").Completed += (op) => {
                            var numberPanel = op.Result;
                            var numberPanelSprite = AddressableAssetManager.GetAsset<Sprite>($"numberPanel{panelData.number}").Result;
                            if (numberPanelSprite != null) numberPanel.GetComponent<SpriteRenderer>().sprite = numberPanelSprite;
                            numberPanel.GetComponent<NumberPanelController>().Initialize(panelData.number, panelData.initPos, panelData.targetPos);
                        };
                        break;
                    case EPanelType.Dynamic:
                        CreateDynamicDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.Static:
                        CreateStaticDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.LifeNumber:
                        AddressableAssetManager.Instantiate("lifeNumberPanelPrefab").Completed += (op) => {
                            var lifeNumberPanel = op.Result;
                            var lifeNumberPanelSprite = AddressableAssetManager.GetAsset<Sprite>($"lifeNumberPanel{panelData.number}").Result;
                            if (lifeNumberPanelSprite != null) lifeNumberPanel.GetComponent<SpriteRenderer>().sprite = lifeNumberPanelSprite;
                            lifeNumberPanel.GetComponent<LifeNumberPanelController>().Initialize(panelData.number, panelData.initPos, panelData.targetPos, panelData.life);
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
            AddressableAssetManager.Instantiate("staticDummyPanelPrefab").Completed += (op) => {
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
            AddressableAssetManager.Instantiate("dynamicDummyPanelPrefab").Completed += (op) => {
                var panel = op.Result;
                panel.GetComponent<DynamicPanelController>().Initialize(initialTileNum);
            };
        }
    }
}

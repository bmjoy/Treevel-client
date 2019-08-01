using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class PanelGenerator : SingletonObject<PanelGenerator>
    {
        public GameObject numberPanel1Prefab;
        public GameObject numberPanel2Prefab;
        public GameObject numberPanel3Prefab;
        public GameObject numberPanel4Prefab;
        public GameObject numberPanel5Prefab;
        public GameObject numberPanel6Prefab;
        public GameObject numberPanel7Prefab;
        public GameObject numberPanel8Prefab;
        public GameObject staticDummyPanelPrefab;
        public GameObject dynamicDummyPanelPrefab;

        private List<GameObject> numberPanelPrefabs;

        private TileGenerator tileGenerator;

        private void Awake()
        {
            numberPanelPrefabs = new List<GameObject>
            {
                numberPanel1Prefab,
                numberPanel2Prefab,
                numberPanel3Prefab,
                numberPanel4Prefab,
                numberPanel5Prefab,
                numberPanel6Prefab,
                numberPanel7Prefab,
                numberPanel8Prefab
            };

            tileGenerator = GameObject.Find("TileGenerator").GetComponent<TileGenerator>();
        }

        public void PrepareTilesAndCreateNumberPanels(List<Dictionary<string, int>> numberPanelParams)
        {
            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字タイルの作成
                tileGenerator.CreateNumberTile(panelNum, finalTileNum);
            }

            // ノーマルタイルの一括作成
            tileGenerator.CreateNormalTiles();

            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var initialTileNum = numberPanelParam["initialTileNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字パネルの作成
                var panel = Instantiate(numberPanelPrefabs[panelNum - 1]);
                panel.GetComponent<NumberPanelController>().Initialize(panelNum, initialTileNum, finalTileNum);
            }
        }

        public void CreateStaticDummyPanel(int initialTileNum)
        {
            var panel = Instantiate(staticDummyPanelPrefab);
            panel.GetComponent<StaticPanelController>().Initialize(initialTileNum);
        }

        public void CreateDynamicDummyPanel(int initialTileNum)
        {
            var panel = Instantiate(dynamicDummyPanelPrefab);
            panel.GetComponent<DynamicPanelController>().Initialize(initialTileNum);
        }

        /* 変数をもらい辞書型に変換 */
        public static Dictionary<string, int> ComvartToDictionary(int panelNum, int initialTileNum, int finalTileNum)
        {
            return new Dictionary<string, int>()
            {
                {"panelNum", panelNum},
                {"initialTileNum", initialTileNum},
                {"finalTileNum", finalTileNum}
            };
        }
    }
}

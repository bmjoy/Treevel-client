using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using static Project.Scripts.GameDatas.StageData;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class PanelGenerator : SingletonObject<PanelGenerator>
    {
        [SerializeField] private GameObject _numberPanel1Prefab;
        [SerializeField] private GameObject _numberPanel2Prefab;
        [SerializeField] private GameObject _numberPanel3Prefab;
        [SerializeField] private GameObject _numberPanel4Prefab;
        [SerializeField] private GameObject _numberPanel5Prefab;
        [SerializeField] private GameObject _numberPanel6Prefab;
        [SerializeField] private GameObject _numberPanel7Prefab;
        [SerializeField] private GameObject _numberPanel8Prefab;
        [SerializeField] private GameObject _lifeNumberPanel1Prefab;
        [SerializeField] private GameObject _lifeNumberPanel2Prefab;
        [SerializeField] private GameObject _lifeNumberPanel3Prefab;
        [SerializeField] private GameObject _lifeNumberPanel4Prefab;
        [SerializeField] private GameObject _lifeNumberPanel5Prefab;
        [SerializeField] private GameObject _lifeNumberPanel6Prefab;
        [SerializeField] private GameObject _lifeNumberPanel7Prefab;
        [SerializeField] private GameObject _lifeNumberPanel8Prefab;

        [SerializeField] private GameObject _staticDummyPanelPrefab;
        [SerializeField] private GameObject _dynamicDummyPanelPrefab;

        private List<GameObject> _numberPanelPrefabs;
        private List<GameObject> _lifeNumberPanelPrefabs;

        private TileGenerator _tileGenerator;

        private void Awake()
        {
            _numberPanelPrefabs = new List<GameObject> {
                _numberPanel1Prefab,
                _numberPanel2Prefab,
                _numberPanel3Prefab,
                _numberPanel4Prefab,
                _numberPanel5Prefab,
                _numberPanel6Prefab,
                _numberPanel7Prefab,
                _numberPanel8Prefab
            };

            _lifeNumberPanelPrefabs = new List<GameObject> {
                _lifeNumberPanel1Prefab,
                _lifeNumberPanel2Prefab,
                _lifeNumberPanel3Prefab,
                _lifeNumberPanel4Prefab,
                _lifeNumberPanel5Prefab,
                _lifeNumberPanel6Prefab,
                _lifeNumberPanel7Prefab,
                _lifeNumberPanel8Prefab
            };

            _tileGenerator = FindObjectOfType<TileGenerator>();
        }

        public void CreatePanels(ICollection<PanelData> panelDatas)
        {
            foreach (PanelData panelData in panelDatas) {
                switch (panelData.type) {
                    case EPanelType.Number:
                        int panelNum = int.Parse(panelData.parameters[0]);
                        int finalTileNum = int.Parse(panelData.parameters[1]);

                        // 数字タイルの作成
                        _tileGenerator.CreateNumberTile(panelNum, finalTileNum);
                        break;
                }
            }

            // _tileGenerator.MakeRelations();
            _tileGenerator.CreateNormalTiles();

            foreach (PanelData panelData in panelDatas) {
                switch (panelData.type) {
                    case EPanelType.Number:
                        int panelNum = int.Parse(panelData.parameters[0]);
                        int initialTileNum = panelData.position;
                        int finalTileNum = int.Parse(panelData.parameters[1]);

                        // _tileGenerator.CreateNumberTile(panelNum, finalTileNum);
                        var panel = Instantiate(_numberPanelPrefabs[panelNum - 1]);
                        panel.GetComponent<NumberPanelController>().Initialize(panelNum, initialTileNum, finalTileNum);
                        break;
                    case EPanelType.Dynamic:
                        CreateDynamicDummyPanel(panelData.position);
                        break;
                    case EPanelType.Static:
                        CreateStaticDummyPanel(panelData.position);
                        break;
                }
            }
        }

        /// <summary>
        /// 必要なタイルを準備してから，数字パネルを作成する
        /// </summary>
        /// <param name="numberPanelParams"> ComvartToDictionary によって変換された辞書型リスト </param>
        public void PrepareTilesAndCreateNumberPanels(List<Dictionary<string, int>> numberPanelParams)
        {
            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字タイルの作成
                _tileGenerator.CreateNumberTile(panelNum, finalTileNum);
            }

            // ノーマルタイルの一括作成
            _tileGenerator.CreateNormalTiles();

            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var initialTileNum = numberPanelParam["initialTileNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字パネルの作成
                var panel = Instantiate(_numberPanelPrefabs[panelNum - 1]);
                panel.GetComponent<NumberPanelController>().Initialize(panelNum, initialTileNum, finalTileNum);
            }
        }

        public void PrepareTilesAndCreateLifeNumberPanels(List<Dictionary<string, int>> numberPanelParams)
        {
            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字タイルの作成
                _tileGenerator.CreateNumberTile(panelNum, finalTileNum);
            }

            // ノーマルタイルの一括作成
            _tileGenerator.CreateNormalTiles();

            foreach (Dictionary<string, int> numberPanelParam in numberPanelParams) {
                // パラメータの取得
                var panelNum = numberPanelParam["panelNum"];
                var initialTileNum = numberPanelParam["initialTileNum"];
                var finalTileNum = numberPanelParam["finalTileNum"];
                // 数字パネルの作成
                var panel = Instantiate(_lifeNumberPanelPrefabs[panelNum - 1]);
                panel.GetComponent<NumberPanelController>().Initialize(panelNum, initialTileNum, finalTileNum);
            }
        }

        /// <summary>
        /// 動かないダミーパネルを作成する
        /// </summary>
        /// <param name="initialTileNum"> 配置するタイルの番号 </param>
        public void CreateStaticDummyPanel(int initialTileNum)
        {
            var panel = Instantiate(_staticDummyPanelPrefab);
            panel.GetComponent<StaticPanelController>().Initialize(initialTileNum);
        }

        /// <summary>
        /// 動くダミーパネルを作成する
        /// </summary>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        public void CreateDynamicDummyPanel(int initialTileNum)
        {
            var panel = Instantiate(_dynamicDummyPanelPrefab);
            panel.GetComponent<DynamicPanelController>().Initialize(initialTileNum);
        }

        /// <summary>
        /// 辞書型に変換 (PrepareTilesAndCreateNumberPanels の引数のため)
        /// </summary>
        /// <param name="panelNum"> 作成されたパネルの番号 </param>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        /// <param name="finalTileNum"> パネルのゴールとなるタイルの番号 </param>
        /// <returns></returns>
        public static Dictionary<string, int> ComvartToDictionary(int panelNum, int initialTileNum, int finalTileNum)
        {
            return new Dictionary<string, int>() {
                {"panelNum", panelNum},
                {"initialTileNum", initialTileNum},
                {"finalTileNum", finalTileNum}
            };
        }
    }
}

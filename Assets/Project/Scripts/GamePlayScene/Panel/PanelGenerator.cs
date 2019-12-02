using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using static Project.Scripts.GameDatas.StageData;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class PanelGenerator : SingletonObject<PanelGenerator>
    {
        [SerializeField] private GameObject _numberPanelPrefab;
        [SerializeField] private GameObject _lifeNumberPanelPrefab;

        [SerializeField] private GameObject _staticDummyPanelPrefab;
        [SerializeField] private GameObject _dynamicDummyPanelPrefab;

        private TileGenerator _tileGenerator;

        private void Awake()
        {
            _tileGenerator = FindObjectOfType<TileGenerator>();
        }

        public void CreatePanels(ICollection<PanelData> panelDatas)
        {
            foreach (PanelData panelData in panelDatas) {
                switch (panelData.type) {
                    case EPanelType.Number:
                        // 数字タイルの作成
                        _tileGenerator.CreateNumberTile(panelData.number, panelData.targetPos);
                        break;
                }
            }

            // _tileGenerator.MakeRelations();
            _tileGenerator.CreateNormalTiles();

            foreach (PanelData panelData in panelDatas) {
                switch (panelData.type) {
                    case EPanelType.Number:
                        var numberPanel = Instantiate(_numberPanelPrefab);
                        var numberPanelSprite = Resources.Load<Sprite>("Textures/Panel/numberPanel" + panelData.number);
                        if (numberPanelSprite != null) numberPanel.GetComponent<SpriteRenderer>().sprite = numberPanelSprite;
                        numberPanel.GetComponent<NumberPanelController>().Initialize(panelData.number, panelData.initPos, panelData.targetPos);
                        break;
                    case EPanelType.Dynamic:
                        CreateDynamicDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.Static:
                        CreateStaticDummyPanel(panelData.initPos);
                        break;
                    case EPanelType.LifeNumber:
                        var lifeNumberPanel = Instantiate(_lifeNumberPanelPrefab);
                        var lifeNumberPanelSprite = Resources.Load<Sprite>("Textures/Panel/lifeNumberPanel" + panelData.number);
                        if (lifeNumberPanelSprite != null) lifeNumberPanel.GetComponent<SpriteRenderer>().sprite = lifeNumberPanelSprite;
                        lifeNumberPanel.GetComponent<LifeNumberPanelController>().Initialize(panelData.number, panelData.initPos, panelData.targetPos, panelData.life);
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
                var panel = Instantiate(_numberPanelPrefab);
                var sprite = Resources.Load<Sprite>("Textures/Panel/numberPanel" + panelNum);
                if (sprite != null) panel.GetComponent<SpriteRenderer>().sprite = sprite;
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
                var panel = Instantiate(_lifeNumberPanelPrefab);
                var sprite = Resources.Load<Sprite>("Textures/Panel/lifeNumberPanel" + panelNum);
                if (sprite != null) panel.GetComponent<SpriteRenderer>().sprite = sprite;
                panel.GetComponent<LifeNumberPanelController>().Initialize(panelNum, initialTileNum, finalTileNum);
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

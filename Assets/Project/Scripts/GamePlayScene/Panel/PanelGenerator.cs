using System;
using Project.Scripts.Library.Data;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class PanelGenerator : MonoBehaviour
	{
		public GameObject normalPanelPrefab;
		public GameObject numberPanel1Prefab;
		public GameObject numberPanel2Prefab;
		public GameObject numberPanel3Prefab;
		public GameObject numberPanel4Prefab;
		public GameObject numberPanel5Prefab;
		public GameObject numberPanel6Prefab;
		public GameObject numberPanel7Prefab;
		public GameObject numberPanel8Prefab;
		public GameObject staticDummyPanelPrefab;

		// 現段階では8枚のパネル群
		public void CreatePanels(int stageId)
		{
			switch (stageId)
			{
				case 1:
					CreateNumberPanel(initialTileNum: "4", finalTileNum: "4", panelPrefab: numberPanel1Prefab);
					CreateNumberPanel(initialTileNum: "5", finalTileNum: "5", panelPrefab: numberPanel2Prefab);
					CreateNumberPanel(initialTileNum: "6", finalTileNum: "6", panelPrefab: numberPanel3Prefab);
					CreateNumberPanel(initialTileNum: "7", finalTileNum: "7", panelPrefab: numberPanel4Prefab);
					CreateNumberPanel(initialTileNum: "8", finalTileNum: "8", panelPrefab: numberPanel5Prefab);
					CreateNumberPanel(initialTileNum: "9", finalTileNum: "9", panelPrefab: numberPanel6Prefab);
					CreateNumberPanel(initialTileNum: "10", finalTileNum: "10", panelPrefab: numberPanel7Prefab);
					CreateNumberPanel(initialTileNum: "14", finalTileNum: "11", panelPrefab: numberPanel8Prefab);
					CreateStaticDummyPanel(initialTileNum: "15", panelPrefab: staticDummyPanelPrefab);
					break;
				case 2:
					CreateNumberPanel(initialTileNum: "1", finalTileNum: "4", panelPrefab: numberPanel1Prefab);
					CreateNumberPanel(initialTileNum: "3", finalTileNum: "5", panelPrefab: numberPanel2Prefab);
					CreateNumberPanel(initialTileNum: "5", finalTileNum: "6", panelPrefab: numberPanel3Prefab);
					CreateNumberPanel(initialTileNum: "6", finalTileNum: "7", panelPrefab: numberPanel4Prefab);
					CreateNumberPanel(initialTileNum: "8", finalTileNum: "8", panelPrefab: numberPanel5Prefab);
					CreateNumberPanel(initialTileNum: "11", finalTileNum: "9", panelPrefab: numberPanel6Prefab);
					CreateNumberPanel(initialTileNum: "13", finalTileNum: "10", panelPrefab: numberPanel7Prefab);
					CreateNumberPanel(initialTileNum: "15", finalTileNum: "11", panelPrefab: numberPanel8Prefab);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private static void CreateNumberPanel(string initialTileNum, string finalTileNum, GameObject panelPrefab)
		{
			// 引数に渡されたPrefabを元にオブジェクトを生成
			var panel = Instantiate(panelPrefab);
			panel.name = "NumberPanel";
			Setup(initialTileNum, ref panel);
			// 最終タイルを登録
			var finalTile = GameObject.Find("Tile" + finalTileNum);
			panel.GetComponent<PanelController>().Initialize(finalTile);
		}

		private static void CreateStaticDummyPanel(string initialTileNum, GameObject panelPrefab)
		{
			var panel = Instantiate(panelPrefab);
			panel.name = "StaticPanel";
			Setup(initialTileNum, ref panel);
		}

		// refは参照渡し
		private static void Setup(string initialTileNum, ref GameObject panel)
		{
			// 初期位置にするタイルを取得
			var initialTile = GameObject.Find("Tile" + initialTileNum);
			// パネルの初期設定
			panel.transform.localScale = new Vector2(PanelSize.WIDTH * 0.5f, PanelSize.HEIGHT * 0.5f);
			panel.transform.parent = initialTile.transform;
			panel.transform.position = initialTile.transform.position;
			panel.GetComponent<Renderer>().sortingLayerName = "Panel";
		}
	}
}

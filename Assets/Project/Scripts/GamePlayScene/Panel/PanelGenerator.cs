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

		// 現段階では8枚のパネル群
		public void CreatePanels(string stageNum)
		{
			switch (stageNum)
			{
				case "1":
					CreateOnePanel(initialTileNum: "4", finalTileNum: "4", panelPrefab: numberPanel1Prefab);
					CreateOnePanel(initialTileNum: "5", finalTileNum: "5", panelPrefab: numberPanel2Prefab);
					CreateOnePanel(initialTileNum: "6", finalTileNum: "6", panelPrefab: numberPanel3Prefab);
					CreateOnePanel(initialTileNum: "7", finalTileNum: "7", panelPrefab: numberPanel4Prefab);
					CreateOnePanel(initialTileNum: "8", finalTileNum: "8", panelPrefab: numberPanel5Prefab);
					CreateOnePanel(initialTileNum: "9", finalTileNum: "9", panelPrefab: numberPanel6Prefab);
					CreateOnePanel(initialTileNum: "10", finalTileNum: "10", panelPrefab: numberPanel7Prefab);
					CreateOnePanel(initialTileNum: "14", finalTileNum: "11", panelPrefab: numberPanel8Prefab);
					break;
				case "2":
					CreateOnePanel(initialTileNum: "1", finalTileNum: "4", panelPrefab: numberPanel1Prefab);
					CreateOnePanel(initialTileNum: "3", finalTileNum: "5", panelPrefab: numberPanel2Prefab);
					CreateOnePanel(initialTileNum: "5", finalTileNum: "6", panelPrefab: numberPanel3Prefab);
					CreateOnePanel(initialTileNum: "6", finalTileNum: "7", panelPrefab: numberPanel4Prefab);
					CreateOnePanel(initialTileNum: "8", finalTileNum: "8", panelPrefab: numberPanel5Prefab);
					CreateOnePanel(initialTileNum: "11", finalTileNum: "9", panelPrefab: numberPanel6Prefab);
					CreateOnePanel(initialTileNum: "13", finalTileNum: "10", panelPrefab: numberPanel7Prefab);
					CreateOnePanel(initialTileNum: "15", finalTileNum: "11", panelPrefab: numberPanel8Prefab);
					break;
				default:
					throw new NotImplementedException();
					break;
			}
		}

		private static void CreateOnePanel(string initialTileNum, string finalTileNum, GameObject panelPrefab)
		{
			// 初期位置にするタイルを取得
			GameObject initialTile = GameObject.Find("Tile" + initialTileNum);
			// 最終位置にするタイルを取得
			GameObject finalTile = GameObject.Find("Tile" + finalTileNum);
			// 引数に渡されたPrefabを元にオブジェクトを生成
			GameObject panel = Instantiate(panelPrefab);
			// パネルの初期設定
			panel.name = "Panel";
			panel.transform.localScale = new Vector2(PanelSize.WIDTH * 0.5f, PanelSize.HEIGHT * 0.5f);
			panel.transform.parent = initialTile.transform;
			panel.transform.position = initialTile.transform.position;
			// 最終タイルを登録
			panel.GetComponent<PanelController>().Initialize(finalTile);
			panel.GetComponent<Renderer>().sortingLayerName = "Panel";
		}
	}
}

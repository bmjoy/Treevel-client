using System;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class PanelGenerator : MonoBehaviour
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

		public void CreateNumberPanel(int panelNumber, int initialTileNum, int finalTileNum)
		{
			GameObject panelPrefab;
			switch (panelNumber)
			{
				case 1:
					panelPrefab = numberPanel1Prefab;
					break;
				case 2:
					panelPrefab = numberPanel2Prefab;
					break;
				case 3:
					panelPrefab = numberPanel3Prefab;
					break;
				case 4:
					panelPrefab = numberPanel4Prefab;
					break;
				case 5:
					panelPrefab = numberPanel5Prefab;
					break;
				case 6:
					panelPrefab = numberPanel6Prefab;
					break;
				case 7:
					panelPrefab = numberPanel7Prefab;
					break;
				case 8:
					panelPrefab = numberPanel8Prefab;
					break;
				default:
					throw new NotImplementedException();
			}

			var panel = Instantiate(panelPrefab);
			panel.GetComponent<NumberPanelController>().Initialize(initialTileNum, finalTileNum);
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
	}
}

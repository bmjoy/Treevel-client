using System.Collections.Generic;
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

		private List<GameObject> numberPanelPrefabs;

		private void Awake()
		{
			numberPanelPrefabs = new List<GameObject>
			{
				numberPanel1Prefab,
				numberPanel2Prefab,
				numberPanel3Prefab,
				numberPanel4Prefab,
				numberPanel4Prefab,
				numberPanel5Prefab,
				numberPanel6Prefab,
				numberPanel7Prefab,
				numberPanel8Prefab
			};
		}

		public void CreateNumberPanel(int panelNumber, int initialTileNum, int finalTileNum)
		{
			var panel = Instantiate(numberPanelPrefabs[panelNumber - 1]);
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

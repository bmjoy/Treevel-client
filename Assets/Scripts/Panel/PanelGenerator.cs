using UnityEngine;

namespace Panel
{
    public class PanelGenerator : MonoBehaviour
    {
        public GameObject normalPanelPrefab;

        // Use this for initialization
        private void Awake()
        {
            CreatePanels();
        }

        // 現段階では8枚のパネル群
        private void CreatePanels()
        {
            CreateOnePanel(GameObject.Find("Tile0"), "0");
            CreateOnePanel(GameObject.Find("Tile2"), "2");
            CreateOnePanel(GameObject.Find("Tile4"), "4");
            CreateOnePanel(GameObject.Find("Tile5"), "5");
            CreateOnePanel(GameObject.Find("Tile7"), "7");
            CreateOnePanel(GameObject.Find("Tile10"), "10");
            CreateOnePanel(GameObject.Find("Tile12"), "12");
            CreateOnePanel(GameObject.Find("Tile14"), "14");
        }

        private void CreateOnePanel(GameObject tile, string panelNum)
        {
            GameObject panel = Instantiate(normalPanelPrefab) as GameObject;
            panel.transform.localScale = new Vector2(PanelSize.WIDTH * 0.5f, PanelSize.HEIGHT * 0.5f);
            panel.transform.parent = tile.transform;
            panel.transform.position =  tile.transform.position;
            panel.name = "Panel" + panelNum;
        }
    }
}

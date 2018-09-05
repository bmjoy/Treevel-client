using UnityEngine;

namespace Panel
{
    public class PanelGenerator : MonoBehaviour
    {
        public GameObject normalPanelPrefab;

        // Awake: 初期化処理，他オブジェクトの参照はNG．
        // Start: 他オブジェクトの参照をしたい場合．
        // Tileオブジェクトを参照するため，Startを用いるが，きちんとした調査が必要．
        private void Start()
        {
            CreatePanels();
        }

        // 現段階では8枚のパネル群
        private void CreatePanels()
        {
            CreateOnePanel(GameObject.Find("Tile0"), 0);
            CreateOnePanel(GameObject.Find("Tile2"), 1);
            CreateOnePanel(GameObject.Find("Tile4"), 2);
            CreateOnePanel(GameObject.Find("Tile5"), 3);
            CreateOnePanel(GameObject.Find("Tile7"), 4);
            CreateOnePanel(GameObject.Find("Tile10"), 5);
            CreateOnePanel(GameObject.Find("Tile12"), 6);
            CreateOnePanel(GameObject.Find("Tile14"), 7);
        }

        private void CreateOnePanel(GameObject tile, int panelNum)
        {
            GameObject panel = Instantiate(normalPanelPrefab) as GameObject;
            panel.transform.localScale = new Vector2(PanelSize.WIDTH * 0.5f, PanelSize.HEIGHT * 0.5f);
            panel.transform.parent = tile.transform;
            panel.transform.position =  tile.transform.position;
            panel.name = "Panel" + panelNum.ToString();
            panel.GetComponent<Renderer>().sortingLayerName = "Panel";
        }
    }
}

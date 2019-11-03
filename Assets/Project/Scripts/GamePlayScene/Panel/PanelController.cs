using UnityEngine;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Panel
{
    public abstract class PanelController : MonoBehaviour
    {
        protected virtual void Awake()
        {
            // パネル画像のサイズを取得
            var panelWidth = GetComponent<SpriteRenderer>().size.x;
            var panelHeight = GetComponent<SpriteRenderer>().size.y;
            // パネルの初期設定
            transform.localScale = new Vector2(PanelSize.WIDTH / panelWidth, PanelSize.HEIGHT / panelHeight);
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.PANEL;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        public void Initialize(int initialTileNum)
        {
            // 初期位置にするタイルを取得
            var initialTile = TileLibrary.GetTile(initialTileNum);
            var script = initialTile.GetComponent<NormalTileController>();
            script.hasPanel = true;
            transform.parent = initialTile.transform;
            transform.position = initialTile.transform.position;
        }
    }
}

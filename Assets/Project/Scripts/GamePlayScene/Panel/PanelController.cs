using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public abstract class PanelController : MonoBehaviour
    {
        protected virtual void Awake(){}

        private void InitializeSprite()
        {
            // パネル画像のサイズを取得
            var panelWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var panelHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // パネルの初期設定
            transform.localScale = new Vector2(PanelSize.WIDTH / panelWidth, PanelSize.HEIGHT / panelHeight);
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.PANEL;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        public virtual void Initialize(PanelData panelData)
        {
            var initialTileNum = panelData.initPos;
            // 初期位置にするタイルを取得
            var initialTile = TileLibrary.GetTile(initialTileNum);
            var script = initialTile.GetComponent<NormalTileController>();
            script.hasPanel = true;
            transform.SetParent(initialTile.transform);
            transform.position = initialTile.transform.position;

            InitializeSprite();
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public abstract class AbstractPanelController : MonoBehaviour
    {
        protected virtual void Awake() {}

        private void InitializeSprite()
        {
            // パネル画像のサイズを取得
            var panelWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var panelHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // パネルの初期設定
            transform.localScale = new Vector2(PanelSize.WIDTH / panelWidth, PanelSize.HEIGHT / panelHeight);

            if (GetComponent<Collider2D>() is BoxCollider2D) {
                GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            }
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.PANEL;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        public virtual void Initialize(PanelData panelData)
        {
            var initialTileNum = panelData.initPos;

            // パネルをボードに設定
            BoardManager.SetPanel(this, initialTileNum);

            InitializeSprite();
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

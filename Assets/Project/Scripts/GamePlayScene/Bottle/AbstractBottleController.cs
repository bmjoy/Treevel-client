using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    public abstract class AbstractBottleController : MonoBehaviour
    {
        protected virtual void Awake() {}

        private void InitializeSprite()
        {
            // ボトル画像のサイズを取得
            var panelWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var panelHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // ボトルの初期設定
            transform.localScale = new Vector2(BottleSize.WIDTH / panelWidth, BottleSize.HEIGHT / panelHeight);

            if (GetComponent<Collider2D>() is BoxCollider2D) {
                GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            }
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.BOTTLE;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="panelData"> ボトルのデータ </param>
        public virtual void Initialize(BottleData panelData)
        {
            var initialTileNum = panelData.initPos;

            // ボトルをボードに設定
            BoardManager.SetPanel(this, initialTileNum);

            InitializeSprite();
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

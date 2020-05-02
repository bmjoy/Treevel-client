using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public abstract class AbstractBottleController : MonoBehaviour
    {
        protected virtual void Awake() {}

        private void InitializeSprite()
        {
            // ボトル画像のサイズを取得
            var bottleWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var bottleHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // ボトルの初期設定
            transform.localScale = new Vector2(BottleSize.WIDTH / bottleWidth, BottleSize.HEIGHT / bottleHeight);

            if (GetComponent<Collider2D>() is BoxCollider2D) {
                GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            }
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.BOTTLE;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData"> ボトルのデータ </param>
        public virtual void Initialize(BottleData bottleData)
        {
            var initialTileNum = bottleData.initPos;

            // ボトルをボードに設定
            BoardManager.SetBottle(this, initialTileNum);

            InitializeSprite();
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

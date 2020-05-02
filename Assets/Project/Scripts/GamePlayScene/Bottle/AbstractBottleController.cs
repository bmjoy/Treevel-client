using Project.Scripts.GameDatas;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class AbstractBottleController : MonoBehaviour
    {
        /// <summary>
        /// ボトルのId (初期位置と同じ)
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 自身が壊されたかどうか
        /// </summary>
        internal protected bool IsDead
        {
            get;
            internal set;
        }

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable
        {
            get {
                return _getDamagedHandler != null;
            }
        }

        /// <summary>
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        protected IBottleGetDamagedHandler _getDamagedHandler;

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        protected IEnterTileHandler _enterTileHandler;

        /// <summary>
        /// ボトルの成功判定と成功時の挙動
        /// </summary>
        protected IBottleSuccessHandler _successHandler;

        protected virtual void Awake() {}

        /// <summary>
        /// 衝突イベントを処理する
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，ボトル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(TagName.BULLET)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (other.gameObject.transform.position.z < 0) return;

            _getDamagedHandler?.OnGetDamaged(other.gameObject);
        }

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        /// <param name="targetTile">目標のタイル</param>
        public void OnEnterTile(GameObject targetTile)
        {
            _enterTileHandler?.OnEnterTile(targetTile);
        }

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
            Id = bottleData.initPos;

            // ボトルをボードに設定
            BoardManager.SetBottle(this, Id);

            if (bottleData.bottleSprite != null) {
                var bottleSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.bottleSprite);
                GetComponent<SpriteRenderer>().sprite = bottleSprite;
            }

            InitializeSprite();
            GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// ボトルの成功判定
        /// </summary>
        public bool IsSuccess()
        {
            // SuccessHandler未定義の時は成功とみなす。
            return _successHandler?.IsSuccess() ?? true;
        }

        /// <summary>
        /// 成功判定があるかどうか
        /// </summary>
        public bool HasSuccessHandler()
        {
            return _successHandler != null;
        }
    }
}

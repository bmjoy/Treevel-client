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
        /// ボトルのId (初期位置を擬似的に利用)
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 自身が死んだかどうか
        /// </summary>
        protected internal bool IsDead
        {
            get;
            internal set;
        }

        /// <summary>
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        protected IBottleGetDamagedHandler getDamagedHandler;

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        protected IBottleEnterTileHandler bottleEnterTileHandler;

        /// <summary>
        /// ボトルの成功判定と成功時の挙動
        /// </summary>
        protected IBottleSuccessHandler successHandler;

        /// <summary>
        /// ボトルを勝手に移動させる時の挙動
        /// </summary>
        protected ISelfishHandler selfishHandler;

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable => getDamagedHandler != null;

        /// <summary>
        /// 無敵状態かどうか
        /// </summary>
        public bool Invincible = false;

        protected virtual void Awake() {}

        private void FixedUpdate()
        {
            selfishHandler?.DoWhenFixedUpdate();
        }

        /// <summary>
        /// ゲーム開始時の挙動
        /// </summary>
        public void OnStartGame()
        {
            selfishHandler?.OnStartGame();
        }

        /// <summary>
        /// 衝突イベントを処理する
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，ボトル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(TagName.GIMMICK)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (other.gameObject.transform.position.z < 0) return;
            // 無敵状態なら，衝突を考えない
            if (Invincible) return;

            getDamagedHandler?.OnGetDamaged(other.gameObject);
        }

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        /// <param name="targetTile">目標のタイル</param>
        public void OnEnterTile(GameObject targetTile)
        {
            bottleEnterTileHandler?.OnEnterTile(targetTile);
        }

        /// <summary>
        /// タイルから出た時の挙動
        /// </summary>
        /// <param name="targetTile">出たタイル</param>
        public void OnExitTile(GameObject targetTile)
        {
            bottleEnterTileHandler?.OnExitTile(targetTile);
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
            BoardManager.Instance.InitializeBottle(this, Id);

            if (bottleData.bottleSprite.RuntimeKeyIsValid()) {
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
            return successHandler?.IsSuccess() ?? true;
        }
    }
}

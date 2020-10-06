using System;
using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(ObjectUnifier))]
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
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        public event Action<GameObject> HandleOnGetDamaged;

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        protected event Action<GameObject> HandleOnEnterTile;

        /// <summary>
        /// タイルから出る時の挙動
        /// </summary>
        protected event Action<GameObject> HandleOnExitTile;

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable => HandleOnGetDamaged != null;

        /// <summary>
        /// 無敵状態かどうか
        /// </summary>
        public bool Invincible = false;

        protected virtual void Awake()
        {
            Debug.Assert(GetComponent<SpriteRenderer>().sortingLayerName == SortingLayerName.BOTTLE, $"Sorting Layer Name should be {SortingLayerName.BOTTLE}");
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

            HandleOnGetDamaged?.Invoke(other.gameObject);
        }

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        /// <param name="targetTile">目標のタイル</param>
        public virtual void OnEnterTile(GameObject targetTile)
        {
            HandleOnEnterTile?.Invoke(targetTile);
        }

        /// <summary>
        /// タイルから出た時の挙動
        /// </summary>
        /// <param name="targetTile">出たタイル</param>
        public virtual void OnExitTile(GameObject targetTile)
        {
            HandleOnExitTile?.Invoke(targetTile);
        }

        private IEnumerator InitializeSprite(AssetReferenceSprite spriteAsset)
        {
            // 無限ループを防ぐためにタイムアウトを設ける
            const float timeOut = 2f;
            var elapsedTime = 0f;
            while (true) {
                if (elapsedTime >= timeOut) {
                    UIManager.Instance.ShowErrorMessage(EErrorCode.LoadDataError);
                    throw new System.ArgumentNullException("ボトル画像の読み込みが失敗しました");
                }

                // 経過時間計算
                elapsedTime += Time.deltaTime;
                var bottleSprite = AddressableAssetManager.GetAsset<Sprite>(spriteAsset);
                if (bottleSprite == null)
                    yield return new WaitForEndOfFrame();
                else {
                    GetComponent<SpriteRenderer>().sprite = bottleSprite;
                    break;
                }
            }

            // ボトル画像のサイズを取得
            var bottleWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            var bottleHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            // ボトルの初期設定
            transform.localScale = new Vector2(BottleSize.WIDTH / bottleWidth, BottleSize.HEIGHT / bottleHeight);

            if (GetComponent<Collider2D>() is BoxCollider2D) {
                GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            }
            GetComponent<Renderer>().sortingLayerName = SortingLayerName.BOTTLE;
            GetComponent<SpriteRenderer>().enabled = true;
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

            // sprite が設定されている場合読み込む
            if (bottleData.bottleSprite.RuntimeKeyIsValid()) {
                StartCoroutine(InitializeSprite(bottleData.bottleSprite));
            } else {
                GetComponent<ObjectUnifier>().Unify();
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        /// <summary>
        /// ボトルの成功判定
        /// </summary>
        public virtual bool IsSuccess()
        {
            // 未定義の時は成功とみなす
            return true;
        }
    }
}

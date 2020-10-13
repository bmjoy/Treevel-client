using System;
using System.Collections;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Modules.GamePlayScene.Bottle
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
        public event Action<GameObject> OnGetDamaged;

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        protected event Action<GameObject> onEnterTile;

        /// <summary>
        /// タイルから出る時の挙動
        /// </summary>
        protected event Action<GameObject> onExitTile;

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable => OnGetDamaged != null;

        /// <summary>
        /// 無敵状態かどうか
        /// </summary>
        public bool Invincible = false;

        protected virtual void Awake()
        {
            Debug.Assert(GetComponent<SpriteRenderer>().sortingLayerName == Constants.SortingLayerName.BOTTLE, $"Sorting Layer Name should be {Constants.SortingLayerName.BOTTLE}");
        }

        /// <summary>
        /// 衝突イベントを処理する
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，ボトル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(Constants.TagName.GIMMICK)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (other.gameObject.transform.position.z < 0) return;
            // 無敵状態なら，衝突を考えない
            if (Invincible) return;

            OnGetDamaged?.Invoke(other.gameObject);
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
            transform.localScale = new Vector2(Constants.BottleSize.WIDTH / bottleWidth, Constants.BottleSize.HEIGHT / bottleHeight);

            if (GetComponent<Collider2D>() is BoxCollider2D) {
                GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            }
            GetComponent<Renderer>().sortingLayerName = Constants.SortingLayerName.BOTTLE;
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
        /// タイルに移動した時のイベント発火
        /// </summary>
        /// <param name="targetTile">目標の出たタイル</param>
        public void TriggerOnEnterTile(GameObject targetTile)
        {
            onEnterTile?.Invoke(targetTile);
        }

        /// <summary>
        /// タイルから出た時のイベント発火
        /// </summary>
        /// <param name="targetTile">出たタイル</param>
        public void TriggerOnExitTile(GameObject targetTile)
        {
            onExitTile?.Invoke(targetTile);
        }
    }
}

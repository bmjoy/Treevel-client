using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(GameSpriteRendererUnifier))]
    public abstract class AbstractBottleController : AbstractGameObjectController
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
        /// ボトルタイプ
        /// </summary>
        private EBottleType bottleType;

        /// <summary>
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        public IObservable<GameObject> GetDamaged => _getDamagedSubject;
        private readonly Subject<GameObject> _getDamagedSubject = new Subject<GameObject>();

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        public IObservable<GameObject> EnterTile => _enterTileSubject;
        private readonly Subject<GameObject> _enterTileSubject = new Subject<GameObject>();

        /// <summary>
        /// タイルから出る時の挙動
        /// </summary>
        public IObservable<GameObject> ExitTile => _exitTileSubject;
        private readonly Subject<GameObject> _exitTileSubject = new Subject<GameObject>();

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable => bottleType.IsAttackable();

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

            _getDamagedSubject.OnNext(other.gameObject);
        }

        private async UniTask InitializeSprite(AssetReferenceSprite spriteAsset)
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
                    await UniTask.Yield();
                else {
                    GetComponent<SpriteRenderer>().sprite = bottleSprite;
                    break;
                }
            }

            // ボトル画像のサイズを調整
            GetComponent<GameSpriteRendererUnifier>().Unify();
            GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData"> ボトルのデータ </param>
        public virtual async UniTask Initialize(BottleData bottleData)
        {
            Id = bottleData.initPos;
            bottleType = bottleData.type;

            // ボトルをボードに設定
            BoardManager.Instance.InitializeBottle(this, Id);

            // sprite が設定されている場合読み込む
            if (bottleData.bottleSprite.RuntimeKeyIsValid()) {
                await InitializeSprite(bottleData.bottleSprite);
            } else {
                GetComponent<SpriteRendererUnifier>().Unify();
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        /// <summary>
        /// タイルに移動した時のイベント発火
        /// </summary>
        /// <param name="targetTile">目標の出たタイル</param>
        public void OnEnterTile(GameObject targetTile)
        {
            _enterTileSubject.OnNext(targetTile);
        }

        /// <summary>
        /// タイルから出た時のイベント発火
        /// </summary>
        /// <param name="targetTile">出たタイル</param>
        public void OnExitTile(GameObject targetTile)
        {
            _exitTileSubject.OnNext(targetTile);
        }
    }
}

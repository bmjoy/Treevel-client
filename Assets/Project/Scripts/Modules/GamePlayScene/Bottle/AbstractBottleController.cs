using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(GameSpriteRendererUnifier))]
    public abstract class AbstractBottleController : AbstractGameObjectController
    {
        /// <summary>
        /// 無敵状態かどうか
        /// </summary>
        public bool Invincible;

        private readonly Subject<GameObject> _enterTileSubject = new Subject<GameObject>();

        private readonly Subject<GameObject> _exitTileSubject = new Subject<GameObject>();

        private readonly Subject<GameObject> _getDamagedSubject = new Subject<GameObject>();

        /// <summary>
        /// ボトルタイプ
        /// </summary>
        private EBottleType bottleType;

        /// <summary>
        /// ボトルのId (初期位置を擬似的に利用)
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        public IObservable<GameObject> GetDamaged => _getDamagedSubject;

        /// <summary>
        /// タイルに移動した時の挙動
        /// </summary>
        public IObservable<GameObject> EnterTile => _enterTileSubject;

        /// <summary>
        /// タイルから出る時の挙動
        /// </summary>
        public IObservable<GameObject> ExitTile => _exitTileSubject;

        /// <summary>
        /// 攻撃対象かどうか
        /// </summary>
        public bool IsAttackable => bottleType.IsAttackable();

        protected virtual void Awake()
        {
            Debug.Assert(GetComponent<SpriteRenderer>().sortingLayerName == Constants.SortingLayerName.BOTTLE,
                         $"Sorting Layer Name should be {Constants.SortingLayerName.BOTTLE}");
            // 衝突イベントを処理する
            this.OnTriggerEnter2DAsObservable()
                .Where(other => other.gameObject.CompareTag(Constants.TagName.GIMMICK))
                .Where(other => other.gameObject.transform.position.z >= 0)
                .Where(_ => !Invincible)
                .Subscribe(other => _getDamagedSubject.OnNext(other.gameObject)).AddTo(this);
        }

        private async UniTask InitializeSprite(AssetReferenceSprite spriteAsset)
        {
            // 無限ループを防ぐためにタイムアウトを設ける
            const float timeOut = 2f;
            var elapsedTime = 0f;
            while (true) {
                if (elapsedTime >= timeOut) {
                    UIManager.Instance.ShowErrorMessage(EErrorCode.LoadDataError);
                    throw new ArgumentNullException("ボトル画像の読み込みが失敗しました");
                }

                // 経過時間計算
                elapsedTime += Time.deltaTime;
                var bottleSprite = AddressableAssetManager.GetAsset<Sprite>(spriteAsset);
                if (bottleSprite == null) {
                    await UniTask.Yield();
                } else {
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

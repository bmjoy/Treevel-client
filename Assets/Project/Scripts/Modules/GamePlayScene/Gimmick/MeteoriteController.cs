using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(GameSpriteRendererUnifier))]
    [RequireComponent(typeof(Collider2D))]
    public class MeteoriteController : AbstractGimmickController
    {
        /// <summary>
        /// 目標座標
        /// </summary>
        private Vector2 _targetPos;

        /// <summary>
        /// 警告のプレハブ
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _warningPrefab;

        /// <summary>
        /// 速さ
        /// </summary>
        [SerializeField] private float _speed = 0.05f;

        /// <summary>
        /// 警告オブジェクトインスタンス（ゲーム終了時片付けするためにメンバー変数として持つ）
        /// </summary>
        private GameObject _warningObj;

        /// <summary>
        /// 隕石の画像の表示時間
        /// </summary>
        [SerializeField] private float _meteoriteDisplayTime = 1.5f;

        /// <summary>
        /// 隕石本体移動させる用フラグ
        /// </summary>
        private bool _isMoving = false;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
            .Where(_ => _warningObj != null)
            .Subscribe(_ => {
                _warningPrefab.ReleaseInstance(_warningObj);
            }).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            switch (gimmickData.type) {
                case EGimmickType.Meteorite:
                    if (gimmickData.isRandom) {
                        var row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray());
                        var column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray());
                        _targetPos = BoardManager.Instance.GetTilePos(column, row);
                    } else {
                        _targetPos = BoardManager.Instance.GetTilePos((int)gimmickData.targetColumn, (int)gimmickData.targetRow);
                    }
                    break;
                case EGimmickType.AimingMeteorite:
                    if (gimmickData.isRandom) {
                        // 乱数インデックスを重みに基づいて取得
                        var randomIndex = GimmickLibrary.SamplingArrayIndex(gimmickData.randomAttackableBottles.ToArray());
                        // 乱数インデックスをボトルIDに変換
                        var targetId = CalcBottleIdByRandomArrayIndex(randomIndex);
                        _targetPos = BoardManager.Instance.GetBottlePosById(targetId);
                    } else {
                        _targetPos = BoardManager.Instance.GetBottlePosById(gimmickData.targetBottle);
                    }
                    break;
                default:
                    throw new System.NotImplementedException("不正なギミックタイプです");
            }
        }

        private void FixedUpdate()
        {
            if (!_isMoving || GamePlayDirector.Instance.State != GamePlayDirector.EGameState.Playing)
                return;

            // 奥方向に移動させる（見た目変化ない）
            transform.Translate(Vector3.back * _speed, Space.World);

            // 透明度をあげてだんだんと見えなくする
            var deltaAlpha = Time.fixedDeltaTime / _meteoriteDisplayTime;
            _renderer.color -= new Color(0, 0, 0, deltaAlpha);

            if (transform.position.z < -1 * (_meteoriteDisplayTime / Time.fixedDeltaTime) * _speed) {
                Destroy(gameObject);
            }
        }

        public override async UniTask Trigger(CancellationToken token)
        {
            try
            {
                await ShowWarning(token, _targetPos, _warningDisplayTime);
                if (token.IsCancellationRequested) return;

                transform.position = new Vector3(_targetPos.x, _targetPos.y, _speed);
                GetComponent<Collider2D>().enabled = true;
                GetComponent<SpriteRenderer>().enabled = true;
                _isMoving = true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 警告表示
        /// </summary>
        /// <param name="warningPos">表示する座標</param>
        /// <param name="displayTime">表示時間</param>
        private async UniTask ShowWarning(CancellationToken token, Vector2 warningPos, float displayTime)
        {
            try
            {
                if (_warningObj != null)
                {
                    _warningPrefab.ReleaseInstance(_warningObj);
                }

                _warningObj = await _warningPrefab.InstantiateAsync(warningPos, Quaternion.identity).ToUniTask();

                _warningObj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                _warningObj.GetComponent<SpriteRenderer>().enabled = true;

                // 警告終わるまで待つ
                while ((displayTime -= Time.fixedDeltaTime) >= 0) await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);

                if (_warningObj != null)
                {
                    _warningPrefab.ReleaseInstance(_warningObj);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            // 隕石が出現したフレーム以外では衝突を考えない
            if (transform.position.z < 0) return;

            // ボトルとの衝突
            var bottle = other.GetComponent<AbstractBottleController>();
            if (bottle != null && bottle.IsAttackable && !bottle.Invincible) {
                // 数字ボトルとの衝突
                // 衝突したオブジェクトは赤色に変える
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;

                gameObject.GetComponent<Renderer>().sortingLayerName = Constants.SortingLayerName.GIMMICK;

                // 隕石を衝突したボトルに追従させる
                gameObject.transform.SetParent(other.transform);
            }
        }

        /// <summary>
        /// 乱数配列のインデックスをボトルのIdに変換する
        /// </summary>
        /// <param name="index">_randomAttackableBottlesから取ったインデックス</param>
        /// <returns>ボトルのID</returns>
        private static int CalcBottleIdByRandomArrayIndex(int index)
        {
            var bottles = BottleLibrary.OrderedAttackableBottles;
            var bottleAtIndex = bottles.ElementAt(index);
            return bottleAtIndex.Id;
        }
    }
}

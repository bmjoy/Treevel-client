using System.Collections;
using System.Linq;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(ObjectUnifier))]
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

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            switch (gimmickData.type) {
                case EGimmickType.Meteorite:
                    _targetPos = BoardManager.Instance.GetTilePos((int)gimmickData.targetColumn - 1, (int)gimmickData.targetRow - 1);
                    break;
                case EGimmickType.AimingMeteorite:
                    _targetPos = BoardManager.Instance.GetBottlePosById(gimmickData.targetBottle).Value;
                    break;
                case EGimmickType.RandomMeteorite:
                    var row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray()) + 1;
                    var column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray()) + 1;
                    _targetPos = BoardManager.Instance.GetTilePos(column - 1, row - 1);
                    break;
                case EGimmickType.RandomAimingMeteorite:
                    // 乱数インデックスを重みに基づいて取得
                    var randomIndex = GimmickLibrary.SamplingArrayIndex(gimmickData.randomAttackableBottles.ToArray());
                    // 乱数インデックスをボトルIDに変換
                    var targetId = CalcBottleIdByRandomArrayIndex(randomIndex);
                    _targetPos = BoardManager.Instance.GetBottlePosById(targetId).Value;
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

        public override IEnumerator Trigger()
        {
            yield return ShowWarning(_targetPos, _warningDisplayTime);

            transform.position = new Vector3(_targetPos.x, _targetPos.y, _speed);
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            _isMoving = true;
        }

        /// <summary>
        /// 警告表示
        /// </summary>
        /// <param name="warningPos">表示する座標</param>
        /// <param name="displayTime">表示時間</param>
        private IEnumerator ShowWarning(Vector2 warningPos, float displayTime)
        {
            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
            }

            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _warningPrefab.InstantiateAsync(warningPos, Quaternion.identity);

            _warningObj = warningOp.Result;
            _warningObj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            _warningObj.GetComponent<SpriteRenderer>().enabled = true;

            // 警告終わるまで待つ
            while ((displayTime -= Time.fixedDeltaTime) >= 0) yield return new WaitForFixedUpdate();

            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
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

                gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.GIMMICK;

                // 隕石を衝突したボトルに追従させる
                gameObject.transform.SetParent(other.transform);
            }
        }

        protected override void OnEndGame()
        {
            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
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

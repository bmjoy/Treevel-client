using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
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
        /// 目標行
        /// </summary>
        private ERow _targetRow;

        /// <summary>
        /// 目標列
        /// </summary>
        private EColumn _targetColumn;

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
        private bool startMoveFlag = false;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _targetRow = gimmickData.targetRow;
            _targetColumn = gimmickData.targetColumn;
        }

        private void FixedUpdate()
        {
            if (!startMoveFlag || GamePlayDirector.Instance.State != GamePlayDirector.EGameState.Playing)
                return;

            // 奥方向に移動させる（見た目変化ない）
            transform.Translate(Vector3.back * _speed, Space.World);

            // 透明度をあげてだんだんと見えなくする
            var deltaAlpha = Time.fixedDeltaTime / _meteoriteDisplayTime;
            _renderer.color -= new Color(0, 0, 0, deltaAlpha);

            if (transform.position.z < -1 * (_meteoriteDisplayTime / Time.fixedDeltaTime) * _speed) {
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }

        public override IEnumerator Trigger()
        {
            Vector2 targetPos = BoardManager.Instance.GetTilePos((int)_targetColumn - 1, (int)_targetRow - 1);
            yield return ShowWarning(targetPos, _warningDisplayTime);

            transform.position = new Vector3(targetPos.x, targetPos.y, _speed);
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            startMoveFlag = true;
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
            }
        }
    }
}

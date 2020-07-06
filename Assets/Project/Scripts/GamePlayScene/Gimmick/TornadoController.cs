using System;
using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public class TornadoController : AbstractGimmickController
    {
        /// <summary>
        /// 竜巻の移動速度
        /// </summary>
        [SerializeField] private float _speed = 3.0f;

        /// <summary>
        /// 警告のプレハブ
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _warningPrefab;

        /// <summary>
        /// 銃弾の移動方向
        /// </summary>
        private ECartridgeDirection _direction = ECartridgeDirection.Random;

        /// <summary>
        /// 攻撃する行／列
        /// </summary>
        private int _line;

        private Rigidbody2D _rigidBody;

        private GameObject _warningObj;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);
            _direction = gimmickData.direction;
            _line = gimmickData.line;

            SetInitialPosition(_direction, _line);
        }

        public override IEnumerator Trigger()
        {
            yield return ShowWarning();

            // ギミック発動
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            SetDirection(_direction);
        }


        /// <summary>
        /// 警告の表示
        /// </summary>
        protected IEnumerator ShowWarning()
        {
            Vector2 bulletMotionVector;
            Vector2 warningPosition;
            switch (_direction) {
                case ECartridgeDirection.ToLeft:
                    warningPosition = new Vector2(WindowSize.WIDTH / 2,
                        TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - _line));
                    bulletMotionVector = Vector2.left;
                    break;
                case ECartridgeDirection.ToRight:
                    warningPosition = new Vector2(-WindowSize.WIDTH / 2,
                        TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - _line));
                    bulletMotionVector = Vector2.right;
                    break;
                case ECartridgeDirection.ToUp:
                    warningPosition = new Vector2(TileSize.WIDTH * (_line - (StageSize.COLUMN / 2 + 1)),
                        -WindowSize.HEIGHT / 2);
                    bulletMotionVector = Vector2.up;
                    break;
                case ECartridgeDirection.ToBottom:
                    warningPosition = new Vector2(TileSize.WIDTH * (_line - (StageSize.COLUMN / 2 + 1)),
                        WindowSize.HEIGHT / 2);
                    bulletMotionVector = Vector2.down;
                    break;
                case ECartridgeDirection.Random:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            warningPosition += Vector2.Scale(bulletMotionVector, new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y)) / 2;
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _warningPrefab.InstantiateAsync(warningPosition, Quaternion.identity);

            _warningObj = warningOp.Result;

            // 警告終わるまで待つ
            yield return new WaitForSeconds(_warningDisplayTime);

            // 警告オブジェクト破壊
            _warningPrefab.ReleaseInstance(_warningObj);
        }

        /// <summary>
        /// 移動方向の設定
        /// </summary>
        /// <param name="direction">移動方向</param>
        private void SetDirection(ECartridgeDirection direction)
        {
            switch (direction) {
                case ECartridgeDirection.ToUp:
                    _rigidBody.velocity = Vector2.up * _speed;
                    break;
                case ECartridgeDirection.ToLeft:
                    _rigidBody.velocity = Vector2.left * _speed;
                    break;
                case ECartridgeDirection.ToRight:
                    _rigidBody.velocity = Vector2.right * _speed;
                    break;
                case ECartridgeDirection.ToBottom:
                    _rigidBody.velocity = Vector2.down * _speed;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }


        /// <summary>
        /// 初期位置設定
        /// </summary>
        /// <param name="direction">初期の移動方向</param>
        /// <param name="line">攻撃する行（1~5）／列（1~3）</param>
        private void SetInitialPosition(ECartridgeDirection direction, int line)
        {
            switch (direction) {
                case ECartridgeDirection.ToLeft:
                case ECartridgeDirection.ToRight: {
                        float x, y;
                        // 目標列の一番右端のタイルのY座標を取得
                        var tileNum = line * StageSize.COLUMN;
                        y = BoardManager.Instance.GetTilePos(tileNum).y;

                        if (direction == ECartridgeDirection.ToLeft) {
                            x = (WindowSize.WIDTH + CartridgeSize.WIDTH) / 2;
                        } else {
                            x = -(WindowSize.WIDTH + CartridgeSize.WIDTH) / 2;
                        }

                        transform.position = new Vector2(x, y);
                        break;
                    }
                case ECartridgeDirection.ToUp:
                case ECartridgeDirection.ToBottom: {
                        float x, y;
                        // 目標行の一列目のタイルのx座標を取得
                        var tileNum = line;
                        x = BoardManager.Instance.GetTilePos(tileNum).x;

                        if (direction == ECartridgeDirection.ToUp) {
                            y = -(WindowSize.HEIGHT + CartridgeSize.HEIGHT) / 2;
                        } else {
                            y = (WindowSize.HEIGHT + CartridgeSize.HEIGHT) / 2;
                        }

                        transform.position = new Vector2(x, y);
                        break;
                    }
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            // 数字ボトルとの衝突以外は考えない
            var bottle = other.GetComponent<AbstractBottleController>();
            if (bottle == null || !bottle.IsAttackable || bottle.Invincible) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        protected override void GameEnd()
        {
            _rigidBody.velocity = Vector2.zero;
            _warningPrefab.ReleaseInstance(_warningObj);
        }
    }
}

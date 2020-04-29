using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Controllers
{
    public class NormalCartridgeController : BulletController
    {
        /// <summary>
        /// 銃弾が存在することを許す画面外の余幅
        /// </summary>
        [NonSerialized] private const float _ADDITIONAL_MARGIN = 0.00001f;

        /// <summary>
        /// 銃弾の移動ベクトル
        /// </summary>
        [NonSerialized] protected Vector2 motionVector;

        protected override void Awake()
        {
            base.Awake();
            // 銃弾の先頭部分のみに当たり判定を与える
            const float betweenBottles = TileSize.WIDTH - BottleSize.WIDTH;
            var collider =  gameObject.GetComponent<Collider2D>();
            collider.offset = new Vector2(-(originalWidth - betweenBottles) / 2, 0);
            if (collider is BoxCollider2D)
                ((BoxCollider2D)collider).size = new Vector2(betweenBottles, originalHeight);
        }

        /// <summary>
        /// 銃弾の移動ベクトル、初期座標を設定する
        /// </summary>
        /// <param name="direction"> 移動方向</param>
        /// <param name="line"> 移動する行(または列) </param>
        /// <param name="motionVector"> 移動ベクトル </param>
        public void Initialize(ECartridgeDirection direction, int line, Vector2 motionVector)
        {
            this.motionVector = motionVector;
            SetInitialPosition(direction, line);
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
        }

        protected void Update()
        {
            // Check if bullet goes out of window
            if (transform.position.x <
                -((WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + _ADDITIONAL_MARGIN) ||
                transform.position.x > (WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + _ADDITIONAL_MARGIN ||
                transform.position.y <
                -((WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + _ADDITIONAL_MARGIN) ||
                transform.position.y > (WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + _ADDITIONAL_MARGIN)
                Destroy(gameObject);
        }

        protected virtual void FixedUpdate()
        {
            transform.Translate(motionVector * speed, Space.World);
        }

        /// <summary>
        /// 銃弾の初期座標を設定する
        /// </summary>
        /// <param name="direction"> 移動方向 </param>
        /// <param name="line"> 移動する行(または列)</param>
        private void SetInitialPosition(ECartridgeDirection direction, int line)
        {
            // 移動方向の初期座標
            var motionVectorPosition = new Vector2(-(WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2,
                -(WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2) * motionVector;
            // 移動方向に垂直な方向の初期座標
            var orthogonalMotionVector = motionVector.Transposition().Abs();
            var orthogonalMotionVectorPosition = new Vector2(TileSize.WIDTH * (line - (StageSize.COLUMN / 2 + 1)),
                TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - line)) * orthogonalMotionVector;
            // 銃弾の座標の初期位置
            transform.position = motionVectorPosition + orthogonalMotionVectorPosition;
            // 銃弾画像の向きを反転させるかどうか
            transform.localScale = new Vector2(CartridgeSize.WIDTH / originalWidth,
                -1 * Mathf.Sign(motionVector.x) * CartridgeSize.HEIGHT / originalHeight) *
            LOCAL_SCALE;
            // 銃弾画像を何度回転させるか
            var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
            angle = (float)(Mathf.Acos(angle) * 180 / Math.PI);
            angle *= -1 * Mathf.Sign(motionVector.y);
            transform.Rotate(new Vector3(0, 0, angle), Space.World);

        }

        protected override void OnFail()
        {
            speed = 0;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // 数字ボトルとの衝突以外は考えない
            if (!other.gameObject.CompareTag(TagName.NUMBER_BOTTLE)) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}

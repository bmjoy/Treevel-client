using UnityEngine;
using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class NormalCartridgeController : BulletController
    {
        public float additionalMargin = 0.00001f;
        public Vector2 motionVector;
        [NonSerialized] protected float speed = 0.05f;

        protected override void Awake()
        {
            base.Awake();
            // BocCollider2Dのアタッチメント
            gameObject.AddComponent<BoxCollider2D>();
            // 銃弾の先頭部分のみに当たり判定を与える
            const float betweenPanels = TileSize.WIDTH - PanelSize.WIDTH;
            gameObject.GetComponent<BoxCollider2D>().offset =
                new Vector2(-(originalWidth - betweenPanels) / 2, 0);
            gameObject.GetComponent<BoxCollider2D>().size =
                new Vector2(betweenPanels, originalHeight);
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            // RigidBodyのアタッチメント
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
        }

        public void Initialize(ECartridgeDirection direction, int line, Vector2 motionVector)
        {
            this.motionVector = motionVector;
            SetInitialPosition(direction, line);
        }

        protected void Update()
        {
            // Check if bullet goes out of window
            if (transform.position.x <
                -((WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + additionalMargin) ||
                transform.position.x > (WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + additionalMargin ||
                transform.position.y <
                -((WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + additionalMargin) ||
                transform.position.y > (WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2 + additionalMargin)
                Destroy(gameObject);
        }

        protected virtual void FixedUpdate()
        {
            transform.Translate(motionVector * speed, Space.World);
        }

        // 銃弾の初期配置の設定
        private void SetInitialPosition(ECartridgeDirection direction, int line)
        {
            // 移動方向に関わる座標
            var motionVectorPosition = new Vector2(-(WindowSize.WIDTH + CartridgeSize.WIDTH * LOCAL_SCALE) / 2,
                -(WindowSize.HEIGHT + CartridgeSize.WIDTH * LOCAL_SCALE) / 2) * motionVector;
            // 移動方向に垂直な方向の座標
            var orthogonalMotionVector = motionVector.Transposition().Abs();
            var orthogonalMotionVectorPosition = new Vector2(TileSize.WIDTH * (line - 2),
                WindowSize.HEIGHT * 0.5f -
                (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
                TileSize.HEIGHT * (line - 1)) * orthogonalMotionVector;
            // 銃弾の座標の初期位置
            transform.position = motionVectorPosition + orthogonalMotionVectorPosition;
            // Check if a bullet should be flipped vertically
            transform.localScale = new Vector2(CartridgeSize.WIDTH / originalWidth,
                -1 * Mathf.Sign(motionVector.x) * CartridgeSize.HEIGHT / originalHeight) *
            LOCAL_SCALE;
            // Calculate rotation angle
            var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
            angle = (float)(Mathf.Acos(angle) * 180 / Math.PI);
            angle *= -1 * Mathf.Sign(motionVector.y);
            transform.Rotate(new Vector3(0, 0, angle), Space.World);
        }

        protected override void OnFail()
        {
            speed = 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 数字パネルとの衝突以外は考えない
            if (!other.gameObject.CompareTag(TagName.NUMBER_PANEL)) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}

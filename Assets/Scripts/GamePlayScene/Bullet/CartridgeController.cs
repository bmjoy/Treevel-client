using System;
using UnityEngine;

namespace GamePlayScene.Bullet
{
    public abstract class CartridgeController : BulletController
    {
        public float additionalMargin = 0.00001f;
        public float localScale;
        public Vector2 motionVector;
        public float speed;

        // 元画像のサイズ
        public float originalWidth;
        public float originalHeight;


        protected void Update()
        {
            // Check if bullet goes out of window
            if (transform.position.x < -((WindowSize.WIDTH + localScale * originalWidth) / 2 + additionalMargin) ||
                transform.position.x > (WindowSize.WIDTH + localScale * originalWidth) / 2 + additionalMargin ||
                transform.position.y < -((WindowSize.HEIGHT + localScale * originalHeight) / 2 + additionalMargin) ||
                transform.position.y > (WindowSize.HEIGHT + localScale * originalHeight) / 2 + additionalMargin)
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        protected void FixedUpdate()
        {
            transform.Translate(motionVector * speed, Space.World);
        }

        // コンストラクタがわりのメソッド
        protected void Initialize(Vector2 motionVector)
        {
            this.motionVector = motionVector;
            // Calculate rotation angle
            var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
            angle = (float) (Mathf.Acos(angle) * 180 / Math.PI);
            angle *= -1 * Mathf.Sign(motionVector.y);

            transform.Rotate(new Vector3(0, 0, angle), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // パネルとの衝突以外は考えない
            if (!other.gameObject.CompareTag("Panel")) return;
            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        private void OnFail()
        {
            speed = 0;
        }

        private void OnSucceed()
        {
            Destroy(gameObject);
        }
    }
}

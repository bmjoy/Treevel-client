using System;
using Directors;
using UnityEngine;

namespace Bullet
{
    public abstract class CartridgeController : BulletController
    {
        public Vector2 motionVector;
        protected float speed;
        public float localScale;

        protected override void Update()
        {
            // Check if bullet goes out of window
            if (this.transform.position.x < -(WindowSize.WIDTH + localScale) / 2 ||
                this.transform.position.x > (WindowSize.WIDTH + localScale) / 2 ||
                this.transform.position.y < -(WindowSize.HEIGHT + localScale) / 2 ||
                this.transform.position.y > (WindowSize.HEIGHT + localScale) / 2)
            {
                Destroy(gameObject);
            }
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

        protected override void FixedUpdate()
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
            angle *= (-1) * Mathf.Sign(motionVector.y);

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

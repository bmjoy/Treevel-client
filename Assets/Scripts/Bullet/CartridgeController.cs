using System;
using Directors;
using UnityEngine;

namespace Bullet
{
    public class CartridgeController : BulletController
    {
        private Vector2 motionVector;
        protected float speed;
        protected float width;
        protected float height;

        // Update is called once per frame
        protected override void Update()
        {
            // Check if bullet goes out of window
            if (this.transform.position.x < -WindowSize.WIDTH - width / 2 ||
                this.transform.position.x > WindowSize.WIDTH + width / 2 ||
                this.transform.position.y < -WindowSize.HEIGHT - height / 2 ||
                this.transform.position.y > WindowSize.HEIGHT + height / 2)
            {
                Destroy(gameObject);
            }

            if (gamePlayDirector.currentState == GamePlayDirector.GameState.Failure)
            {
                speed = 0;
            }
        }

        protected override void FixedUpdate()
        {
            transform.Translate(motionVector * speed, Space.World);
        }

        // コンストラクタがわりのメソッド
        public virtual void Initialize(Vector2 motionVector)
        {
            this.motionVector = motionVector;
            // Calculate rotation angle
            var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
            angle = (float) (Mathf.Acos(angle) * 180 / Math.PI);
            angle *= (-1) * Mathf.Sign(motionVector.y);

            // Check if a bullet should be flipped vertically
            if (motionVector.x > 0)
            {
                var tempLocalScale = transform.localScale;
                tempLocalScale.y *= (-1);
                transform.localScale = tempLocalScale;
            }

            transform.Rotate(new Vector3(0, 0, angle), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // パネルとの衝突以外は考えない
            if (!other.gameObject.CompareTag("Panel")) return;
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}

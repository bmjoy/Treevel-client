using UnityEngine;

namespace Bullet
{
    public class CartridgeController : BulletController
    {
        private Vector2 motionVector;
        protected float speed;
        private float width;
        private float height;

        // Use this for initialization
        protected override void Start()
        {
        }

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
        }

        protected override void FixedUpdate()
        {
            transform.Translate(motionVector * speed);
        }

        // コンストラクタがわりのメソッド
        public virtual void Initialize(Vector2 motionVector)
        {
            this.motionVector = motionVector;
            width = (float) (WindowSize.WIDTH * 0.15);
            height = width;
            this.transform.localScale = new Vector3(width, height, 0);
        }
    }
}

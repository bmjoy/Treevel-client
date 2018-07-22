using UnityEngine;

namespace Bullet
{
    public class CartridgeController : BulletController
    {
        private Vector2 motionVector;
        protected float speed;

        // Use this for initialization
        protected override void Start()
        {
        }

        // Update is called once per frame
        protected override void Update()
        {
        }

        protected override void FixedUpdate()
        {
            transform.Translate(motionVector * speed);
        }

        // コンストラクタがわりのメソッド
        public virtual void Initialize(Vector2 motionVector)
        {
            this.motionVector = motionVector;
        }
    }
}

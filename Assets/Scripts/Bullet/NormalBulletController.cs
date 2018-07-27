using UnityEngine;

namespace Bullet
{
    public class NormalBulletController : CartridgeController
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        // コンストラクタがわりのメソッド
        public override void Initialize(Vector2 motionVector)
        {
            base.Initialize(motionVector);
            this.speed = 0.1f;
            width = (float) (WindowSize.WIDTH * 0.15);
            height = width;
            this.transform.localScale = new Vector3(width, height, 0);
        }
    }
}

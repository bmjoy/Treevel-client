using UnityEngine;

namespace Bullet
{
    public class NormalBulletController : CartridgeController
    {
        // コンストラクタがわりのメソッド
        public override void Initialize(Vector2 motionVector)
        {
            base.Initialize(motionVector);
            this.speed = 0.1f;
            width = (float) (WindowSize.WIDTH * 0.15);
            height = width;
            this.transform.localScale *= new Vector2(width, height);
        }
    }
}

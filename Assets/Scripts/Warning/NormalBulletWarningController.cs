using UnityEngine;

namespace Warning
{
    public class NormalBulletWarningController : CartridgeWarningController
    {
        public override void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale)
        {
            localScale = (float) (WindowSize.WIDTH * 0.15);
            transform.localScale *= new Vector2(localScale, localScale);
            transform.position = bulletPosition + Vector2.Scale(bulletMotionVector,
                                     new Vector2((bulletLocalScale + localScale) / 2,
                                         (bulletLocalScale + localScale) / 2));
        }
    }
}

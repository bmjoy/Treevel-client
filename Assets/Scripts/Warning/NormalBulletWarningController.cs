using UnityEngine;

namespace Warning
{
    public class NormalBulletWarningController : CartridgeWarningController
    {
        public override void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale,
            float bulletWidth, float bulletHeight)
        {
            localScale = (float) (WindowSize.WIDTH * 0.15);
            originalWidth = 1.82f;
            originalHeight = 1.52f;
            transform.localScale *= new Vector2(localScale, localScale);
            transform.position = bulletPosition + Vector2.Scale(bulletMotionVector,
                                     new Vector2((bulletLocalScale * bulletWidth + localScale * originalWidth) / 2,
                                         (bulletLocalScale * bulletHeight + localScale * originalHeight) / 2));
        }
    }
}

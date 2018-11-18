using UnityEngine;

namespace GamePlayScene.Warning
{
    public class NormalCartridgeWarningController : CartridgeWarningController
    {
        public void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale,
            float bulletWidth, float bulletHeight)
        {
            localScale = (float) (WindowSize.WIDTH * 0.08);
            originalWidth = GetComponent<Renderer>().bounds.size.x;
            originalHeight = GetComponent<Renderer>().bounds.size.y;
            transform.localScale *= new Vector2(localScale, localScale);
            transform.position = bulletPosition + Vector2.Scale(bulletMotionVector,
                                     new Vector2((bulletLocalScale * bulletWidth + localScale * originalWidth) / 2,
                                         (bulletLocalScale * bulletHeight + localScale * originalHeight) / 2));
        }
    }
}

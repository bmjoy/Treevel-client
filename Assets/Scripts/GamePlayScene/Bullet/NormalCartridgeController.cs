using UnityEngine;

namespace GamePlayScene.Bullet
{
    public class NormalCartridgeController : CartridgeController
    {
        // コンストラクタがわりのメソッド
        public void Initialize(Vector2 position, Vector2 motionVector)
        {
            Initialize(motionVector);
            speed = 0.08f;
            originalWidth = GetComponent<Renderer>().bounds.size.x;
            originalHeight = GetComponent<Renderer>().bounds.size.y;
            localScale = (float) (WindowSize.WIDTH * 0.10);

            if (motionVector.Equals(Vector2.right))
                transform.position = new Vector2(-(WindowSize.WIDTH + localScale * originalWidth) / 2,
                    position.y);
            else if (motionVector.Equals(Vector2.left))
                transform.position = new Vector2((WindowSize.WIDTH + localScale * originalWidth) / 2,
                    position.y);
            else if (motionVector.Equals(Vector2.up))
                transform.position = new Vector2(position.x, -(WindowSize.HEIGHT + localScale * originalHeight) / 2);
            else if (motionVector.Equals(Vector2.down))
                transform.position = new Vector2(position.x, (WindowSize.HEIGHT + localScale * originalHeight) / 2);
            // Check if a bullet should be flipped vertically
            transform.localScale *= new Vector2(localScale, -1 * Mathf.Sign(motionVector.x) * localScale);
        }
    }
}

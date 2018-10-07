using UnityEngine;

namespace Bullet
{
    public class NormalBulletController : CartridgeController
    {
        // コンストラクタがわりのメソッド
        public virtual void Initialize(Vector2 position, Vector2 motionVector)
        {
            base.Initialize(motionVector);
            this.speed = 0.1f;
            this.localScale = (float) (WindowSize.WIDTH * 0.15);

            if (motionVector.Equals(Vector2.right)) this.transform.position = new Vector2(-(WindowSize.WIDTH+localScale)/2, position.y);
            else if (motionVector.Equals(Vector2.left)) this.transform.position = new Vector2((WindowSize.WIDTH+localScale)/2, position.y);
            else if (motionVector.Equals(Vector2.up)) this.transform.position = new Vector2(position.x, -(WindowSize.HEIGHT+localScale)/2);
            else if (motionVector.Equals(Vector2.down)) this.transform.position = new Vector2(position.x, (WindowSize.HEIGHT+localScale)/2);

            // Check if a bullet should be flipped vertically
            this.transform.localScale *= new Vector2(localScale, (-1)*Mathf.Sign(motionVector.x)*localScale);
        }
    }
}

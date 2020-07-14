using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObjectUnifier : MonoBehaviour
    {
        [Range(0, 1), Tooltip("画面の横幅に占める比率")]
        public float RatioToWindowWidth;

        [Tooltip("画像の横縦比")]
        public float ImageRatio = 1f;

        private void Awake()
        {
            var originalWidth = GetComponent<SpriteRenderer>().size.x;
            var originalHeight = GetComponent<SpriteRenderer>().size.y;

            var widthEfficient = WindowSize.WIDTH * RatioToWindowWidth;
            var heightEfficient = widthEfficient * ImageRatio ;
            transform.localScale = new Vector3(widthEfficient / originalWidth, heightEfficient / originalHeight);

            var collider = GetComponent<BoxCollider2D>();
            if (collider == null)
                return;

            collider.size = new Vector2(originalWidth, originalHeight);
        }
    }
}

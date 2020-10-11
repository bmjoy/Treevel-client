using Project.Scripts.Common.Utils;
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

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            Unify();
        }

        public void Unify()
        {
            if (_renderer.sprite == null)
                return;

            var originalWidth = _renderer.sprite.bounds.size.x;
            var originalHeight = _renderer.sprite.bounds.size.y;

            var widthEfficient = Constants.WindowSize.WIDTH * RatioToWindowWidth;
            var heightEfficient = widthEfficient * ImageRatio ;
            transform.localScale = new Vector3(widthEfficient / originalWidth, heightEfficient / originalHeight);

            var collider = GetComponent<BoxCollider2D>();
            if (collider == null)
                return;

            collider.size = new Vector2(originalWidth, originalHeight);
        }
    }
}

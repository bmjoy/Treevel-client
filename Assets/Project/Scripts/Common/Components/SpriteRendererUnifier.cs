using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererUnifier : MonoBehaviour
    {
        /// <summary>
        /// 基準とする横幅
        /// </summary>
        protected float baseWidth;

        [Range(0, 1), Tooltip("画面の横幅に占める比率")] public float RatioToWindowWidth;

        [Tooltip("画像の横縦比")] public float ImageRatio = 1f;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            SetBaseWidth();
            Unify();
        }

        protected virtual void SetBaseWidth()
        {
            baseWidth = Constants.WindowSize.WIDTH;
        }

        /// <summary>
        /// SpriteおよびColliderのサイズを画面に対する比率で調整する
        /// </summary>
        public void Unify()
        {
            if (_renderer.sprite == null) return;

            var originalWidth = _renderer.sprite.bounds.size.x;
            var originalHeight = _renderer.sprite.bounds.size.y;

            var widthEfficient = baseWidth * RatioToWindowWidth;
            var heightEfficient = widthEfficient * ImageRatio;
            transform.localScale = new Vector3(widthEfficient / originalWidth, heightEfficient / originalHeight);

            var collider = GetComponent<BoxCollider2D>();
            if (collider == null) return;

            collider.size = new Vector2(originalWidth, originalHeight);
        }

        /// <summary>
        /// Spriteを設定し、サイズを調整する
        /// </summary>
        /// <param name="sprite"></param>
        public void SetSprite(Sprite sprite)
        {
            _renderer.sprite = sprite;
            Unify();
        }
    }
}

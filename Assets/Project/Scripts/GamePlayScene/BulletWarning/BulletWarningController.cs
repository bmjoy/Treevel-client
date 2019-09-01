using UnityEngine;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public abstract class BulletWarningController : MonoBehaviour
    {
        /// <summary>
        /// 元画像の縦幅
        /// </summary>
        protected float originalHeight;
        /// <summary>
        /// 元画像の横幅
        /// </summary>
        protected float originalWidth;

        protected virtual void Awake()
        {
            originalWidth = GetComponent<SpriteRenderer>().size.x;
            originalHeight = GetComponent<SpriteRenderer>().size.y;
            gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.BULLET_WARNING;
        }

        private void OnEnable()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        private void OnSucceed()
        {
            Destroy(gameObject);
        }

        private void OnFail()
        {
            Destroy(gameObject);
        }
    }
}

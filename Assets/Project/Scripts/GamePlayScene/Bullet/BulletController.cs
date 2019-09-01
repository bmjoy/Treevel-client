using UnityEngine;
using System;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    // Bulletに共通したフィールド、メソッドの定義
    public abstract class BulletController : MonoBehaviour
    {
        /// <summary>
        /// 銃弾の大きさ
        /// </summary>
        [NonSerialized] protected const float LOCAL_SCALE = 1.0f;

        /// <summary>
        /// 元画像の横幅
        /// </summary>
        [NonSerialized] public float originalWidth;
        /// <summary>
        /// 元画像の縦幅
        /// </summary>
        [NonSerialized] public float originalHeight;

        protected virtual void Awake()
        {
            originalWidth = GetComponent<SpriteRenderer>().size.x;
            originalHeight = GetComponent<SpriteRenderer>().size.y;
            // sortingLayerの設定
            gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.BULLET;
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

        protected abstract void OnFail();
    }
}

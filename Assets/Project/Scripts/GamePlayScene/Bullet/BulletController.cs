using UnityEngine;
using System;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
    // Bulletに共通したフィールド、メソッドの定義
    public abstract class BulletController : MonoBehaviour
    {
        [NonSerialized] protected const float LOCAL_SCALE = 1.0f;

        // 元画像のサイズ
        public float originalWidth;
        public float originalHeight;

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

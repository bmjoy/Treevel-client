using System;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Controllers
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

        protected GamePlayDirector gamePlayDirector;

        /// <summary>
        /// 銃弾の移動する速さ
        /// </summary>
        [SerializeField, NonEditable] protected float speed = 0.05f;

        protected virtual void Awake()
        {
            // 元々の画像サイズの取得
            originalWidth = GetComponent<SpriteRenderer>().size.x;
            originalHeight = GetComponent<SpriteRenderer>().size.y;
            // sortingLayerの設定
            gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.BULLET;
            // 衝突判定を有効にする
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            // 重力を用いた物理演算を行わない
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            // GamePlayDirectorの取得
            gamePlayDirector = FindObjectOfType<GamePlayDirector>();
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

        protected abstract void OnTriggerEnter2D(Collider2D other);
    }
}

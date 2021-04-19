using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(SpriteRenderer))]
    public class BottleAttributeControllerBase : GameObjectControllerBase
    {
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// ゲーム開始時に描画するイベントを購読する
        /// </summary>
        protected void Initialize()
        {
            GamePlayDirector.Instance.GameStart.Subscribe(_ => spriteRenderer.enabled = true).AddTo(compositeDisposableOnGameEnd, this);
        }
    }
}

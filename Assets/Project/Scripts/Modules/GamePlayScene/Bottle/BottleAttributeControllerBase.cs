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
    }
}

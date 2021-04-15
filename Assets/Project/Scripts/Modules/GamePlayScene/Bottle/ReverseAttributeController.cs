using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class ReverseAttributeController : GameObjectControllerBase
    {
        private DynamicBottleController _bottleController;

        private Animator _animator;
        private static readonly int _ANIMATOR_PARAM_FLOAT_SPEED = Animator.StringToHash("ReverseSpeed");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            // イベントに処理を登録する
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f)).AddTo(this);
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            // y座標を中心から上に調整する
            transform.localPosition = new Vector3(0, 75f);
            _bottleController = bottleController;

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleAttributeType.Reverse.GetOrderInLayer();
        }
    }
}

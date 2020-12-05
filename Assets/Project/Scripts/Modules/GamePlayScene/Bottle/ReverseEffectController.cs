using Treevel.Common.Entities;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class ReverseEffectController : MonoBehaviour
    {
        private DynamicBottleController _bottleController;

        private Animator _animator;
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "ReverseSpeed";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            // y座標を中心から上に調整する
            transform.localPosition = new Vector3(0, 0.75f);
            _bottleController = bottleController;

            // イベントに処理を登録する
            _bottleController.EndGame += HandleEndGame;

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleEffectType.Reverse.GetOrderInLayer();
        }

        private void OnDestroy()
        {
            _bottleController.EndGame -= HandleEndGame;
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        private void HandleEndGame()
        {
            _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            _bottleController.EndGame -= HandleEndGame;
        }
    }
}

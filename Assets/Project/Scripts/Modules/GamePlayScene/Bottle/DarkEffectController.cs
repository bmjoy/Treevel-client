using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class DarkEffectController : MonoBehaviour
    {
        private NormalBottleController _bottleController;

        /// <summary>
        /// 暗闇状態かどうか
        /// </summary>
        private bool _isDark;

        private Animator _animator;
        private const string _ANIMATOR_IS_DARK = "IsDark";
        private const string _ANIMATOR_PARAM_FLOAT_SPEED = "DarkSpeed";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NormalBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;

            // イベントに処理を登録する
            _bottleController.OnEnterTile += HandleOnEnterTile;
            _bottleController.OnExitTile += HandleOnExitTile;
            _bottleController.OnEndGame += HandleOnEndGame;

            // 初期状態の登録
            _isDark = !_bottleController.IsSuccess();
            _animator.SetBool(_ANIMATOR_IS_DARK, _isDark);
        }

        private void OnDestroy()
        {
            _bottleController.OnEnterTile -= HandleOnEnterTile;
            _bottleController.OnExitTile -= HandleOnExitTile;
            _bottleController.OnEndGame -= HandleOnEndGame;
        }

        /// <summary>
        /// タイルから移動した時の挙動
        /// </summary>
        /// <param name="targetTile"></param>
        private void HandleOnEnterTile(GameObject targetTile)
        {
            _isDark = !_bottleController.IsSuccess();
            _animator.SetBool(_ANIMATOR_IS_DARK, _isDark);
        }

        /// <summary>
        /// タイルから出る時の挙動
        /// </summary>
        /// <param name="targetTile"></param>
        private void HandleOnExitTile(GameObject targetTile)
        {
            _isDark = !_bottleController.IsSuccess();
            _animator.SetBool(_ANIMATOR_IS_DARK, _isDark);
        }

        /// <summary>
        /// ゲーム終了時の挙動
        /// </summary>
        private void HandleOnEndGame()
        {
            _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
        }
    }
}

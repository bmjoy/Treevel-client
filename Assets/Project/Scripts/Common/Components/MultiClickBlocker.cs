using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components
{
    public class MultiClickBlocker : MonoBehaviour
    {
        /// <summary>
        /// button.interactable が変化する際に発火する static な event
        /// </summary>
        private static event Action<bool> _buttonInteractableChanged;

        /// <summary>
        /// 個々の button
        /// </summary>
        private Button _button;

        /// <summary>
        /// 特定のボタンタップ後、他のボタンをブロックする時間
        /// </summary>
        private const int _BLOCKING_TIME = 500;

        /// <summary>
        /// 予期しない状況かどうか
        /// </summary>
        private bool _canChangeButtonInteractable = true;

        private void Awake()
        {
            _buttonInteractableChanged += HandleButtonInteractableChanged;
            _button = GetComponent<Button>();

            if (_button != null) {
                _button.onClick.AddListener(HandleOnClick);
            }
        }

        private void OnDestroy()
        {
            _buttonInteractableChanged -= HandleButtonInteractableChanged;
        }

        private void HandleButtonInteractableChanged(bool interactable)
        {
            if (interactable) {
                if (_canChangeButtonInteractable == false) {
                    _canChangeButtonInteractable = true;
                    return;
                }
            } else {
                if (_button.interactable == false) {
                    _canChangeButtonInteractable = false;
                    return;
                }
            }

            _button.interactable = interactable;
        }

        private async void HandleOnClick()
        {
            // 同一フレームに 2 回のイベントが発生した場合
            if (_button.interactable == false) return;

            _buttonInteractableChanged?.Invoke(false);

            await Task.Delay(_BLOCKING_TIME);

            _buttonInteractableChanged?.Invoke(true);
        }
    }
}

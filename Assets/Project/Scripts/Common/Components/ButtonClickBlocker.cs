using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components
{
    public class ButtonClickBlocker : MonoBehaviour
    {
        /// <summary>
        /// button.enabled が変化する際に発火する static な event
        /// </summary>
        private static event Action<bool> _buttonEnabledChanged;

        /// <summary>
        /// 個々の button
        /// </summary>
        [SerializeField] private Button _button;

        /// <summary>
        /// 特定のボタンタップ後、他のボタンをブロックする時間
        /// </summary>
        [SerializeField] private int _blockingTime = 1000;

        /// <summary>
        /// 予期しない状況かどうか
        /// </summary>
        private bool _isUnexpected = false;

        private void Awake()
        {
            _buttonEnabledChanged += HandleButtonEnabledChanged;
            _button.onClick.AddListener(HandleOnClick);
        }

        private void OnDestroy()
        {
            _buttonEnabledChanged -= HandleButtonEnabledChanged;
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }

        private void HandleButtonEnabledChanged(bool isEnabled)
        {
            if (isEnabled) {
                if (_isUnexpected) {
                    _isUnexpected = false;
                    return;
                }
            } else {
                if (_button.enabled == false) {
                    _isUnexpected = true;
                    return;
                }
            }

            _button.enabled = isEnabled;
        }

        private async void HandleOnClick()
        {
            // 同一フレームに 2 回のイベントが発生した場合
            if (_button.enabled == false) return;

            _buttonEnabledChanged?.Invoke(false);

            await Task.Delay(_blockingTime);

            _buttonEnabledChanged?.Invoke(true);
        }
    }
}

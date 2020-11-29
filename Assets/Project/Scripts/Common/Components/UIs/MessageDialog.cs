using System;
using System.Collections;
using System.Collections.Generic;
using Treevel.Common.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components.UIs
{
    public class MessageDialog : MonoBehaviour
    {
        [SerializeField] private MultiLanguageText _message;
        [SerializeField] private Button _okButton;
        [SerializeField] private MultiLanguageText _okButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private MultiLanguageText _cancelButtonText;

        public void Initialize(ETextIndex message, ETextIndex okText, Action okCallBack)
        {
            // メッセージ設定
            _message.TextIndex = message;

            // OKボタンの文字
            _okButtonText.TextIndex = okText;

            // コールバック設定
            _okButton.onClick.RemoveAllListeners();
            _okButton.onClick.AddListener(() => {
                // 設定したコールバックを実行
                okCallBack?.Invoke();
                // クリックした後閉じる
                gameObject.SetActive(false);
            });
        }
    }
}

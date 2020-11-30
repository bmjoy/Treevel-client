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
        public enum EDialogType {
            Ok_Cancel, // 確定、キャンセルボタン付き
            Ok, 　 // 確定ボタン付き
        }

        [SerializeField] private RectTransform _OkCancelTypeBtnPos;
        [SerializeField] private RectTransform _OkTypeBtnPos;
        [SerializeField] private MultiLanguageText _message;
        [SerializeField] private Button _okButton;
        [SerializeField] private MultiLanguageText _okButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private MultiLanguageText _cancelButtonText;

        public void Initialize(EDialogType dialogType, ETextIndex message, ETextIndex okText, Action okCallBack)
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

            // OKボタンの位置設定
            if (dialogType == EDialogType.Ok_Cancel) {
                _okButton.transform.SetParent(_OkCancelTypeBtnPos);
            } else if (dialogType == EDialogType.Ok) {
                _okButton.transform.SetParent(_OkTypeBtnPos);
                _cancelButton.gameObject.SetActive(false);
            }
        }
    }
}

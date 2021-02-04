using System;
using Treevel.Common.Entities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components.UIs
{
    public class MessageDialog : MonoBehaviour
    {
        /// <summary>
        /// ダイアログのタイプ
        /// </summary>
        public enum EDialogType
        {
            Ok_Cancel, // 確定、キャンセルボタン付き
            Ok,        // 確定ボタン付き
        }

        [SerializeField] private RectTransform _OkCancelTypeOkBtnPos;
        [SerializeField] private RectTransform _OkTypeBtnPos;
        [SerializeField] private MultiLanguageText _message;
        [SerializeField] private Button _okButton;
        [SerializeField] private MultiLanguageText _okButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private MultiLanguageText _cancelButtonText;

        /// <summary>
        /// 外側タップする時に閉じるためのボタン
        /// </summary>
        [SerializeField] private Button _backgroundButton;

        /// <summary>
        /// ok button のクリックイベントを破棄するための disposable
        /// </summary>
        private IDisposable _disposable;

        private void Awake()
        {
            _cancelButton.onClick.AsObservable()
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(this);
        }

        public void Initialize(EDialogType dialogType, ETextIndex message, ETextIndex okText, Action okCallBack)
        {
            // メッセージ設定
            _message.TextIndex = message;

            // OKボタンの文字
            _okButtonText.TextIndex = okText;

            // 以前の disposable は破棄する
            _disposable?.Dispose();
            _disposable = _okButton.onClick.AsObservable()
                .Subscribe(_ => {
                    // 設定したコールバックを実行
                    okCallBack?.Invoke();
                    // クリックした後閉じる
                    gameObject.SetActive(false);
                })
                .AddTo(this);

            // OKボタンの位置設定
            if (dialogType == EDialogType.Ok_Cancel) {
                _okButton.transform.SetParent(_OkCancelTypeOkBtnPos, false);
                _cancelButton.gameObject.SetActive(true);
            } else if (dialogType == EDialogType.Ok) {
                _okButton.transform.SetParent(_OkTypeBtnPos, false);
                _cancelButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 外側タップで閉じれるかどうかの設定
        /// </summary>
        public void SetBackgroundButtonActive(bool active)
        {
            _backgroundButton.enabled = active;
        }
    }
}

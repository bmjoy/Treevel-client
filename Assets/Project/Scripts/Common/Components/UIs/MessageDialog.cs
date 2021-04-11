using System;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
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
        /// OK ボタンがクリックされた時に実行される Action
        /// </summary>
        private Action _okCallBack;

        private void Awake()
        {
            _cancelButton.onClick.AsObservable()
                .Subscribe(_ => {
                    SoundManager.Instance.PlaySE(ESEKey.UI_Dropdown_Close);
                    gameObject.SetActive(false);
                })
                .AddTo(this);

            _okButton.onClick.AsObservable()
                .Subscribe(_ => {
                    SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);

                    // コールバックが設定されていたら実行する
                    _okCallBack?.Invoke();
                    // ユースケース的には必要ないが、安全のために null に戻しておく
                    _okCallBack = null;
                    // クリックした後閉じる
                    gameObject.SetActive(false);
                })
                .AddTo(this);
        }

        public void Initialize(EDialogType dialogType, ETextIndex message, ETextIndex okText, Action okCallBack)
        {
            // メッセージ設定
            _message.TextIndex = message;

            // OKボタンの文字
            _okButtonText.TextIndex = okText;

            // コールバックの設定
            _okCallBack = okCallBack;

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

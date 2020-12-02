using System;
using Treevel.Common.Components.UIs;
using Treevel.Common.Entities;
using Treevel.Common.Patterns.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Common.Managers
{
    /// <summary>
    /// 全体ゲームに出現するUI（プログレスバー、メッセージダイアログ等）
    /// を制御するマネージャークラス
    /// </summary>
    public class UIManager : SingletonObject<UIManager>
    {
        /// <summary>
        /// プログレスバーのプレハブ
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _progressBar;

        /// <summary>
        /// エラーメッセージのポップアップ
        /// 普段使われない予想なので、使うときだけロード、実体化させる
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _errorMessageBoxRef;

        /// <summary>
        /// 汎用メッセージダイアログのプレハブ
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _messageDialogRef;

        /// <summary>
        /// プログレスバーのインスタンス
        /// </summary>
        public ProgressBar ProgressBar
        {
            get;
            private set;
        }

        private MessageDialog _messageDialog;

        /// <summary>
        /// 初期化済みかどうか
        /// </summary>
        public bool Initialized
        {
            get;
            private set;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            #if !UNITY_EDITOR && UNITY_ANDROID
            // ステータスバーを表示する
            StatusBarController.Show();
            #endif

            // キャンバスがなければ作る
            if (GetComponentInChildren<Canvas>() == null) {
                gameObject.AddComponent<Canvas>();
            }

            var canvas = GetComponentInChildren<Canvas>().transform;

            // キャンバスの下にプログレスバーの実体を生成
            _progressBar.InstantiateAsync(canvas).Completed += (obj) => {
                ProgressBar = obj.Result.GetComponentInChildren<ProgressBar>();
            };

            _messageDialogRef.InstantiateAsync(canvas).Completed += (obj) => {
                _messageDialog = obj.Result.GetComponentInChildren<MessageDialog>();
            };
        }

        private void Update()
        {
            // TODO:UniRx導入したら複数タスク待ちの実装でやる（タスク待ちはUniTask？）
            if (!Initialized) {
                Initialized = (ProgressBar != null) && (_messageDialog != null);
            }
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="errorCode">対応するエラーコード</param>
        public void ShowErrorMessage(EErrorCode errorCode)
        {
            var canvas = GetComponentInChildren<Canvas>().transform;
            _errorMessageBoxRef.InstantiateAsync(canvas).Completed += (op) => {
                var messageBoxObj = op.Result;

                // テキスト、エラーコードを設定
                messageBoxObj.GetComponent<ErrorMessageBox>().ErrorCode = errorCode;
            };
        }

        /// <summary>
        /// 確定、キャンセルボタン付きメッセージダイアログを作成
        /// </summary>
        /// <param name="message"> メッセージインデックス </param>
        /// <param name="okText"> OKボタン用文字 </param>
        /// <param name="okCallback"> OKボタン押した時のコールバック </param>
        public void CreateOkCancelMessageDialog(ETextIndex message, ETextIndex okText, Action okCallback)
        {
            _messageDialog.Initialize(MessageDialog.EDialogType.Ok_Cancel, message, okText, okCallback);
            _messageDialog.gameObject.SetActive(true);
        }

        /// <summary>
        /// 確定ボタンのみのメッセージダイアログを作成
        /// </summary>
        /// <param name="message"> メッセージインデックス </param>
        /// <param name="okText"> OKボタン用文字 </param>
        /// <param name="okCallback"> OKボタン押した時のコールバック </param>
        public void CreateOkMessageDialog(ETextIndex message, ETextIndex okText, Action okCallback)
        {
            _messageDialog.Initialize(MessageDialog.EDialogType.Ok, message, okText, okCallback);
            _messageDialog.gameObject.SetActive(true);
        }
    }
}

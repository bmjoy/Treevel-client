using System;
using Cysharp.Threading.Tasks;
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
    public class UIManager : SingletonObjectBase<UIManager>
    {
        /// <summary>
        /// プログレスバーのプレハブ
        /// </summary>
        [SerializeField] private AssetReferenceGameObject _progressBar;

        /// <summary>
        /// エラーメッセージのポップアップ
        /// 普段使われない予想なので、使うときだけロード、実体化させる
        /// </summary>
        [SerializeField] private AssetReferenceGameObject _errorMessageBoxRef;

        /// <summary>
        /// 汎用メッセージダイアログのプレハブ
        /// </summary>
        [SerializeField] private AssetReferenceGameObject _messageDialogRef;

        /// <summary>
        /// プログレスバーのインスタンス
        /// </summary>
        public ProgressBar ProgressBar { get; private set; }

        private MessageDialog _messageDialog;

        public async UniTask InitializeAsync()
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
            var task1 = _progressBar.InstantiateAsync(canvas).ToUniTask();
            var task2 = _messageDialogRef.InstantiateAsync(canvas).ToUniTask();

            var (progressBarGo, messageDialogGo) = await UniTask.WhenAll(task1, task2);

            ProgressBar = progressBarGo.GetComponentInChildren<ProgressBar>();
            _messageDialog = messageDialogGo.GetComponentInChildren<MessageDialog>();
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="errorCode">対応するエラーコード</param>
        public UniTask ShowErrorMessageAsync(EErrorCode errorCode)
        {
            var canvas = GetComponentInChildren<Canvas>().transform;
            return _errorMessageBoxRef.InstantiateAsync(canvas)
                .ToUniTask()
                .ContinueWith(obj => obj.GetComponent<ErrorMessageBox>().ErrorCode = errorCode);
        }

        /// <summary>
        /// 確定、キャンセルボタン付きメッセージダイアログを作成
        /// </summary>
        /// <param name="message"> メッセージインデックス </param>
        /// <param name="okText"> OKボタン用文字 </param>
        /// <param name="okCallback"> OKボタン押した時のコールバック </param>
        public void CreateOkCancelMessageDialog(ETextIndex message, ETextIndex okText, Action okCallback,
                                                bool backgroundBtnActive = true)
        {
            _messageDialog.Initialize(MessageDialog.EDialogType.Ok_Cancel, message, okText, okCallback);
            _messageDialog.SetBackgroundButtonActive(backgroundBtnActive);
            _messageDialog.gameObject.SetActive(true);
        }

        /// <summary>
        /// 確定ボタンのみのメッセージダイアログを作成
        /// </summary>
        /// <param name="message"> メッセージインデックス </param>
        /// <param name="okText"> OKボタン用文字 </param>
        /// <param name="okCallback"> OKボタン押した時のコールバック </param>
        public void CreateOkMessageDialog(ETextIndex message, ETextIndex okText, Action okCallback = null,
                                          bool backgroundBtnActive = true)
        {
            _messageDialog.Initialize(MessageDialog.EDialogType.Ok, message, okText, okCallback);
            _messageDialog.SetBackgroundButtonActive(backgroundBtnActive);
            _messageDialog.gameObject.SetActive(true);
        }
    }
}

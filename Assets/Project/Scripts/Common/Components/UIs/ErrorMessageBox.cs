using Project.Scripts.Common.Utils;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// エラー発生時にユーザに提示するエラーメッセージのポップアップ
    /// 以下のフローを従って新しいエラーを定義する：
    /// 1. <see cref="EErrorCode">でエラーコード追加する
    /// 2. <see cref="ETextIndex">とtranslation.csvでユーザに提示するメッセージを追加する
    ///
    /// メモリ節約のため、エラーメッセージボックスは必要になる度に作成し、タイトル画面に戻るボタンを押すと破壊する。
    /// </summary>
    public class ErrorMessageBox : MonoBehaviour
    {
        /// <summary>
        /// ユーザに提示するメッセージ
        /// </summary>
        [SerializeField]
        public MultiLanguageText _text;

        /// <summary>
        /// 該当エラーのコード
        /// </summary>
        private EErrorCode _errorCode;

        public EErrorCode ErrorCode
        {
            get {
                return _errorCode;
            }
            set {
                _errorCode = value;
                // エラーコードから対応するエラーメッセージを取得
                _text.TextIndex = (ETextIndex)((int)ETextIndex.ErrorTextStart + (int)_errorCode);
                // _errorCodeが1なら001と出力
                _text.text += $"(ErrorCode:{((int)_errorCode):D3})";
            }
        }

        private void OnEnable()
        {
            // エラーになったらゲーム停止
            Time.timeScale = 0.0f;
        }

        private void OnDisable()
        {
            // タイムスケールを元に戻す
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// タイトル画面に戻るボタンがクリックされたときの挙動
        /// </summary>
        public void OnReturnToTitleButtonClicked()
        {
            // スプラッシュ画面に変える
            AddressableAssetManager.LoadScene(Constants.SceneName.START_UP_SCENE);

            // 実体を破壊し、アセットのメモリ領域も同時に解放されるはず
            Addressables.Release(gameObject);
        }
    }
}

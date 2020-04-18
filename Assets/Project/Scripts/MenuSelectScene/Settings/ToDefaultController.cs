using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.Utils.TextUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class ToDefaultController : MonoBehaviour
    {
        public delegate void ChangeAction();

        /// <summary>
        /// 失敗時のイベント
        /// </summary>
        public static event ChangeAction OnUpdate;

        /// <summary>
        /// デフォルトに戻すボタン
        /// </summary>
        private Button _toDefaultButton;

        private void Awake()
        {
            _toDefaultButton = GetComponent<Button>();
            _toDefaultButton.onClick.AddListener(ToDefaultButtonDown);
        }

        /// <summary>
        /// デフォルトに戻すボタンを押した場合の処理
        /// </summary>
        private static void ToDefaultButtonDown()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.LANGUAGE);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_DETAILS);

            // Canvasの更新
            OnUpdate?.Invoke();
            // 言語の更新
            LanguageUtility.CurrentLanguage = Default.LANGUAGE;
        }
    }
}

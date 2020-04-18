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
        /// デフォルトボタンが押された際のイベント
        /// </summary>
        public static event ChangeAction OnUpdate;

        /// <summary>
        /// デフォルトに戻すボタンを押した場合の処理
        /// </summary>
        public void ToDefaultButtonDown()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.LANGUAGE);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_DETAILS);

            // Canvasの更新
            OnUpdate?.Invoke();
        }
    }
}

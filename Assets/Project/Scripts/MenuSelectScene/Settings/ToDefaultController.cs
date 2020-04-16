using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class ToDefaultController : MonoBehaviour
    {
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
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.BGM);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SE);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_DETAILS);
        }
    }
}

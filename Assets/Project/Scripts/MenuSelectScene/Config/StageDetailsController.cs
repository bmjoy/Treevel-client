using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;


namespace Project.Scripts.MenuSelectScene.Config
{
    public class StageDetailsController : MonoBehaviour
    {
        /// <summary>
        /// ONボタン
        /// </summary>
        private Button _onButton;

        /// <summary>
        /// OFFボタン
        /// </summary>
        private Button _offButton;

        private void Awake()
        {
            _onButton = transform.Find("On").GetComponent<Button>();
            _onButton.onClick.AddListener(OnButtonDown);

            _offButton = transform.Find("Off").GetComponent<Button>();
            _offButton.onClick.AddListener(OffButtonDown);
        }

        /// <summary>
        /// ONボタンを押した場合の処理
        /// </summary>
        private static void OnButtonDown()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SHOW_DETAILED, 1);
        }

        /// <summary>
        /// OFFボタンを押した場合の処理
        /// </summary>
        private static void OffButtonDown()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SHOW_DETAILED, 0);
        }
    }
}

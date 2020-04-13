using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class StageDetailsController : MonoBehaviour
    {
        /// <summary>
        /// ONボタンを押した場合の処理
        /// </summary>
        public void OnButtonDown()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SHOW_DETAILED, 1);
        }

        /// <summary>
        /// OFFボタンを押した場合の処理
        /// </summary>
        public void OffButtonDown()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SHOW_DETAILED, 0);
        }
    }
}

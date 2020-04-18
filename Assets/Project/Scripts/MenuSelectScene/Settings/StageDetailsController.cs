using UnityEngine;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class StageDetailsController : MonoBehaviour
    {
        /// <summary>
        /// ONボタンを押した場合の処理
        /// </summary>
        public void OnButtonDown()
        {
            SettingsManager.StageDetails = 1;
        }

        /// <summary>
        /// OFFボタンを押した場合の処理
        /// </summary>
        public void OffButtonDown()
        {
            SettingsManager.StageDetails = 0;
        }
    }
}

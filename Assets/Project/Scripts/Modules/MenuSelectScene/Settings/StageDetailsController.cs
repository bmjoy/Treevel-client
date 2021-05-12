using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class StageDetailsController : MonoBehaviour
    {
        /// <summary>
        /// ONボタンを押した場合の処理
        /// </summary>
        public void OnButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            UserSettings.Instance.StageDetails = 1;
        }

        /// <summary>
        /// OFFボタンを押した場合の処理
        /// </summary>
        public void OffButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            UserSettings.Instance.StageDetails = 0;
        }
    }
}

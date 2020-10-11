using Treevel.Common.Entities;
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
            UserSettings.StageDetails = 1;
        }

        /// <summary>
        /// OFFボタンを押した場合の処理
        /// </summary>
        public void OffButtonDown()
        {
            UserSettings.StageDetails = 0;
        }
    }
}

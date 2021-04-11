using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class ToDefaultController : MonoBehaviour
    {
        /// <summary>
        /// デフォルトに戻すボタンを押した場合の処理
        /// </summary>
        public void ToDefaultButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            // 設定の更新
            UserSettings.ToDefault();
        }
    }
}

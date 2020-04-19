using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

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
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_DETAILS);

            // Canvasの更新
            OnUpdate?.Invoke();
        }
    }
}

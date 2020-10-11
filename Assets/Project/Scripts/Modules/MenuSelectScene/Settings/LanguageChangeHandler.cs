using Project.Scripts.Utils.TextUtils;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.MenuSelectScene.Settings
{
    /// <summary>
    /// ボタンで言語選択を制御するためのオブジェクトクラス
    /// </summary>
    public class LanguageChangeHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ELanguage language;
        public void OnPointerClick(PointerEventData eventData)
        {
            UserSettings.CurrentLanguage = language;
        }
    }
}

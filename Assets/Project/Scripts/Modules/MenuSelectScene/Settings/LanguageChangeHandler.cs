using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    /// <summary>
    /// ボタンで言語選択を制御するためのオブジェクトクラス
    /// </summary>
    public class LanguageChangeHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ELanguage language;
        public void OnPointerClick(PointerEventData eventData)
        {
            UserSettings.CurrentLanguage.Value = language;
        }
    }
}

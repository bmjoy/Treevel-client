using Treevel.Common.Entities;
using Treevel.Common.Managers;
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
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            UserSettings.CurrentLanguage.Value = language;
        }
    }
}

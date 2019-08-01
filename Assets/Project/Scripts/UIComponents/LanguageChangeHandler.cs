using Project.Scripts.Utils.TextUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// ボタンで言語選択を制御するためのオブジェクトクラス
    /// </summary>
    public class LanguageChangeHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ELanguage language;
        public void OnPointerClick(PointerEventData eventData)
        {
            LanguageUtility.CurrentLanguage = language;
        }
    }
}

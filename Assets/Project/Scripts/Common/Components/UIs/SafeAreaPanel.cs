using UnityEngine;

namespace Treevel.Common.Components.UIs
{
    public class SafeAreaPanel : MonoBehaviour
    {
        private RectTransform _panel;

        private void Awake()
        {
            _panel = GetComponent<RectTransform>();
            var (anchorMin, anchorMax) = GetSafeAreaAnchor();
            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;
        }

        public static (Vector2, Vector2) GetSafeAreaAnchor()
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y = StatusBarController.GetStatusBarBottomAnchor();
            return (anchorMin, anchorMax);
        }
    }
}

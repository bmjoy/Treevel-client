using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.ConfigScene
{
    public class HideOverviewController : MonoBehaviour
    {
        /// <summary>
        /// 概要を表示するかのトグル
        /// </summary>
        private Toggle _hideOverviewToggle;

        private void Awake()
        {
            _hideOverviewToggle = GetComponent<Toggle>();
            _hideOverviewToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.DO_NOT_SHOW, 0) == 1;
            _hideOverviewToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(_hideOverviewToggle);
            });
        }

        /// <summary>
        /// 概要を表示するかのトグルが押されたときの処理
        /// </summary>
        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.DO_NOT_SHOW, toggle.isOn ? 1 : 0);
        }
    }
}

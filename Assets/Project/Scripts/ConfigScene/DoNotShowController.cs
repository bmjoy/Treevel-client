using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.ConfigScene
{
    public class DoNotShowController : MonoBehaviour
    {
        /// <summary>
        /// 概要を表示するかのトグル
        /// </summary>
        private Toggle _doNotShowToggle;

        private void Awake()
        {
            _doNotShowToggle = GetComponent<Toggle>();
            _doNotShowToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.DO_NOT_SHOW, 0) == 1;
            _doNotShowToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(_doNotShowToggle);
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

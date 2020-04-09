using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class SwitchStageDetailedController : MonoBehaviour
    {
        /// <summary>
        /// ステージ詳細を表示するかのトグル
        /// </summary>
        private Toggle _switchStageDetailedToggle;

        private void Awake()
        {
            _switchStageDetailedToggle = GetComponent<Toggle>();
            _switchStageDetailedToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.SHOW_DETAILED, 0) == 1;
            _switchStageDetailedToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(_switchStageDetailedToggle);
            });
        }

        /// <summary>
        /// ステージ詳細を表示するかのトグルが押されたときの処理
        /// </summary>
        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SHOW_DETAILED, toggle.isOn ? 1 : 0);
        }
    }
}

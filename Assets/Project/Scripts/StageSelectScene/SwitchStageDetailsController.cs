using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class SwitchStageDetailsController : MonoBehaviour
    {
        /// <summary>
        /// ステージ詳細を表示するかのトグル
        /// </summary>
        private Toggle _switchStageDetailsToggle;

        private void Awake()
        {
            _switchStageDetailsToggle = GetComponent<Toggle>();
            _switchStageDetailsToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS) == 1;
            _switchStageDetailsToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(_switchStageDetailsToggle);
            });
        }

        /// <summary>
        /// ステージ詳細を表示するかのトグルが押されたときの処理
        /// </summary>
        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.STAGE_DETAILS, toggle.isOn ? 1 : 0);
        }
    }
}

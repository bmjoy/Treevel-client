using Treevel.Common.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
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
            _switchStageDetailsToggle.isOn = UserSettings.StageDetails == 1;
            _switchStageDetailsToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(_switchStageDetailsToggle);
            });
        }

        /// <summary>
        /// ステージ詳細を表示するかのトグルが押されたときの処理
        /// </summary>
        private static void ToggleValueChanged(Toggle toggle)
        {
            UserSettings.StageDetails = toggle.isOn ? 1 : 0;
        }
    }
}

using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class BGMController : MonoBehaviour
    {
        /// <summary>
        /// BGMスライダー
        /// </summary>
        private Slider _BGMSlider;

        private void Awake()
        {
            _BGMSlider = GetComponent<Slider>();
            _BGMSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM, Default.BGM);
            _BGMSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        /// <summary>
        /// BGMスライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM, _BGMSlider.value);
        }
    }
}

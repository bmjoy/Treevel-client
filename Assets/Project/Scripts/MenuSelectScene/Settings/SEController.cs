using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class SEController : MonoBehaviour
    {
        /// <summary>
        /// SEスライダー
        /// </summary>
        private Slider _SESlider;

        private void Awake()
        {
            _SESlider = GetComponent<Slider>();
            _SESlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.SE, Audio.DEFAULT_SE);
            _SESlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        /// <summary>
        /// SEスライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SE, _SESlider.value);
        }
    }
}

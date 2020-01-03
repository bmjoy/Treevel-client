using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.ConfigScene
{
    public class VolumeController : MonoBehaviour
    {
        /// <summary>
        /// 音量スライダー
        /// </summary>
        private Slider _volumeSlider;

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
            _volumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME, Audio.DEFAULT_VOLUME);
            _volumeSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        /// <summary>
        /// 音量スライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.VOLUME, _volumeSlider.value);
        }
    }
}

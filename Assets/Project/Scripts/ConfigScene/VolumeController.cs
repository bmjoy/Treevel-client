using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.ConfigScene
{
    public class VolumeController : MonoBehaviour
    {
        private Slider _volumeSlider;

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
            _volumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME, Audio.DEFAULT_VOLUME);
            _volumeSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.VOLUME, _volumeSlider.value);
        }
    }
}

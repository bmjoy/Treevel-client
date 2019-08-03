using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.ConfigScene
{
    public class VolumeController : MonoBehaviour
    {
        private Slider volumeSlider;

        private void Awake()
        {
            volumeSlider = GetComponent<Slider>();
            volumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME, Audio.DEFAULT_VOLUME);
            volumeSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.VOLUME, volumeSlider.value);
        }
    }
}

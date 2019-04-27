using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.ConfigScene
{
	public class VolumeController : MonoBehaviour
	{
		private Slider volumeSlider;

		private void Awake()
		{
			volumeSlider = GetComponent<Slider>();
			volumeSlider.value = PlayerPrefs.GetFloat("Volume", Audio.DEFAULT_VOLUME);
			volumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
		}

		private void ValueChangeCheck()
		{
			PlayerPrefs.SetFloat("Volume", volumeSlider.value);
		}
	}
}

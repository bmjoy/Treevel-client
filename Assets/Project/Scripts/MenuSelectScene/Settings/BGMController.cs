using System;
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
            _BGMSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME);
            _BGMSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });
        }

        private void OnEnable()
        {
            ToDefaultController.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            ToDefaultController.OnUpdate -= OnUpdate;
        }

        /// <summary>
        /// BGMスライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM_VOLUME, _BGMSlider.value);
        }

        /// <summary>
        /// デフォルト設定に戻された時に呼ばれる
        /// </summary>
        private void OnUpdate()
        {
            _BGMSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME);
        }
    }
}

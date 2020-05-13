﻿using UnityEngine;
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
            _BGMSlider.value = SettingsManager.BGMVolume;
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
            SettingsManager.BGMVolume = _BGMSlider.value;
        }

        /// <summary>
        /// デフォルト設定に戻された時に呼ばれる
        /// </summary>
        private void OnUpdate()
        {
            _BGMSlider.value = SettingsManager.BGMVolume;
        }
    }
}
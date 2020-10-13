using Treevel.Common.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Settings
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
            _SESlider.value = UserSettings.SEVolume;
            _SESlider.onValueChanged.AddListener(delegate {
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
        /// SEスライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            UserSettings.SEVolume = _SESlider.value;
        }

        /// <summary>
        /// デフォルト設定に戻された時に呼ばれる
        /// </summary>
        private void OnUpdate()
        {
            _SESlider.value = UserSettings.SEVolume;
        }
    }
}

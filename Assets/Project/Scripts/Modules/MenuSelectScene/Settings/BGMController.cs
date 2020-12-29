using Treevel.Common.Entities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Settings
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
            _BGMSlider.value = UserSettings.BGMVolume;
            _BGMSlider.onValueChanged.AddListener(delegate {
                ValueChangeCheck();
            });

            ToDefaultController.OnToDefaultClicked.Subscribe(_ => OnUpdate()).AddTo(this);
        }

        /// <summary>
        /// BGMスライダーが変化した場合の処理
        /// </summary>
        private void ValueChangeCheck()
        {
            UserSettings.BGMVolume = _BGMSlider.value;
        }

        /// <summary>
        /// デフォルト設定に戻された時に呼ばれる
        /// </summary>
        private void OnUpdate()
        {
            _BGMSlider.value = UserSettings.BGMVolume;
        }
    }
}

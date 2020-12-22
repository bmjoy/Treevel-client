using Treevel.Common.Entities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class SeasonSelectButton : MonoBehaviour
    {
        public Color SeasonColor => _selectedColor;
        [SerializeField] private Color _selectedColor;

        /// <summary>
        /// トグルに紐付ける季節
        /// </summary>
        public ESeasonId SeasonId => _seasonId;
        [SerializeField] private ESeasonId _seasonId;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.OnValueChangedAsObservable().Subscribe(selected => {
                var image = _toggle.targetGraphic.GetComponent<Image>();
                if (selected)
                    image.color = _selectedColor;
                else
                    image.color = Color.clear;
            }).AddTo(this);
        }
    }
}


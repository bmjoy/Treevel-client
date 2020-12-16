using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class SeasonSelectButton : MonoBehaviour
    {
        [SerializeField] private Color _selectedColor;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.OnValueChangedAsObservable().Subscribe(selected =>
            {
                var image = _toggle.targetGraphic.GetComponent<Image>();
                if (selected)
                    image.color = _selectedColor;
                else
                    image.color = Color.clear;
            }).AddTo(this);
        }
    }
}


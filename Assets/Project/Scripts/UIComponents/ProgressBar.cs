using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UIComponents
{
    public class ProgressBar : MonoBehaviour
    {
        /// <summary>
        /// プログレス
        /// </summary>
        [Range(0f, 100f)]
        private float _progress;


        /// <summary>
        /// プログレスを表示するテキスト
        /// </summary>
        [SerializeField] private Image _progressImage;

        public float Progress {
            get => _progress;
            set {
                if (_progress.Equals(value))
                    return;
                _progress = value;
                _progressImage.fillAmount = _progress / 100f;
            }
        }

        private void Awake()
        {
            Progress = 0;
        }
    }
}

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
        /// 更新頻度を抑えるための閾値
        /// </summary>
        private const float THRESHOLD = 1f;

        /// <summary>
        /// プログレスを示すイメージ
        /// </summary>
        [SerializeField] private Image _progressImage;

        /// <summary>
        /// プログレスを示すテキスト(x %)
        /// </summary>
        [SerializeField] private Text _progressText;

        public float Progress {
            get => _progress;
            set {
                if (Mathf.Abs(_progress - value) < THRESHOLD)
                    return;

                _progress = Mathf.Clamp(value, 0, 100);
                _progressImage.fillAmount = _progress / 100f;
                _progressText.text = $"{_progress:F0} %";
            }
        }

        private void Awake()
        {
            // イメージ、テキストを初期化
            Progress = 0;
        }
    }
}

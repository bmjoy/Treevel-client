using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Managers;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Treevel.Common.Components.UIs
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

        /// <summary>
        /// ロード中のプロセス
        /// </summary>
        private readonly List<AsyncOperationHandle> _loadingOps = new List<AsyncOperationHandle>();

        public float Progress
        {
            get => _progress;
            set {
                if (Mathf.Abs(_progress - value) < THRESHOLD) return;

                _progress = Mathf.Clamp(value, 0, 100);
                _progressImage.fillAmount = _progress / 100f;
                _progressText.text = $"{_progress:F0} %";
            }
        }

        private void Awake()
        {
            // イメージ、テキストを初期化
            Progress = 0;

            // アセットロードするイベントを購読
            AddressableAssetManager.OnAssetStartLoad.Subscribe(op => {
                _loadingOps.Add(op);
                op.Completed += op1 => {
                    var idx = _loadingOps.FindIndex(h => h.GetHashCode() == op1.GetHashCode());
                    _loadingOps.RemoveAt(idx);
                };

                if (!gameObject.activeSelf) {
                    gameObject.SetActive(true);
                }
            }).AddTo(this);
        }

        private void Update()
        {
            if (_loadingOps.Count > 0) {
                Progress = _loadingOps.Select(op => op.PercentComplete).Sum() / _loadingOps.Count;
            } else if (gameObject.activeSelf) {
                gameObject.SetActive(false);
            }
        }
    }
}

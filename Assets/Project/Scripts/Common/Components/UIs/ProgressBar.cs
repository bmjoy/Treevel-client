using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private List<AsyncOperationHandle> _loadingOps = new List<AsyncOperationHandle>();

        public float Progress
        {
            get => _progress;
            set {
                if (Mathf.Abs(_progress - value) < THRESHOLD)
                    return;

                _progress = Mathf.Clamp(value, 0, 100);
                _progressImage.fillAmount = _progress / 100f;
                _progressText.text = $"{_progress:F0} %";
            }
        }

        public void Load(AsyncOperationHandle op)
        {
            _loadingOps.Add(op);
            op.Completed += (op1 => {
                _loadingOps.Remove(op1);
            });

            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
        }

        public void Load(ICollection<AsyncOperationHandle> ops)
        {
            foreach (var op in ops) {
                Load(op);
            }
        }

        private void Awake()
        {
            // イメージ、テキストを初期化
            Progress = 0;
        }

        private void OnEnable()
        {
            StartCoroutine(ShowProgressBar());
        }

        private void OnDisable()
        {
            _loadingOps.Clear();
        }

        private IEnumerator ShowProgressBar()
        {
            while (true) {
                // 進捗の計算
                var percent = _loadingOps.Select(op => op.PercentComplete).Sum() / _loadingOps.Count;

                // Set Progress
                Progress = percent;

                // 全部終わったら終了
                if (_loadingOps.All(op => op.IsDone)) {
                    break;
                }

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}

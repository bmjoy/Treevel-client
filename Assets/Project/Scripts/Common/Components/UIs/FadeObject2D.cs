using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treevel.Common.Components.UIs
{
    public class FadeObject2D : MonoBehaviour
    {
        [SerializeField, Tooltip("fade duration in second")]
        private float _duration = 0.8f;

        private readonly List<CanvasRenderer> _renderers = new List<CanvasRenderer>();

        private void Awake()
        {
            GetComponentsInChildren<CanvasRenderer>(true, _renderers);
            if (!gameObject.activeSelf) {
                _renderers.ForEach(r => r.SetAlpha(0));
            }
        }

        private void OnEnable()
        {
            if (_renderers == null || _renderers.Count == 0) return;

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            // 経過時間
            var elapsed = 0.0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;

                // 透明度計算
                var alpha = Mathf.Lerp(0, 1, elapsed / _duration);
                _renderers.ForEach(r => r.SetAlpha(alpha));
                yield return null;
            }
        }
    }
}

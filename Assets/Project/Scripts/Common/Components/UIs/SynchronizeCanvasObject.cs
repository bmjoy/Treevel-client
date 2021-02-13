using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components.UIs
{
    [DefaultExecutionOrder(1)]
    public class SynchronizeCanvasObject : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;

        /// <summary>
        /// 位置・拡大縮小率を反映させるオブジェクト
        /// </summary>
        /// <returns></returns>
        [SerializeField] private GameObject _synchronizedObject;

        private void Awake()
        {
            _scrollRect.onValueChanged.AsObservable()
                .Subscribe(_ => {
                    _synchronizedObject.transform.localPosition = gameObject.transform.localPosition;
                    _synchronizedObject.transform.localScale = gameObject.transform.localScale;
                })
                .AddTo(this);
        }

        private void OnEnable()
        {
            _synchronizedObject.transform.localPosition = gameObject.transform.localPosition;
            _synchronizedObject.transform.localScale = gameObject.transform.localScale;
        }
    }
}

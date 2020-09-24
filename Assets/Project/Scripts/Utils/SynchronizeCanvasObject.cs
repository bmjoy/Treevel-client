using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Utils
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
            _scrollRect.onValueChanged.AddListener(_ => {
                _synchronizedObject.transform.localPosition = gameObject.transform.localPosition;
                _synchronizedObject.transform.localScale = gameObject.transform.localScale;
            });
        }

        private void OnEnable()
        {
            _synchronizedObject.transform.localPosition = gameObject.transform.localPosition;
            _synchronizedObject.transform.localScale = gameObject.transform.localScale;
        }
    }
}

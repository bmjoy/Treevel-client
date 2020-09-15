using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils
{
    public class SynchronizeObject : MonoBehaviour
    {
        [SerializeField] private GameObject synchronizedObject;

        /// <summary>
        /// 対象のGameObjectの位置・大きさを主とするGameObjectと同じにする
        /// </summary>
        void Update()
        {
            synchronizedObject.transform.localPosition = gameObject.transform.localPosition;
            synchronizedObject.transform.localScale = gameObject.transform.localScale;
        }
    }
}

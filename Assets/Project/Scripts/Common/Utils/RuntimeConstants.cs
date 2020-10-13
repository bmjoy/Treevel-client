using UnityEngine;

namespace Treevel.Common.Utils
{
    public class RuntimeConstants
    {
        /// <summary>
        /// CanvasScalerによる拡大縮小後のキャンバスサイズ
        /// </summary>
        public static class ScaledCanvasSize
        {
            public static readonly Vector2 SIZE_DELTA = GameObject.Find("UIManager/Canvas").GetComponent<RectTransform>().sizeDelta;
        }
    }
}

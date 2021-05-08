using UnityEngine;

namespace Treevel.Common.Utils
{
    public class RuntimeConstants
    {
        /// <summary>
        /// CanvasScalerによる拡大縮小後のキャンバスサイズ
        /// </summary>
        public static readonly Vector2 SCALED_CANVAS_SIZE = GameObject.Find("UIManager/Canvas")?.GetComponent<RectTransform>().sizeDelta ?? new Vector2(Constants.DeviceSize.WIDTH, Constants.DeviceSize.HEIGHT);
    }
}

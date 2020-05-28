using UnityEngine;
using Project.Scripts.Utils.TextUtils;

namespace Project.Scripts.Utils.Definitions
{
    public static class Default
    {
        /// <summary>
        /// デフォルトの言語
        /// </summary>
        public const ELanguage LANGUAGE = ELanguage.Japanese;

        /// <summary>
        /// デフォルトのBGM
        /// </summary>
        public const float BGM_VOLUME = 0.5f;

        /// <summary>
        /// デフォルトのSE
        /// </summary>
        public const float SE_VOLUME = 0.5f;

        /// <summary>
        /// デフォルトのステージ詳細
        /// </summary>
        public const int STAGE_DETAILS = 1;

        /// <summary>
        /// デフォルトのキャンバスの拡大率
        /// </summary>
        public const float CANVAS_SCALE = 1;

        /// <summary>
        /// デフォルトのスクロール位置
        /// </summary>
        /// <returns></returns>
        public static Vector2 SCROLL_POSITION = new Vector2(0, ScaledCanvasSize.SIZE_DELTA.y / 2);
    }
}

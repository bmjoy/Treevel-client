using UnityEngine;
using Project.Scripts.Utils.TextUtils;
using Project.Scripts.UIComponents;

namespace Project.Scripts.Utils.Definitions
{
    public static class Default
    {
        // <summary>
        /// デフォルトの木の解放状態
        /// </summary>
        public const int TREE_RELEASED = 0;

        /// <summary>
        /// デフォルトの木のクリア状態
        /// </summary>
        public const int TREE_CLEARED = 0;

        /// <summary>
        /// デフォルトの道の解放状態
        /// </summary>
        public const int ROAD_RELEASED = 0;

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
        public const float LEVEL_SELECT_CANVAS_SCALE = 1;

        /// <summary>
        /// デフォルトのスクロール位置
        /// </summary>
        /// <returns></returns>
        public static Vector2 LEVEL_SELECT_SCROLL_POSITION
        {
            get {
                var(anchorMin, anchorMax) = SafeAreaPanel.GetSafeAreaAnchor();
                return new Vector2(0, ScaledCanvasSize.SIZE_DELTA.y * (anchorMax.y - anchorMin.y) / 2);
            }
        }
    }
}

using Treevel.Common.Components.UIs;
using Treevel.Common.Entities;
using UnityEngine;

namespace Treevel.Common.Utils
{
    public static class Default
    {
        // <summary>
        /// デフォルトの木の解放状態
        /// </summary>
        public const int TREE_STATE = (int)ETreeState.Unreleased;

        /// <summary>
        /// デフォルトの道の解放状態
        /// </summary>
        public const int ROAD_RELEASED = 0;

        /// <summary>
        /// デフォルトの枝の解放状態
        /// </summary>
        public const int BRANCH_RELEASED = 0;

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
        public static Vector2 LEVEL_SELECT_SCROLL_POSITION {
            get {
                var (anchorMin, anchorMax) = SafeAreaPanel.GetSafeAreaAnchor();
                return new Vector2(0, RuntimeConstants.SCALED_CANVAS_SIZE.y * (anchorMax.y - anchorMin.y) / 2);
            }
        }

        /// <summary>
        /// 起動日数
        /// </summary>
        public const int STARTUP_DAYS = 1;
    }
}

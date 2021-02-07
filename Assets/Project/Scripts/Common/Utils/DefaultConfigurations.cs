using System.Collections.Generic;
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
                return new Vector2(0, RuntimeConstants.ScaledCanvasSize.SIZE_DELTA.y * (anchorMax.y - anchorMin.y) / 2);
            }
        }

        /// <summary>
        /// 各失敗原因に対する失敗回数
        /// </summary>
        public static readonly Dictionary<EFailureReasonType, int> FAILURE_REASON_COUNT = new Dictionary<EFailureReasonType, int> {
            { EFailureReasonType.Others, 0 },
            { EFailureReasonType.Tornado, 0 },
            { EFailureReasonType.Meteorite, 0 },
            { EFailureReasonType.AimingMeteorite, 0 },
            { EFailureReasonType.Thunder, 0 },
            { EFailureReasonType.SolarBeam, 0 },
            { EFailureReasonType.Powder, 0 },
        };

        /// <summary>
        /// 起動日数
        /// </summary>
        public const int STARTUP_DAYS = 1;
    }
}

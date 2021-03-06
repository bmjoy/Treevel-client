using System;
using System.Collections.Generic;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// 季節一覧
    /// </summary>
    public enum EGoalColor
    {
        None,      // 指定なし
        Red,       // 赤
        Pink,      // ピンク
        Orange,    // オレンジ
        Yellow,    // 黄色
        Green,     // 緑
        LightBlue, // 水色
        Blue,      // 青
        Purple,    // 紫
    }

    public static class GoalColorExtension
    {
        public static string GetBottleAddress(this EGoalColor goalColor)
        {
            if (goalColor == EGoalColor.None) throw new Exception("GoalColor should not be None");
            return Constants.Address.NORMAL_BOTTLE_SPRITE_PREFIX + goalColor;
        }

        public static string GetTileAddress(this EGoalColor goalColor)
        {
            if (goalColor == EGoalColor.None) throw new Exception("GoalColor should not be None");
            return Constants.Address.GOAL_TILE_SPRITE_PREFIX + goalColor;
        }

        private static readonly Dictionary<EGoalColor, Color> _MAIN_COLORS = new Dictionary<EGoalColor, Color> {
            { EGoalColor.None, Color.white },
            { EGoalColor.Red, new Color(1f, 0.6f, 0.6f) },
            { EGoalColor.Pink, new Color(1f, 0.6f, 0.95f) },
            { EGoalColor.Orange, new Color(1f, 0.7f, 0.4f) },
            { EGoalColor.Yellow, new Color(1f, 1f, 0.35f) },
            { EGoalColor.Green, new Color(0.5f, 0.95f, 0.45f) },
            { EGoalColor.LightBlue, new Color(0.45f, 0.85f, 1f) },
            { EGoalColor.Blue, new Color(0.5f, 0.6f, 1f) },
            { EGoalColor.Purple, new Color(0.5f, 0.2f, 1f) },
        };

        public static Color GetMainColor(this EGoalColor goalColor)
        {
            return _MAIN_COLORS[goalColor];
        }

        public static Color GetMainColor(this EGoalColor goalColor, float alpha)
        {
            var mainColor = _MAIN_COLORS[goalColor];
            return new Color(mainColor.r, mainColor.g, mainColor.b, alpha);
        }
    }
}

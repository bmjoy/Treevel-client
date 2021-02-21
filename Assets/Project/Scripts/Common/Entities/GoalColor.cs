using System;
using System.Collections.Generic;
using Treevel.Common.Managers;
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
            return Constants.Address.NUMBER_TILE_SPRITE_PREFIX + goalColor;
        }
    }
}

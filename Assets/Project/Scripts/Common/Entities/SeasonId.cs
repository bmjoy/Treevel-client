using System;
using Treevel.Common.Utils;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// 季節一覧
    /// </summary>
    public enum ESeasonId {
        Spring, // 春
        Summer, // 夏
        Autumn, // 秋
        Winter, // 冬
    }

    public static class SeasonIdExtension
    {
        public static string GetSceneName(this ESeasonId seasonId)
        {
            switch (seasonId) {
                case ESeasonId.Spring:
                    return Constants.SceneName.SPRING_STAGE_SELECT_SCENE;
                case ESeasonId.Summer:
                    return Constants.SceneName.SUMMER_STAGE_SELECT_SCENE;
                case ESeasonId.Autumn:
                    return Constants.SceneName.AUTUMN_STAGE_SELECT_SCENE;
                case ESeasonId.Winter:
                    return Constants.SceneName.WINTER_STAGE_SELECT_SCENE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seasonId), seasonId, null);
            }
        }

        public static int GetTreeNum(this ESeasonId seasonId)
        {
            switch (seasonId) {
                case ESeasonId.Spring:
                    return 3;
                case ESeasonId.Summer:
                    return 3;
                case ESeasonId.Autumn:
                    return 3;
                case ESeasonId.Winter:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seasonId), seasonId, null);
            }
        }
    }
}

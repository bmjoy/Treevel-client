using System;
using System.Collections.Generic;
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

        public static ETreeId GetFirstTree(this ESeasonId seasonId)
        {
            switch (seasonId) {
                case ESeasonId.Spring:
                    return ETreeId.Spring_1;
                case ESeasonId.Summer:
                    return ETreeId.Summer_1;
                case ESeasonId.Autumn:
                    return ETreeId.Autumn_1;
                case ESeasonId.Winter:
                    return ETreeId.Winter_1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seasonId), seasonId, null);
            }
        }

        private static readonly Dictionary<ESeasonId, List<ETreeId>> _TREES = new Dictionary<ESeasonId, List<ETreeId>>()
        {
            {ESeasonId.Spring, new List<ETreeId> {ETreeId.Spring_1, ETreeId.Spring_2, ETreeId.Spring_3}},
            {ESeasonId.Summer, new List<ETreeId> {ETreeId.Summer_1, ETreeId.Summer_2, ETreeId.Summer_3}},
            {ESeasonId.Autumn, new List<ETreeId> {ETreeId.Autumn_1, ETreeId.Autumn_2, ETreeId.Autumn_3}},
            {ESeasonId.Winter, new List<ETreeId> {ETreeId.Winter_1, ETreeId.Winter_2, ETreeId.Winter_3}},
        };

        public static List<ETreeId> GetTrees(this ESeasonId seasonId)
        {
            return _TREES[seasonId];
        }
    }
}

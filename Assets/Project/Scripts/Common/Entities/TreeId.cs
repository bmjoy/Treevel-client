using System.Collections.Generic;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeId
    {
        Spring_1 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 1, // 春の木
        Spring_2 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Spring_3 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Summer_1 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 1, // 夏の木
        Summer_2 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Summer_3 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Autumn_1 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 1, // 秋の木
        Autumn_2 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Autumn_3 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Winter_1 = ESeasonId.Winter * Constants.MAX_TREE_NUM_IN_SEASON + 1, // 冬の木
        Winter_2 = ESeasonId.Winter * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Winter_3 = ESeasonId.Winter * Constants.MAX_TREE_NUM_IN_SEASON + 3,
    }

    public static class TreeIdExtension
    {
        public static ESeasonId GetSeasonId(this ETreeId treeId)
        {
            return (ESeasonId)((int)treeId / Constants.MAX_TREE_NUM_IN_SEASON);
        }

        public static string GetTreeIdAsKey(this ETreeId treeId)
        {
            return treeId.ToString().Replace("_", "-");
        }
    }
}

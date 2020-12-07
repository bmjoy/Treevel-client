using System.Collections.Generic;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeId {
        Dummy = 0,  // ダミー
        Spring_1 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 1,   // 春の木
        Spring_2 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Spring_3 = ESeasonId.Spring * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Summer_1 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 1,    // 夏の木
        Summer_2 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Summer_3 = ESeasonId.Summer * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Autumn_1 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 1,     // 秋の木
        Autumn_2 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 2,
        Autumn_3 = ESeasonId.Autumn * Constants.MAX_TREE_NUM_IN_SEASON + 3,
        Winter_1 = ESeasonId.Winter * Constants.MAX_TREE_NUM_IN_SEASON + 1,    // 冬の木
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

    public static class TreeInfo
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        /// TODO: 実際にある木のステージ数で決める
        public static readonly Dictionary<ETreeId, int> NUM = new Dictionary<ETreeId, int>()
        {
            {ETreeId.Dummy, 0},
            {ETreeId.Spring_1, 10},
            {ETreeId.Spring_2, 10},
            {ETreeId.Spring_3, 10},
            {ETreeId.Summer_1, 10},
            {ETreeId.Summer_2, 10},
            {ETreeId.Summer_3, 10},
            {ETreeId.Autumn_1, 10},
            {ETreeId.Autumn_2, 10},
            {ETreeId.Autumn_3, 10},
            {ETreeId.Winter_1, 10},
            {ETreeId.Winter_2, 10},
            {ETreeId.Winter_3, 10},
        };

        public static readonly Dictionary<ETreeId, IClearTreeHandler> CLEAR_HANDLER = new Dictionary<ETreeId, IClearTreeHandler>()
        {
            {ETreeId.Dummy, null},
            {ETreeId.Spring_1, new NumClearTreeHandler(ETreeId.Spring_1, 1)},
            {ETreeId.Spring_2, new NumClearTreeHandler(ETreeId.Spring_2, 1)},
            {ETreeId.Spring_3, new NumClearTreeHandler(ETreeId.Spring_3, 1)},
            {ETreeId.Summer_1, new NumClearTreeHandler(ETreeId.Summer_1, 1)},
            {ETreeId.Summer_2, new NumClearTreeHandler(ETreeId.Summer_3, 1)},
            {ETreeId.Summer_3, new NumClearTreeHandler(ETreeId.Summer_3, 1)},
            {ETreeId.Autumn_1, new NumClearTreeHandler(ETreeId.Autumn_1, 1)},
            {ETreeId.Autumn_2, new NumClearTreeHandler(ETreeId.Autumn_2, 1)},
            {ETreeId.Autumn_3, new NumClearTreeHandler(ETreeId.Autumn_3, 1)},
            {ETreeId.Winter_1, new NumClearTreeHandler(ETreeId.Winter_1, 1)},
            {ETreeId.Winter_2, new NumClearTreeHandler(ETreeId.Winter_2, 1)},
            {ETreeId.Winter_3, new NumClearTreeHandler(ETreeId.Winter_3, 1)},
        };
    }
}

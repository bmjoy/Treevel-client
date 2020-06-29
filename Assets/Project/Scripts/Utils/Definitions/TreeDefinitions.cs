using System.Collections.Generic;
using Project.Scripts.MenuSelectScene.LevelSelect;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeId {
        Spring_1 = 1,   // 春の木
        Spring_2 = 2,
        Spring_3 = 3,
        Summer_1 = 1001,    // 夏の木
        Summer_2 = 1002,
        Summer_3 = 1003,
        Automn_1 = 2001,     // 秋の木
        Automn_2 = 2002,
        Automn_3 = 2003,
        Winter_1 = 3001,    // 冬の木
        Winter_2 = 3002,
        Winter_3 = 3003,
    }

    public static class TreeInfo
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        /// TODO: 実際にある木のステージ数で決める
        public static readonly Dictionary<ETreeId, int> NUM = new Dictionary<ETreeId, int>()
        {
            {ETreeId.Spring_1, 10},
            {ETreeId.Spring_2, 10},
            {ETreeId.Spring_3, 10},
            {ETreeId.Summer_1, 10},
            {ETreeId.Summer_2, 10},
            {ETreeId.Summer_3, 10},
            {ETreeId.Automn_1, 10},
            {ETreeId.Automn_2, 10},
            {ETreeId.Automn_3, 10},
            {ETreeId.Winter_1, 10},
            {ETreeId.Winter_2, 10},
            {ETreeId.Winter_3, 10},
        };

        public static readonly Dictionary<ETreeId, IClearTreeHandler> CLEAR_HANDLER = new Dictionary<ETreeId, IClearTreeHandler>()
        {
            {ETreeId.Spring_1, new NumClearTreeHandler(1)},
            {ETreeId.Spring_2, new NumClearTreeHandler(1)},
            {ETreeId.Spring_3, new NumClearTreeHandler(1)},
            {ETreeId.Summer_1, new NumClearTreeHandler(1)},
            {ETreeId.Summer_2, new NumClearTreeHandler(1)},
            {ETreeId.Summer_3, new NumClearTreeHandler(1)},
            {ETreeId.Automn_1, new NumClearTreeHandler(1)},
            {ETreeId.Automn_2, new NumClearTreeHandler(1)},
            {ETreeId.Automn_3, new NumClearTreeHandler(1)},
            {ETreeId.Winter_1, new NumClearTreeHandler(1)},
            {ETreeId.Winter_2, new NumClearTreeHandler(1)},
            {ETreeId.Winter_3, new NumClearTreeHandler(1)},
        };
    }

    /// <summary>
    /// 木の状態
    /// </summary>
    public enum ETreeState {
        Unreleased = 1,
        Released,
        Cleared,
        Finished
    }
}

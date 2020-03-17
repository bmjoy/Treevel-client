using System.Collections.Generic;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeName {
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
        public static readonly Dictionary<ETreeName, int> NUM = new Dictionary<ETreeName, int>()
        {
            {ETreeName.Spring_1, 10},
            {ETreeName.Spring_2, 10},
            {ETreeName.Spring_3, 10},
            {ETreeName.Summer_1, 10},
            {ETreeName.Summer_2, 10},
            {ETreeName.Summer_3, 10},
            {ETreeName.Automn_1, 10},
            {ETreeName.Automn_2, 10},
            {ETreeName.Automn_3, 10},
            {ETreeName.Winter_1, 10},
            {ETreeName.Winter_2, 10},
            {ETreeName.Winter_3, 10},
        };
    }
}

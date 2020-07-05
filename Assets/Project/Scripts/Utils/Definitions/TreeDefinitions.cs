using System.Collections.Generic;
using Project.Scripts.MenuSelectScene.LevelSelect;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeId {
        Dummy = 0,  // ダミー
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
            {ETreeId.Dummy, 0},
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
            {ETreeId.Dummy, null},
            {ETreeId.Spring_1, new NumClearTreeHandler(ETreeId.Spring_1, 1)},
            {ETreeId.Spring_2, new NumClearTreeHandler(ETreeId.Spring_2, 1)},
            {ETreeId.Spring_3, new NumClearTreeHandler(ETreeId.Spring_3, 1)},
            {ETreeId.Summer_1, new NumClearTreeHandler(ETreeId.Summer_1, 1)},
            {ETreeId.Summer_2, new NumClearTreeHandler(ETreeId.Summer_3, 1)},
            {ETreeId.Summer_3, new NumClearTreeHandler(ETreeId.Summer_3, 1)},
            {ETreeId.Automn_1, new NumClearTreeHandler(ETreeId.Automn_1, 1)},
            {ETreeId.Automn_2, new NumClearTreeHandler(ETreeId.Automn_2, 1)},
            {ETreeId.Automn_3, new NumClearTreeHandler(ETreeId.Automn_3, 1)},
            {ETreeId.Winter_1, new NumClearTreeHandler(ETreeId.Winter_1, 1)},
            {ETreeId.Winter_2, new NumClearTreeHandler(ETreeId.Winter_2, 1)},
            {ETreeId.Winter_3, new NumClearTreeHandler(ETreeId.Winter_3, 1)},
        };
    }

    /// <summary>
    /// 木の状態
    /// </summary>
    public enum ETreeState {
        /// <summary>
        /// 非解放状態
        /// </summary>
        Unreleased = 1,
        /// <summary>
        /// 解放状態
        /// </summary>
        Released,
        /// <summary>
        /// クリア状態( ⊆ 解放状態)
        /// </summary>
        Cleared,
        /// <summary>
        /// 全ステージクリア状態( ⊆ クリア状態 ⊆ 解放状態)
        /// </summary>
        AllCleared
    }
}

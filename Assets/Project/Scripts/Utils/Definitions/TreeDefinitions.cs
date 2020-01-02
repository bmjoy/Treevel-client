using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 木一覧
    /// </summary>
    public enum ETreeName {
        CherryBlossom = 1, // 桜
        Plum = 2,          // 梅
        Zelkova = 1001,    // ケヤキ
        Momiji = 2001,     // モミジ
        Ceder = 3001,      // 杉
    }

    public static class TreeInfo
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        /// TODO: 実際にある木のステージ数で決める
        public static readonly Dictionary<ETreeName, int> NUM = new Dictionary<ETreeName, int>()
        {
            {ETreeName.CherryBlossom, 10},
            {ETreeName.Plum, 10},
            {ETreeName.Zelkova, 10},
            {ETreeName.Momiji, 10},
            {ETreeName.Ceder, 10},
        };
    }
}

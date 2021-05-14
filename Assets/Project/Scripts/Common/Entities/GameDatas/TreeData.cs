using System;
using System.Collections.Generic;
using UnityEngine;

namespace Treevel.Common.Entities.GameDatas
{
    [CreateAssetMenu(fileName = "tree.asset", menuName = "Tree")]
    public class TreeData : ScriptableObject
    {
        /// <summary>
        /// 木のID
        /// </summary>
        public ETreeId id;

        /// <summary>
        /// 木を解放するための条件
        /// </summary>
        public List<TreeConstraint> constraintTree;

        /// <summary>
        /// この木が所有するステージのリスト
        /// (GameDataManagerでStageDataを基づいて計算して与える）
        /// </summary>
        [NonSerialized] public List<StageData> stages;
    }

    /// <summary>
    /// 木を解放するための条件
    /// </summary>
    [Serializable]
    public class TreeConstraint
    {
        public ETreeId treeId;
        public int clearStageNumber;
    }
}

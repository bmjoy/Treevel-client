using System;
using System.Collections.Generic;
using Treevel.Modules.MenuSelectScene.LevelSelect;
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
        public List<ETreeId> constraintTrees;

        /// <summary>
        /// 木がクリアしたかの判定を行うハンドラーの種別
        /// </summary>
        [SerializeField] private EClearTreeHandlerType _clearTreeHandlerType;

        /// <summary>
        /// NumClearTreeHandler用パラメータ
        /// </summary>
        [SerializeField] private int _clearStageNumber;

        /// <summary>
        /// クリア判定ハンドラーのインスタンスを取得
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IClearTreeHandler GetClearTreeHandler()
        {
            return _clearTreeHandler ??= _clearTreeHandlerType switch {
                EClearTreeHandlerType.NumClear => new NumClearTreeHandler(id, _clearStageNumber),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
        private IClearTreeHandler _clearTreeHandler;

        /// <summary>
        /// この木が所有するステージのリスト
        /// (GameDataManagerでStageDataを基づいて計算して与える）
        /// </summary>
        [NonSerialized] public List<StageData> stages;
    }
}

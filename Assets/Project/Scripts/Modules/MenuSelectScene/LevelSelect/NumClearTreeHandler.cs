using System;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// クリアしているステージの割合がクリア条件
    /// </summary>
    public class NumClearTreeHandler : IClearTreeHandler
    {
        /// <summary>
        /// 木のID
        /// </summary>
        private readonly ETreeId _treeId;

        /// <summary>
        /// クリアに必要なステージ数
        /// </summary>
        private readonly int _clearNumThreshold;

        /// <summary>
        /// 木に存在するステージ数
        /// </summary>
        private readonly int _stageNum;

        /// <summary>
        /// クリアに必要なステージ数を設定するコンストラクタ
        /// </summary>
        /// <param name="treeId"> 木のID(ステージ数を取得) </param>
        /// <param name="clearThreshold"> クリアに必要なステージ数 </param>
        public NumClearTreeHandler(ETreeId treeId, int clearThreshold)
        {
            _treeId = treeId;
            _clearNumThreshold = clearThreshold;
            _stageNum = GameDataManager.GetTreeData(treeId).stages.Count;
            if (clearThreshold < 1) {
                throw new Exception($"clearThreshold(={clearThreshold}) must not be less than 1");
            }

            if (clearThreshold > _stageNum) {
                throw new Exception($"clearThreshold(={clearThreshold}) must not be larger than the number of stages");
            }
        }

        /// <summary>
        /// ステージのクリア数に応じた木の状態を取得する
        /// </summary>
        /// <returns> 木の状態 </returns>
        public ETreeState GetTreeState()
        {
            var clearStageNum = StageRecordService.Instance.Get(_treeId)
                .Count(stageRecord => stageRecord.IsCleared);

            // クリア数に応じた木の状態を返す
            if (clearStageNum == _stageNum) {
                return ETreeState.AllCleared;
            } else if (clearStageNum >= _clearNumThreshold) {
                return ETreeState.Cleared;
            } else {
                return ETreeState.Released;
            }
        }
    }
}

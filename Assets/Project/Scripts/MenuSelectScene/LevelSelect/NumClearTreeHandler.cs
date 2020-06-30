using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Linq;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// クリアしているステージの割合がクリア条件
    /// </summary>
    public class NumClearTreeHandler : IClearTreeHandler
    {
        private ETreeId _treeId;
        private int _clearNumThreshold;
        private int _stageNum;

        public NumClearTreeHandler(ETreeId treeId, int clearThreshold)
        {
            _treeId = treeId;
            _clearNumThreshold = clearThreshold;
            _stageNum = TreeInfo.NUM[_treeId];
            if (clearThreshold < 1) {
                throw new Exception("clearThreshold must not be less than 1");
            } else if (clearThreshold > _stageNum) {
                throw new Exception("clearThreshold must not be larger than the number of stages");
            }
        }

        public ETreeState IsClear()
        {
            var clearStageNum = Enumerable.Range(1, _stageNum).Count(s => StageStatus.Get(_treeId, s).cleared);

            // クリア数に応じた木の状態を返す
            if (clearStageNum == _stageNum) {
                return ETreeState.Finished;
            } else if (clearStageNum >= _clearNumThreshold) {
                return ETreeState.Cleared;
            } else {
                return ETreeState.Released;
            }
        }
    }
}

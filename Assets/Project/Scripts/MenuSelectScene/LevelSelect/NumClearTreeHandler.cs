using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// クリアしているステージの割合がクリア条件
    /// </summary>
    public class NumClearTreeHandler : IClearTreeHandler
    {
        private int _clearNumThreshold;

        public NumClearTreeHandler(int clearThreshold) {
            _clearNumThreshold = clearThreshold;
        }

        public ETreeState IsClear(ETreeId treeId) {
            var stageNum = TreeInfo.NUM[treeId];
            var clearStageNum = 0;
            for (var stageNumber = 1; stageNumber <= stageNum; stageNumber++) {
                clearStageNum += StageStatus.Get(treeId, stageNumber).cleared ? 1 : 0;
            }

            // クリア数に応じた木の状態を返す
            if(clearStageNum == stageNum)
            {
                return ETreeState.Finished;
            } else if (clearStageNum >= _clearNumThreshold) {
                return ETreeState.Cleared;
            } else {
                return ETreeState.Released;
            }
        }
    }
}

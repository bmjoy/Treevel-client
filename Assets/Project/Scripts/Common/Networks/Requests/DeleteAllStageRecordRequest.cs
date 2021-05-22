using System.Linq;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;

namespace Treevel.Common.Networks.Requests
{
    public class DeleteAllStageRecordRequest : DeleteListServerRequestBase
    {
        public DeleteAllStageRecordRequest()
        {
            // 遊んだことがあるステージのみ削除
            keys = StageRecordService.Instance.Get()
                .Where(stageRecord => stageRecord.challengeNum > 0)
                .Select(stageRecord => StageData.EncodeStageIdKey(stageRecord.treeId, stageRecord.stageNumber));
        }
    }
}

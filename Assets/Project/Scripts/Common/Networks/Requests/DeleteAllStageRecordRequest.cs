using System.Linq;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;

namespace Treevel.Common.Networks.Requests
{
    public class DeleteAllStageRecordRequest : DeleteServerRequestBase
    {
        public DeleteAllStageRecordRequest()
        {
            // 遊んだことがあるステージのみ削除
            keys = StageRecordService.Instance.Get()
                .Where(s => s.challengeNum > 0)
                .Select(s => StageData.EncodeStageIdKey(s.treeId, s.stageNumber));
        }
    }
}

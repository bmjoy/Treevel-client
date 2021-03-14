using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;

namespace Treevel.Common.Networks.Requests
{
    public class GetStageStatusRequest : GetServerRequestBase<StageStatus>
    {
        public GetStageStatusRequest(ETreeId treeId, int stageNumber)
        {
            key = StageData.EncodeStageIdKey(treeId, stageNumber);
        }
    }
}

using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;

namespace Treevel.Common.Networks.Requests
{
    public class UpdateStageStatusRequest : UpdateServerRequestBase<StageStatus>
    {
        public UpdateStageStatusRequest(ETreeId treeId, int stageNumber, StageStatus data)
        {
            key = StageData.EncodeStageIdKey(treeId, stageNumber);
            this.data = data;
        }
    }
}

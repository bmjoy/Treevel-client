using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;

namespace Treevel.Common.Networks.Requests
{
    public class UpdateStageRecordRequest : UpdateServerRequestBase<StageRecord>
    {
        public UpdateStageRecordRequest(ETreeId treeId, int stageNumber, StageRecord data)
        {
            key = StageData.EncodeStageIdKey(treeId, stageNumber);
            this.data = data;
        }

        public UpdateStageRecordRequest(string key, StageRecord data)
        {
            this.key = key;
            this.data = data;
        }
    }
}

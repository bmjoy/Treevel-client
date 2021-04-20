using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;

namespace Treevel.Common.Networks.Requests
{
    public class GetAllStageStatusListRequest : GetListServerRequestBase<StageStatus>
    {
        public GetAllStageStatusListRequest()
        {
            keys = GameDataManager.GetAllStages()
                .Select(stage => StageData.EncodeStageIdKey(stage.TreeId, stage.StageNumber));
        }
    }
}

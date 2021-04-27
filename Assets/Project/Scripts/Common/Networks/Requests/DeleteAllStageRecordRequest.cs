using System.Linq;
using Cysharp.Threading.Tasks;
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

        public override async UniTask<bool> Execute()
        {
            var allSuccess = true;
            for (var i = 0 ; i < keys.Count() / 10 ; i++) {
                allSuccess &= await remoteDatabaseService.DeleteDataAsync(keys.Skip(i * 10).Take(10));
            }
            return allSuccess;
        }
    }
}

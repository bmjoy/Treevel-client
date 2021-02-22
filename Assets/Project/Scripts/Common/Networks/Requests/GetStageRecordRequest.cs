using Treevel.Common.Entities;

namespace Treevel.Common.Networks.Requests
{
    /// <summary>
    /// ステージの記録を取得用のリクエスト
    /// </summary>
    public class GetStageRecordRequest : GetServerRequestBase<StageStatus>
    {
        public GetStageRecordRequest(string stageId) : base(stageId) {}
    }
}

using Treevel.Common.Entities;

namespace Treevel.Common.Networks.Requests
{
    /// <summary>
    /// ステージ記録を更新用リクエスト
    /// </summary>
    public class UpdateStageRecordRequest : UpdateServerRequestBase<StageStatus>
    {
        public UpdateStageRecordRequest(string key, StageStatus data) : base(key, data) { }
    }
}

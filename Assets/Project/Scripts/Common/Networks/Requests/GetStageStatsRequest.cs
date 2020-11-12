using Treevel.Common.Networks.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Treevel.Common.Networks.Requests
{
    public class GetStageStatsRequest : GetServerRequest
    {
        public GetStageStatsRequest(string stageId)
        {
            ServerRequest = UnityWebRequest.Get(NetworkService.HOST + "/record/stats/stages/" + stageId);
        }

        public override void SetCache()
        {
            throw new System.NotImplementedException();
        }

        protected override object DeserializeResponse()
        {
            return JsonUtility.FromJson<StageStats>(ServerRequest.downloadHandler.text);
        }

        protected override object GetData_Local()
        {
            throw new System.NotImplementedException();
        }
    }
}

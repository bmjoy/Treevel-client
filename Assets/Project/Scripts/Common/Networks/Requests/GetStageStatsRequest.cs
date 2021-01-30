using System;
using Treevel.Common.Networks.Objects;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

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
            throw new NotImplementedException();
        }

        protected override object DeserializeResponse()
        {
            return JsonUtility.FromJson<StageStats>(ServerRequest.downloadHandler.text);
        }

        protected override object GetData_Local()
        {
            // デバッグ用
            var stageStats = new StageStats(Random.Range(0.0f, 1.0f), 1f, "1-1");

            return stageStats;
        }
    }
}

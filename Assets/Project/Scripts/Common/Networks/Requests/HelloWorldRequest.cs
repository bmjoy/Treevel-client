﻿using System;
using UnityEngine.Networking;

namespace Treevel.Common.Networks.Requests
{
    public class HelloWorldRequest : GetServerRequest
    {
        public HelloWorldRequest()
        {
            ServerRequest = UnityWebRequest.Get(NetworkService.HOST);
        }

        public override void SetCache()
        {
            throw new NotImplementedException();
        }

        protected override object DeserializeResponse()
        {
            if (!IsRemoteDataValid()) return GetData_Local();

            return ServerRequest.downloadHandler.text;
        }

        protected override object GetData_Local()
        {
            return "Hello World By Local";
        }
    }
}

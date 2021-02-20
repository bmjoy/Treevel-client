using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    public class LocalDatabaseService : DatabaseService
    {
        public override UniTask<T> GetData<T>(string key)
        {
            Debug.Log($"Get data for Key:{key} from local database");

            throw new NotImplementedException();
        }

        public override UniTask<bool> UpdateData<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public override UniTask<bool> Login()
        {
            throw new NotImplementedException();
        }
    }
}

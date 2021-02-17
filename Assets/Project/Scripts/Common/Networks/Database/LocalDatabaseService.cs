using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    public class LocalDatabaseService : DatabaseService
    {
        public override UniTask<T> GetData<T>(string key)
        {
            Debug.Log($"Get data for Key:{key} from local database");

            // TODO: Local database implementation
            var source = new UniTaskCompletionSource<T>();
            source.TrySetResult(default);
            return source.Task;
        }

        public override UniTask<bool> UpdateData<T>(string key, T data)
        {
            throw new System.NotImplementedException();
        }

        public override UniTask<bool> Login()
        {
            throw new System.NotImplementedException();
        }
    }
}

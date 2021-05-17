using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks.Database
{
    public interface IDatabaseService
    {
        UniTask<T> GetDataAsync<T>(string key);

        UniTask<IEnumerable<T>> GetListDataAsync<T>(IEnumerable<string> key);

        UniTask<bool> UpdateDataAsync<T>(string key, T data);

        UniTask<bool> LoginAsync();

        UniTask<bool> DeleteDataAsync(string key);

        UniTask<bool> DeleteListDataAsync(IEnumerable<string> keys);
    }

    public class NetworkErrorException : Exception
    {
        public NetworkErrorException() {}
        public NetworkErrorException(string message) : base (message) {}
    }
}

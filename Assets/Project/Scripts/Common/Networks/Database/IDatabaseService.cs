
using System;
using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks.Database
{
    public interface IDatabaseService
    {
        UniTask<T> GetData<T>(string key) where T: new();

        UniTask<bool> UpdateData<T>(string key, T data);

        UniTask<bool> Login();
    }

    public class NetworkErrorException : Exception
    {
        public NetworkErrorException() {}
        public NetworkErrorException(string message) : base (message) {}
    }
}

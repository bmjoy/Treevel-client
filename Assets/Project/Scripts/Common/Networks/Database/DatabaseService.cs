
using System;
using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks.Database
{
    public abstract class DatabaseService
    {
        public abstract UniTask<T> GetData<T>(string key) where T: new();

        public abstract UniTask<bool> UpdateData<T>(string key, T data);

        public abstract UniTask<bool> Login();
    }

    public class NetworkErrorException : Exception
    {
        public NetworkErrorException() {}
        public NetworkErrorException(string message) : base (message) {}
    }
}

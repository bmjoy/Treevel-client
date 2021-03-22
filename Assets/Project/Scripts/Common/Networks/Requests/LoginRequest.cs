using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks.Requests
{
    /// <summary>
    /// リモートサーバへログイン用のリクエスト
    /// </summary>
    public class LoginRequest : ServerRequestBase<bool>
    {
        public override UniTask<bool> Execute()
        {
            return remoteDatabaseService.LoginAsync();
        }
    }
}

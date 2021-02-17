using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks.Requests
{
    /// <summary>
    /// リモートサーバへログイン用のリクエスト
    /// </summary>
    public class LoginRequest : GetServerRequestBase<bool>
    {
        public LoginRequest() : base("") { }

        public override UniTask<bool> GetData()
        {
            return remoteDatabaseService.Login();
        }
    }
}

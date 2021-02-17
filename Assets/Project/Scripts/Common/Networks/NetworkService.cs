using Cysharp.Threading.Tasks;
using Treevel.Common.Networks.Requests;

namespace Treevel.Common.Networks
{
    public static class NetworkService
    {
        /// <summary>
        /// サーバーと問い合わせするためのヘルパーメソッド
        /// </summary>
        /// <param name="request"> `ServerRequestBase`を継承したリクエストのインスタンス </param>
        public static UniTask<T> Execute<T>(ServerRequestBase<T> request)
        {
            return request.Execute();
        }
    }
}

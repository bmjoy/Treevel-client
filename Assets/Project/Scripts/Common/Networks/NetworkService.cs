using Cysharp.Threading.Tasks;
using Treevel.Common.Networks.Requests;

namespace Treevel.Common.Networks
{
    public static class NetworkService
    {
        /// <summary>
        /// データ取得用のヘルパーメソッド
        /// </summary>
        /// <param name="command"> `GetServerCommandBasic`を継承したコマンド </param>
        public static UniTask<T> Execute<T>(GetServerRequestBase<T> request)
        {
            return request.GetData();
        }

        /// <summary>
        /// データ更新用ヘルパーメソッド
        /// </summary>
        /// <param name="command"> `UpdateServerCommand` </param>
        /// <returns></returns>
        public static UniTask<bool> Execute<T>(UpdateServerRequestBase<T> request)
        {
            return request.Update();
        }
    }
}

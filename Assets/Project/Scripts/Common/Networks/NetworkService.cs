using Cysharp.Threading.Tasks;

namespace Treevel.Common.Networks
{
    public static class NetworkService
    {
        #if DEV // ローカルデバッグ用サーバーIP
        private const string _serverIp = "localhost";
        #elif PROD // TODO 本番サーバーIP
        private const string _serverIp = "";
        #else // ERROR
        private const string _serverIp = "";
        #endif

        #if DEV // ローカルデバッグ用サーバーポート番号
        private const string _serverPort = "8080";
        #elif PROD // TODO 本番サーバーポート番号
        private const string _serverPort = "";
        #else // ERROR
        private const string _serverPort = "";
        #endif

        /// <summary>
        /// APIサーバーの接続口
        /// </summary>
        public static readonly string HOST = $"{_serverIp}:{_serverPort}";

        /// <summary>
        /// データ取得用のヘルパーメソッド
        /// </summary>
        /// <param name="command"> `GetServerCommandBasic`を継承したコマンド </param>
        public static async UniTask<object> Execute(GetServerRequest command)
        {
            return await command.GetData();
        }

        /// <summary>
        /// データ更新用ヘルパーメソッド
        /// </summary>
        /// <param name="command"> `UpdateServerCommand` </param>
        /// <returns></returns>
        public static async UniTask<object> Execute(UpdateServerRequest command)
        {
            return await command.Update();
        }
    }
}

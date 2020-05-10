
using System;

namespace Project.Scripts.Networks
{
    public static class NetworkService
    {
        #if DEVELOPE
        // ローカルデバッグ用サーバーIP
        private static readonly string _serverIp = "localhost";
        #else
        // TODO 本番サーバーIP
        private static readonly string _serverIp = "";
        #endif

        #if DEVELOPE
        // ローカルデバッグ用サーバーポート番号
        private static readonly string _serverPort = "8080";
        #else
        // TODO 本番サーバーポート番号
        private static readonly string _serverPort = "";
        #endif

        /// <summary>
        /// APIサーバーの接続口
        /// </summary>
        public static readonly string HOST = $"{_serverIp}:{_serverPort}";

        /// <summary>
        /// データ取得用のヘルパーメソッド
        /// </summary>
        /// <param name="command"> `GetServerCommandBasic`を継承したコマンド </param>
        /// <param name="callback"> データ取得後実行するアクション </param>
        static public async void Execute(GetServerRequest command, Action<object> callback = null)
        {
            var data = await command.GetData();
            callback?.Invoke(data);
        }

        /// <summary>
        /// データ更新用ヘルパーメソッド
        /// </summary>
        /// <param name="command"> `UpdateServerCommand` </param>
        /// <param name="callback"></param>
        /// <returns></returns>
        static public async void Execute(UpdateServerRequest command, Action<bool> callback = null)
        {
            var success = await command.Update();
            callback?.Invoke(success);
        }
    }
}

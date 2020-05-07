
using System;
using Project.Scripts.Utils.NetworkUtil;

namespace Project.Scripts.Utils
{
    public static class NetworkService
    {
        #if UNITY_EDITOR
        // ローカルデバッグ用サーバーIP
        private static readonly string _serverIp = "localhost";
        #else
        // TODO 本番サーバーIP
        private static readonly string _serverIp = "";
        #endif

        #if UNITY_EDITOR
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
        static public async void Execute(GetServerCommand command, Action<object> callback = null)
        {
            var data = await command.GetData();
            callback?.Invoke(data);
        }
    }
}

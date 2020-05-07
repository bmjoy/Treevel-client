using System.Threading.Tasks;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;
using UnityEngine.Networking;

namespace Project.Scripts.Utils.NetworkUtil
{
    public interface IServerCommand
    {
        UnityWebRequest ServerRequest {
            get;
        }
        void SetCache();
    }

    /// <summary>
    /// データ取得用コマンド（DBでSELECT文使う場合）
    /// </summary>
    public abstract class GetServerCommandBasic : IServerCommand
    {
        public UnityWebRequest ServerRequest
        {
            get;
            protected set;
        }

        ~GetServerCommandBasic()
        {
            ServerRequest?.Dispose();
        }

        /// <summary>
        /// サーバーから取得したデータをキャッシュに保存する
        /// </summary>
        public abstract void SetCache();

        public async Task<object> GetData()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                return GetData_Local();
            } else {
                return await GetData_Remote();
            }
        }

        protected async Task<object> GetData_Remote()
        {
            // TODO Show Progress bar
            await ServerRequest.SendWebRequest();

            return DeserializeResponse();
        }

        protected abstract object GetData_Local();

        protected bool IsRemoteDataValid()
        {
            return ServerRequest.isDone && !ServerRequest.isNetworkError && !ServerRequest.isHttpError;
        }

        /// <summary>
        /// サーバからもらったレスポンスをコマンド毎に必要な形に整形する
        /// </summary>
        /// <returns>呼び出し側が取得したいデータ</returns>
        protected abstract object DeserializeResponse();
    }

    /// <summary>
    /// データ更新用コマンド(DBでINSERT/UPDATE文を使う場合)
    /// </summary>
    public abstract class UpdateServerCommand : IServerCommand
    {
        public UnityWebRequest ServerRequest
        {
            get;
            protected set;
        }

        ~UpdateServerCommand()
        {
            ServerRequest?.Dispose();
        }

        public abstract void SetCache();
    }
}
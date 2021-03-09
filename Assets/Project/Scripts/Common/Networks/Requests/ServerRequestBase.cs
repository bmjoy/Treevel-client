using Cysharp.Threading.Tasks;
using PlayFab;
using Treevel.Common.Networks.Database;

namespace Treevel.Common.Networks.Requests
{
    public abstract class ServerRequestBase<T>
    {
        /// <summary>
        /// リモートサーバへ問い合わせするためのサービスクラスのインスタンス
        /// </summary>
        protected static DatabaseService remoteDatabaseService = new PlayFabDatabaseService();

        /// <summary>
        /// ローカルサーバへ問い合わせするためのサービスクラスのインスタンス
        /// </summary>
        protected static DatabaseService localDatabaseService = new PlayerPrefsDatabaseService();

        /// <summary>
        /// リクエストを実行
        /// </summary>
        /// <returns>機能に応じて返す型を定義</returns>
        public abstract UniTask<T> Execute();
    }

    public abstract class GetServerRequestBase<TResult> : ServerRequestBase<TResult> where TResult: new()
    {
        protected string key;

        public override async UniTask<TResult> Execute()
        {
            // TODO: リクエスト数を減らせるようになったら、リモートにもアクセスするようにする
            return await localDatabaseService.GetData<TResult>(key);
        }
    }

    public abstract class UpdateServerRequestBase<TDataType> : ServerRequestBase<bool>
    {
        protected string key;
        protected TDataType data;


        public override async UniTask<bool> Execute()
        {
            // TODO: リクエスト数を減らせるようになったら、リモートにもアクセスするようにする
            return await localDatabaseService.UpdateData(key, data);
        }
    }
}

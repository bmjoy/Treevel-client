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
        protected static DatabaseService localDatabaseService = new LocalDatabaseService();

        /// <summary>
        /// リクエストを実行
        /// </summary>
        /// <returns>機能に応じて返す型を定義</returns>
        public abstract UniTask<T> Execute();
    }

    public abstract class GetServerRequestBase<TResult> : ServerRequestBase<TResult>
    {
        protected string key;

        protected GetServerRequestBase(string key)
        {
            this.key = key;
        }

        public override async UniTask<TResult> Execute()
        {
            try {
                // リモートサーバから取得
                return await remoteDatabaseService.GetData<TResult>(key);;
            } catch (PlayFabException e) {
                // リモートサーバから取得失敗
                return await localDatabaseService.GetData<TResult>(key);;
            }
        }
    }

    public abstract class UpdateServerRequestBase<TDataType> : ServerRequestBase<bool>
    {
        protected string key;
        protected TDataType data;

        protected UpdateServerRequestBase(string key, TDataType data)
        {
            this.key = key;
            this.data = data;
        }

        public override async UniTask<bool> Execute()
        {
            try {
                // リモートサーバに送信
                return await remoteDatabaseService.UpdateData(key, data);;
            } catch (PlayFabException e) {
                // リモートサーバから取得失敗
                return await localDatabaseService.UpdateData(key, data);
            }
        }
    }
}

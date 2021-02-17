using Cysharp.Threading.Tasks;
using PlayFab;
using Treevel.Common.Networks.Database;

namespace Treevel.Common.Networks.Requests
{
    public abstract class ServerRequestBase
    {
        /// <summary>
        /// リモートサーバへ問い合わせするためのサービスクラスのインスタンス
        /// </summary>
        protected static DatabaseService remoteDatabaseService = new PlayFabDatabaseService();

        /// <summary>
        /// ローカルサーバへ問い合わせするためのサービクラスのインスタンス
        /// </summary>
        protected static DatabaseService localDatabaseService = new LocalDatabaseService();
    }

    public abstract class GetServerRequestBase<TResult> : ServerRequestBase
    {
        protected string key;

        protected GetServerRequestBase(string key)
        {
            this.key = key;
        }

        public virtual async UniTask<TResult> GetData()
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

    public abstract class UpdateServerRequestBase<TDataType> : ServerRequestBase
    {
        protected string key;
        protected TDataType data;

        protected UpdateServerRequestBase(string key, TDataType data)
        {
            this.key = key;
            this.data = data;
        }

        public virtual async UniTask<bool> Update()
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

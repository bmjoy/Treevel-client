using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Networks.Database;

namespace Treevel.Common.Networks.Requests
{
    public abstract class ServerRequestBase<T>
    {
        /// <summary>
        /// リモートサーバへ問い合わせするためのサービスクラスのインスタンス
        /// </summary>
        protected static IDatabaseService remoteDatabaseService = new PlayFabDatabaseService();

        /// <summary>
        /// リクエストを実行
        /// </summary>
        /// <returns>機能に応じて返す型を定義</returns>
        public abstract UniTask<T> Execute();
    }

    // 今は使っていないが、今後使う可能性があるので、残しておく
    public abstract class GetServerRequestBase<TResult> : ServerRequestBase<TResult>
    {
        protected string key;

        public override async UniTask<TResult> Execute()
        {
            return await remoteDatabaseService.GetDataAsync<TResult>(key);
        }
    }

    public abstract class GetListServerRequestBase<T> : ServerRequestBase<IEnumerable<T>>
    {
        protected IEnumerable<string> keys;

        public override async UniTask<IEnumerable<T>> Execute()
        {
            return await remoteDatabaseService.GetListDataAsync<T>(keys);
        }
    }

    public abstract class UpdateServerRequestBase<TDataType> : ServerRequestBase<bool>
    {
        protected string key;

        protected TDataType data;

        public override async UniTask<bool> Execute()
        {
            return await remoteDatabaseService.UpdateDataAsync(key, data);
        }
    }

    public abstract class DeleteServerRequestBase : ServerRequestBase<bool>
    {
        protected string key;

        public override async UniTask<bool> Execute()
        {
            return await remoteDatabaseService.DeleteDataAsync(key);
        }
    }

    public abstract class DeleteListServerRequestBase : ServerRequestBase<bool>
    {
        protected IEnumerable<string> keys;

        public override async UniTask<bool> Execute()
        {
            var allSuccess = true;
            // PlayFabの仕様上一回のリクエストにつき10個の値しか消せないので分けてリクエストを送る
            for (var i = 0 ; i <= keys.Count() / 10 ; i++) {
                allSuccess &= await remoteDatabaseService.DeleteListDataAsync(keys.Skip(i * 10).Take(10));
            }
            return allSuccess;
        }
    }
}

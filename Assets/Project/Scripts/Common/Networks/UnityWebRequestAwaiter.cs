using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Treevel.Common.Networks
{
    /// <summary>
    /// UnityWebRequestをawait可能にするための拡張
    /// 参考リンク：https://qiita.com/satotin/items/579fa3b9da0ad0d899e8
    /// </summary>
    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation _asyncOp;
        private Action _continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            _asyncOp = asyncOp;
            asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted => _asyncOp.isDone;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public void GetResult() { }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            _continuation();
        }
    }

    public static class ExtensionMethods
    {
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}

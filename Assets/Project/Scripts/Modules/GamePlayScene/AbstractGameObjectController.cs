using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    public class AbstractGameObjectController : MonoBehaviour
    {
        /// <summary>
        /// 以下を除き、ゲーム終了時に解除したい購読にAddする
        /// - GamePlayDirector.Instance.GameSucceeded
        /// - GamePlayDirector.Instance.GameFailed
        /// </summary>
        protected readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        protected virtual void OnEnable()
        {
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
                .Subscribe(_ => {
                    compositeDisposable.Dispose();
                }).AddTo(this);
        }
    }
}

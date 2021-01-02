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
        /// 購読解除クラス
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

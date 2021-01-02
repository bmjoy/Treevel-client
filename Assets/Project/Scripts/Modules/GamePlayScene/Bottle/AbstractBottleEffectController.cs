using System;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class AbstractBottleEffectController : MonoBehaviour
    {
        /// <summary>
        /// 購読解除クラス
        /// </summary>
        protected CompositeDisposable compositeDisposable = new CompositeDisposable();

        protected void DisposeEvent()
        {
            compositeDisposable.Dispose();
        }
    }
}


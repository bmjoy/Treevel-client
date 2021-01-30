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
        protected readonly CompositeDisposable compositeDisposableOnGameEnd = new CompositeDisposable();

        protected virtual void OnEnable()
        {
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => compositeDisposableOnGameEnd.Dispose()).AddTo(this);
        }
    }
}

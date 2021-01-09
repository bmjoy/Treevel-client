using System.Collections;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public abstract class AbstractGimmickController : MonoBehaviour
    {
        /// <summary>
        /// 警告の表示秒数
        /// </summary>
        [SerializeField] protected float _warningDisplayTime = 1.0f;

        public EGimmickType GimmickType { get; private set; }

        private static short _gimmickId = short.MinValue;

        public virtual void Initialize(GimmickData gimmickData)
        {
            GimmickType = gimmickData.type;

            foreach (var renderer in GetComponentsInChildren<Renderer>()) {
                renderer.sortingOrder = _gimmickId;
            }

            try {
                _gimmickId = checked((short)(_gimmickId + 1));
            } catch (System.OverflowException) {
                _gimmickId = short.MinValue;
            }
        }

        /// <summary>
        /// ギミック発動（初期化の後に呼ぶ）
        /// </summary>
        public abstract IEnumerator Trigger();

        private void OnEnable()
        {
            GamePlayDirector.GameSucceeded += HandleGameSucceeded;
            GamePlayDirector.GameFailed += HandleGameFailed;
        }

        private void OnDisable()
        {
            GamePlayDirector.GameSucceeded -= HandleGameSucceeded;
            GamePlayDirector.GameFailed -= HandleGameFailed;
        }

        protected virtual void HandleGameSucceeded()
        {
            OnEndGame();
        }

        protected virtual void HandleGameFailed()
        {
            OnEndGame();
        }

        protected virtual void OnEndGame() { }
    }
}

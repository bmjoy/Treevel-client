using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public abstract class AbstractGimmickController : MonoBehaviour
    {
        /// <summary>
        /// 警告の表示秒数
        /// </summary>
        [SerializeField] protected float _warningDisplayTime = GimmickWarningParameter.WARNING_DISPLAYED_TIME;

        public EGimmickType GimmickType
        {
            get;
            protected set;
        }

        private static short _gimmickId = short.MinValue;

        public virtual void Initialize(GimmickData gimmickData)
        {
            GetComponent<Renderer>().sortingOrder = _gimmickId;

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
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        protected virtual void OnSucceed()
        {
            OnEndGame();
        }

        protected virtual void OnFail()
        {
            OnEndGame();
        }

        protected virtual void OnEndGame()
        {
        }
    }
}

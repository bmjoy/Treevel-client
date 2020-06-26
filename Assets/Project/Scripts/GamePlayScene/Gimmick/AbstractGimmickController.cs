using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public abstract class AbstractGimmickController : MonoBehaviour
    {
        [SerializeField] protected float _warningDisplayTime = BulletWarningParameter.WARNING_DISPLAYED_TIME;

        public virtual void Initialize(GimmickData gimmickData)
        {
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
            GameEnd();
        }

        protected virtual void OnFail()
        {
            GameEnd();
        }

        protected virtual void GameEnd()
        {
        }
    }
}

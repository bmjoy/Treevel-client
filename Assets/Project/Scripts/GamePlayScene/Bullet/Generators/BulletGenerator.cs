using System;
using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
{
    public abstract class BulletGenerator : MonoBehaviour
    {
        protected GamePlayDirector gamePlayDirector;

        /// <summary>
        /// Generatorの出現する確率
        /// </summary>
        [NonSerialized] public int ratio = BulletGeneratorParameter.INITIAL_RATIO;

        protected virtual void Awake()
        {
            gamePlayDirector = GamePlayDirector.Instance;
        }

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

        /* 実際に1つの銃弾を生成する方法を各銃弾のGenerator毎に実装する */
        public abstract IEnumerator CreateBullet(int bulletId);

        private void OnSucceed()
        {
            GameFinish();
        }

        private void OnFail()
        {
            GameFinish();
        }

        private void GameFinish()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// <see cref="BulletData">BulletDataによる初期化
        /// </summary>
        /// <param name="data"></param>
        public abstract void Initialize(BulletData data);
    }
}

using UnityEngine;
using System.Collections;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public abstract class BulletGenerator : MonoBehaviour
    {
        protected GamePlayDirector gamePlayDirector;

        // ランダムな値を決めるときの各要素の重みの初期値
        private const int INITIAL_RATIO = 100;

        // このGeneratorの出現する重み
        public int ratio = INITIAL_RATIO;

        protected void Awake()
        {
            gamePlayDirector = FindObjectOfType<GamePlayDirector>();
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
    }
}

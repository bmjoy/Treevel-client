using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    /// <summary>
    /// 銃弾の生成時刻、生成する銃弾の種類を管理する
    /// </summary>
    public class BulletGroupController : MonoBehaviour
    {
        private GamePlayDirector gamePlayDirector;

        private BulletGroupGenerator _bulletGroupGenerator;

        /// <summary>
        /// 銃弾生成の開始時刻
        /// </summary>
        private float _appearanceTime;

        /// <summary>
        /// 銃弾生成の時間間隔
        /// </summary>
        private float _interval;

        /// <summary>
        /// 銃弾生成の繰り返しの有無
        /// </summary>
        private bool _loop;

        /// <summary>
        /// 乱数
        /// </summary>
        private System.Random _random;

        /// <summary>
        /// このBulletGroupに属するBulletGeneratorの配列
        /// </summary>
        private List<GameObject> _bulletGenerators;

        /// <summary>
        /// BulletGeneratorの出現割合
        /// </summary>
        private int[] _bulletGeneratorRatio;

        private void Awake()
        {
            _bulletGroupGenerator = BulletGroupGenerator.Instance;
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

        /// <summary>
        /// BulletGroupの初期化
        /// </summary>
        /// <param name="startTime"> ゲームの開始時刻 </param>
        /// <param name="appearanceTime"> 銃弾生成の開始時刻</param>
        /// <param name="interval"> 銃弾生成の時間間隔 </param>
        /// <param name="loop"> 銃弾生成の繰り返しの有無 </param>
        /// <param name="bulletGenerators"> BulletGeneratorの配列 </param>
        public void Initialize(float startTime, float appearanceTime, float interval, bool loop,
            List<GameObject> bulletGenerators)
        {
            _random = new System.Random();
            gamePlayDirector = FindObjectOfType<GamePlayDirector>();
            this._appearanceTime = appearanceTime;
            this._interval = interval;
            this._loop = loop;
            this._bulletGenerators = bulletGenerators;
            _bulletGeneratorRatio = new int[bulletGenerators.Count];
            for (var index = 0; index < bulletGenerators.Count; index++) {
                _bulletGeneratorRatio[index] = bulletGenerators[index].GetComponent<BulletGenerator>().ratio;
            }
        }

        /// <summary>
        /// 銃弾生成時刻を管理し、銃弾を生成する
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateBullets()
        {
            var currentTime = Time.time;
            yield return new WaitForSeconds(_appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
                    (currentTime - _bulletGroupGenerator.startTime));
            var sum = 0;

            do {
                sum++;
                // 出現させる銃弾を決定する
                var index = BulletLibrary.SamplingArrayIndex(_bulletGeneratorRatio);
                var bulletGeneratorScript = _bulletGenerators[index].GetComponent<BulletGenerator>();
                // 銃弾のsortingOrderを管理するIDを決定する
                var nextBulletId = _bulletGroupGenerator.bulletId;
                StartCoroutine(bulletGeneratorScript.CreateBullet(nextBulletId));

                // 作成する銃弾の個数の上限チェック
                try {
                    _bulletGroupGenerator.bulletId = checked((short)(nextBulletId + 1));
                } catch (OverflowException) {
                    _bulletGroupGenerator.bulletId = short.MinValue;
                }

                // 次の銃弾を作成する時刻まで待つ
                currentTime = Time.time;
                yield return new WaitForSeconds(_appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
                        _interval * sum - (currentTime - _bulletGroupGenerator.startTime));
            } while (_loop);
        }

        private void OnFail()
        {
            GameFinish();
        }

        private void OnSucceed()
        {
            GameFinish();
        }

        private void GameFinish()
        {
            // 銃弾グループを削除する
            Destroy(gameObject);
        }
    }
}

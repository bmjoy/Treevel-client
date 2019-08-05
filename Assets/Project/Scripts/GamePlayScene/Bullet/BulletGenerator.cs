﻿using Project.Scripts.Utils.Definitions;
using System;
using System.Collections;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public abstract class BulletGenerator : MonoBehaviour
    {
        protected GamePlayDirector gamePlayDirector;

        // Generatorの出現する確率
        [NonSerialized] public int ratio = BulletGeneratorParameter.INITIAL_RATIO;

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

<<<<<<< HEAD
        /* 実際に1つの銃弾を生成する方法を各銃弾のGenerator毎に実装する */
=======
        /* 配列の初期化を行うメソッド */
        // 必ずライブラリ化する
        protected static int[] SetInitialRatio(int arrayLength)
        {
            var returnArray = new int[arrayLength];
            for (var index = 0; index < arrayLength; index++) {
                returnArray[index] = INITIAL_RATIO;
            }

            return returnArray;
        }

        /* 重みに基づき配列の何番目を選択するかをランダムに決定する(配列の最初であるならば0を返す) */
        // 必ずライブラリ化する
        public static int GetRandomParameter(int[] randomParameters)
        {
            var sumOfRandomParameters = randomParameters.Sum();
            // 1以上重みの総和以下の値をランダムに取得する
            var randomValue = new System.Random().Next(sumOfRandomParameters) + 1;
            var index = 0;
            // 重み配列の最初の要素から順に、ランダムな値から値を引く
            while (randomValue > 0) {
                randomValue -= randomParameters[index];
                index += 1;
            }

            return index - 1;
        }

        /// <summary>
        /// 1つの銃弾の生成を行う
        /// </summary>
        /// <param name="bulletId"> 銃弾の描画順序を決めるID</param>
        /// <returns></returns>
>>>>>>> Add comments in root scripts of bullets
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

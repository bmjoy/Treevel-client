using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class BulletGroupGenerator : SingletonObject<BulletGroupGenerator>
    {
        /// <summary>
        /// 生成された銃弾のID(sortingOrder)
        /// </summary>
        [NonSerialized] public short bulletId;

        /// <summary>
        /// 銃弾グループを制御するcoroutine
        /// </summary>
        private List<IEnumerator> _coroutines;

        // 銃弾グループのprefab
        public GameObject bulletGroupControllerPrefab;

        // 各銃弾のGeneratorのprefab
        [SerializeField] private GameObject _normalCartridgeGeneratorPrefab;
        [SerializeField] private GameObject _turnCartridgeGeneratorPrefab;
        [SerializeField] private GameObject _normalHoleGeneratorPrefab;
        [SerializeField] private GameObject _aimingHoleGeneratorPrefab;

        /// <summary>
        /// ゲームの開始時刻
        /// </summary>
        [NonSerialized] public float startTime;

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
        /// 生成したBulletGroup群のcoroutineを開始する
        /// </summary>
        /// <param name="coroutines"></param>
        public void CreateBulletGroups(List<IEnumerator> coroutines)
        {
            Initialize();

            this._coroutines = coroutines;
            foreach (var coroutine in this._coroutines) StartCoroutine(coroutine);
        }

        /// <summary>
        /// ゲーム開始時およびリトライ時の初期化
        /// </summary>
        private void Initialize()
        {
            bulletId = short.MinValue;
            startTime = Time.time;
        }

        /// <summary>
        /// BulletGroupの生成
        /// </summary>
        /// <param name="appearanceTime"> 銃弾生成の開始時刻</param>
        /// <param name="interval"> 銃弾生成の時間間隔 </param>
        /// <param name="loop"> 銃弾生成の繰り返しの有無 </param>
        /// <param name="bulletGenerators"></param>
        /// <returns></returns>
        public IEnumerator CreateBulletGroup(float appearanceTime, float interval, bool loop,
            List<GameObject> bulletGenerators)
        {
            var bulletGroup = Instantiate(bulletGroupControllerPrefab);
            var bulletGroupScript = bulletGroup.GetComponent<BulletGroupController>();
            bulletGroupScript.Initialize(startTime: startTime, appearanceTime: appearanceTime, interval: interval,
                loop: loop, bulletGenerators: bulletGenerators);
            yield return StartCoroutine(bulletGroupScript.CreateBullets());
        }

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
            // 全てのBulletGroupを停止させる
            StopAllCoroutines();
        }

        /// <summary>
        /// NormalCartridgeのGeneratorを生成する 
        /// 横方向、特定の行を移動するNormalCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合</param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="row"> 銃弾の出現する行 </param>
        /// <returns></returns>
        public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row)
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
            return cartridgeGenerator;
        }

        /// <summary>
        /// NormalCartridgeのGeneratorを生成する 
        /// 縦方向、特定の列を移動するNormalCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="column"> 銃弾の出現する列</param>
        /// <returns></returns>
        public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
            EColumn column)
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
            return cartridgeGenerator;
        }

        /// <summary>
        /// NormalCartridgeのGeneratorを生成する
        /// ランダムな行を移動するNormalCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合</param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="row"> 銃弾の出現する行 </param>
        /// <param name="randomCartridgeDirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> 銃弾の出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み </param>
        /// <returns></returns>
        public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
            int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, randomCartridgeDirection, randomRow,
                randomColumn);
            return cartridgeGenerator;
        }

        /* 縦方向、ランダムな列を移動する銃弾を生成 */
        /// <summary>
        /// NormalCartridgeのGeneratorを生成する
        /// ランダムな列を移動するNormalCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合</param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="column"> 銃弾の出現する列 </param>
        /// <param name="randomCartridgeDirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> null </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み </param>
        /// <returns></returns>
        public GameObject CreateNormalCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
            EColumn column, int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, randomCartridgeDirection, randomRow,
                randomColumn);
            return cartridgeGenerator;
        }

        /// <summary>
        /// TurnCartridgeのGeneratorを生成する
        /// 横方向、特定の行を移動し、特定の場所で特定の方向に曲がるTurnCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="row">銃弾の出現する行 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる列 </param>
        /// <returns></returns>
        public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
            int[] turnDirection = null, int[] turnLine = null)
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row);
            return cartridgeGenerator;
        }

        /// <summary>
        /// TurnCartridgeのGeneratorを生成する
        /// 縦方向、特定の列を移動し、特定の場所で特定の方向に曲がるTurnCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="column">銃弾の出現する列 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる行 </param>
        /// <returns></returns>
        public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
            EColumn column,
            int[] turnDirection = null, int[] turnLine = null)
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column);
            return cartridgeGenerator;
        }

        /// <summary>
        /// TurnCartridgeのGeneratorを生成する
        /// 横方向、ランダムな行を移動し、ランダムな場所でランダムな方向に曲がるTurnCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="row"> 銃弾の移動する行 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる列 </param>
        /// <param name="randomCartridgedirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> 銃弾の出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み</param>
        /// <param name="randomTurnDirections"> 曲がる方向の重み </param>
        /// <param name="randomTurnRow"> 曲がる行の重み </param>
        /// <param name="randomTurnColumn"> 曲がる列の重み</param>
        /// <returns></returns>
        public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
            int[] turnDirection, int[] turnLine, int[] randomCartridgedirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow,
            int[] randomTurnColumn)
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, turnDirection, turnLine,
                randomCartridgedirection, randomRow, randomColumn, randomTurnDirections, randomTurnRow,
                randomTurnColumn);
            return cartridgeGenerator;
        }

        /// <summary>
        /// TurnCartridgeのGeneratorを生成する
        /// 縦方向、ランダムな列を移動し、ランダムな場所でランダムな方向に曲がるTurnCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="column"> 銃弾の移動する列 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる列 </param>
        /// <param name="randomCartridgedirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> null </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み</param>
        /// <param name="randomTurnDirections"> 曲がる方向の重み </param>
        /// <param name="randomTurnRow"> 曲がる行の重み </param>
        /// <param name="randomTurnColumn"> null </param>
        /// <returns></returns>
        public GameObject CreateTurnCartridgeGenerator(int ratio, ECartridgeDirection cartridgeDirection,
            EColumn column,
            int[] turnDirection, int[] turnLine, int[] randomCartridgedirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow,
            int[] randomTurnColumn)
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, turnDirection, turnLine,
                randomCartridgedirection, randomRow, randomColumn, randomTurnDirections, randomTurnRow,
                randomTurnColumn);
            return cartridgeGenerator;
        }

        /// <summary>
        /// NormalHoleのGeneratorを生成する
        /// 特定の行、特定の列を撃つNormalHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="row"> 銃弾が出現する行 </param>
        /// <param name="column"> 銃弾が出現する列 </param>
        /// <returns></returns>
        public GameObject CreateNormalHoleGenerator(int ratio, ERow row, EColumn column)
        {
            var holeGenerator = Instantiate(_normalHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, row, column);
            return holeGenerator;
        }

        /// <summary>
        /// NormalHoleのGeneratorを生成する
        /// ランダムな行、ランダムな列を撃つNormalHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="row"> 銃弾が出現する行 </param>
        /// <param name="column"> 銃弾が出現する列 </param>
        /// <param name="randomRow"> 銃弾が出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾が出現する列の重み </param>
        /// <returns></returns>
        public GameObject CreateNormalHoleGenerator(int ratio, ERow row, EColumn column, int[] randomRow,
            int[] randomColumn)
        {
            var holeGenerator = Instantiate(_normalHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, row, column, randomRow, randomColumn);
            return holeGenerator;
        }

        /// <summary>
        /// AimingHoleのGeneratorを生成する
        /// 特定のNumberPanelの親タイルを撃つAimingHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="aimingPanel"> 銃弾が出現するNumberPanel </param>
        /// <returns></returns>
        public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel = null)
        {
            var holeGenerator = Instantiate(_aimingHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, aimingPanel);
            return holeGenerator;
        }

        /// <summary>
        /// AimingHoleのGeneratorを生成する
        /// ランダムなNumberPanelの親タイルを撃つAimingHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="aimingPanel"> 銃弾が出現するNumberPanel </param>
        /// <param name="randomNumberPanel"> 銃弾が出現するNumberPanelの重み</param>
        /// <returns></returns>
        public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanel, int[] randomNumberPanel)
        {
            var holeGenerator = Instantiate(_aimingHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, aimingPanel, randomNumberPanel);
            return holeGenerator;
        }
    }
}

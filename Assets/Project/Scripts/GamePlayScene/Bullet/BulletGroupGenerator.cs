using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using Project.Scripts.GameDatas;
using System.Linq;
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

        public List<IEnumerator> CreateBulletGroups(ICollection<BulletGroupData> bulletGroupList)
        {
            var coroutines = new List<IEnumerator>();
            foreach (var bulletGroup in bulletGroupList) {
                List<GameObject> bulletList = new List<GameObject>();
                foreach (var bulletData in bulletGroup.bullets) {
                    switch (bulletData.type) {
                        case EBulletType.RandomNormalCartridge:
                        case EBulletType.NormalCartridge:
                            bulletList.Add(this.CreateNormalCartridgeGenerator(
                                    bulletData.ratio,
                                    bulletData.direction,
                                    bulletData.line,
                                    bulletData.randomCartridgeDirection.ToArray(),
                                    bulletData.randomRow.ToArray(),
                                    bulletData.randomColumn.ToArray()
                                ));
                            break;
                        case EBulletType.TurnCartridge:
                            bulletList.Add(this.CreateTurnCartridgeGenerator(
                                    bulletData.ratio,
                                    bulletData.direction,
                                    bulletData.line,
                                    bulletData.turnDirections.Cast<int>().ToArray(),  // map ECartridgeDirection to int
                                    bulletData.turnLines.ToArray()
                                ));
                            break;
                        case EBulletType.RandomTurnCartridge:
                            bulletList.Add(this.CreateRandomTurnCartridgeGenerator(
                                    bulletData.ratio,
                                    bulletData.randomCartridgeDirection.ToArray(),
                                    bulletData.randomRow.ToArray(),
                                    bulletData.randomColumn.ToArray(),
                                    bulletData.randomTurnDirection.ToArray(),
                                    bulletData.randomTurnRow.ToArray(),
                                    bulletData.randomTurnColumn.ToArray()
                                ));
                            break;
                        case EBulletType.NormalHole:
                        case EBulletType.RandomNormalHole:
                            bulletList.Add(this.CreateNormalHoleGenerator(
                                    bulletData.ratio,
                                    bulletData.row,
                                    bulletData.column,
                                    bulletData.randomRow.ToArray(),
                                    bulletData.randomColumn.ToArray()
                                ));
                            break;
                        case EBulletType.AimingHole:
                            bulletList.Add(this.CreateAimingHoleGenerator(
                                    bulletData.ratio,
                                    bulletData.aimingPanels.ToArray()
                                ));
                            break;
                        case EBulletType.RandomAimingHole:
                            bulletList.Add(this.CreateRandomAimingHoleGenerator(
                                    bulletData.ratio,
                                    bulletData.randomNumberPanels.ToArray()
                                ));
                            break;
                    }
                }
                coroutines.Add(CreateBulletGroup(
                        bulletGroup.appearTime,
                        bulletGroup.interval,
                        bulletGroup.loop,
                        bulletList
                    ));
            }
            return coroutines;
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
        /// NormalCartridge、RandomNormalCartridgeのジェネレーターを生成する共通メソッド
        /// <see cref="NormalCartridgeGenerator.Initialize(int, ECartridgeDirection, int, int[], int[], int[])"/>
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="cartridgeDirection"></param>
        /// <param name="line"></param>
        /// <param name="randomCartridgeDirection"></param>
        /// <param name="randomRow"></param>
        /// <param name="randomColumn"></param>
        /// <returns></returns>
        public GameObject CreateNormalCartridgeGenerator(
            int ratio,
            ECartridgeDirection cartridgeDirection,
            int line,
            int[] randomCartridgeDirection,
            int[] randomRow,
            int[] randomColumn
        )
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, line, randomCartridgeDirection, randomRow, randomColumn);
            return cartridgeGenerator;
        }

        public GameObject CreateTurnCartridgeGenerator(
            int ratio,
            ECartridgeDirection cartridgeDirection,
            int line,
            int[] turnDirection,
            int[] turnLine
        )
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, line, turnDirection, turnLine);
            return cartridgeGenerator;
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
        /// ランダムな行(または列)を移動するNormalCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合</param>
        /// <param name="randomCartridgeDirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> 銃弾の出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み </param>
        /// <returns></returns>
        public GameObject CreateRandomNormalCartridgeGenerator(int ratio,
            int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
        {
            var cartridgeGenerator = Instantiate(_normalCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<NormalCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, randomCartridgeDirection, randomRow,
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
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, row, turnDirection, turnLine);
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
            cartridgeGeneratorScript.Initialize(ratio, cartridgeDirection, column, turnDirection, turnLine);
            return cartridgeGenerator;
        }

        /// <summary>
        /// TurnCartridgeのGeneratorを生成する
        /// ランダムな行(または列)を移動し、ランダムな場所でランダムな方向に曲がるTurnCartridge
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="randomCartridgedirection"> 銃弾の移動方向の重み </param>
        /// <param name="randomRow"> 銃弾の出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾の出現する列の重み</param>
        /// <param name="randomTurnDirections"> 曲がる方向の重み </param>
        /// <param name="randomTurnRow"> 曲がる行の重み </param>
        /// <param name="randomTurnColumn"> 曲がる列の重み</param>
        /// <returns></returns>
        public GameObject CreateRandomTurnCartridgeGenerator(int ratio, int[] randomCartridgedirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow, int[] randomTurnColumn)
        {
            var cartridgeGenerator = Instantiate(_turnCartridgeGeneratorPrefab);
            var cartridgeGeneratorScript = cartridgeGenerator.GetComponent<TurnCartridgeGenerator>();
            cartridgeGeneratorScript.Initialize(ratio, randomCartridgedirection, randomRow, randomColumn, randomTurnDirections, randomTurnRow,
                randomTurnColumn);
            return cartridgeGenerator;
        }

        /// <summary>
        /// NormalHole、RandomNormalHoleのジェネレーターを生成する共通メソッド
        /// <see cref="NormalHoleGenerator.Initialize(int, ERow, EColumn, int[], int[])"/>
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="randomRow"></param>
        /// <param name="randomColumn"></param>
        /// <returns></returns>
        public GameObject CreateNormalHoleGenerator(
            int ratio,
            ERow row,
            EColumn column,
            int[] randomRow,
            int[] randomColumn
        )
        {
            var holeGenerator = Instantiate(_normalHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, row, column, randomRow, randomColumn);
            return holeGenerator;
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
        /// <param name="randomRow"> 銃弾が出現する行の重み </param>
        /// <param name="randomColumn"> 銃弾が出現する列の重み </param>
        /// <returns></returns>
        public GameObject CreateRandomNormalHoleGenerator(int ratio, int[] randomRow, int[] randomColumn)
        {
            var holeGenerator = Instantiate(_normalHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<NormalHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, randomRow, randomColumn);
            return holeGenerator;
        }

        /// <summary>
        /// AimingHoleのGeneratorを生成する
        /// 特定のNumberPanelの親タイルを撃つAimingHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="aimingPanels"> 銃弾が出現するNumberPanel </param>
        /// <returns></returns>
        public GameObject CreateAimingHoleGenerator(int ratio, int[] aimingPanels)
        {
            var holeGenerator = Instantiate(_aimingHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
            holeGeneratorScript.Initialize(ratio, aimingPanels);
            return holeGenerator;
        }

        /// <summary>
        /// AimingHoleのGeneratorを生成する
        /// ランダムなNumberPanelの親タイルを撃つAimingHole
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="randomNumberPanels"> 銃弾が出現するNumberPanelの重み</param>
        /// <returns></returns>
        public GameObject CreateRandomAimingHoleGenerator(int ratio, int[] randomNumberPanels)
        {
            var holeGenerator = Instantiate(_aimingHoleGeneratorPrefab);
            var holeGeneratorScript = holeGenerator.GetComponent<AimingHoleGenerator>();
            holeGeneratorScript.InitializeRandom(ratio, randomNumberPanels);
            return holeGenerator;
        }
    }
}

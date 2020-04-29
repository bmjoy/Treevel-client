using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bullet.Controllers;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
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

        /// <summary>
        /// 銃弾グループのprefab
        /// </summary>
        public GameObject bulletGroupControllerPrefab;

        /// <summary>
        /// ゲームの開始時刻
        /// </summary>
        [NonSerialized] public float startTime;

        private readonly Dictionary<EBulletType, string> _prefabAddressableKeys = new Dictionary<EBulletType, string>()
        {
            {EBulletType.NormalCartridge, Address.NORMAL_CARTRIDGE_GENERATOR_PREFAB},
            {EBulletType.RandomNormalCartridge, Address.NORMAL_CARTRIDGE_GENERATOR_PREFAB},
            {EBulletType.TurnCartridge, Address.TURN_CARTRIDGE_GENERATOR_PREFAB},
            {EBulletType.RandomTurnCartridge, Address.TURN_CARTRIDGE_GENERATOR_PREFAB},
            {EBulletType.NormalHole, Address.NORMAL_HOLE_GENERATOR_PREFAB},
            {EBulletType.RandomNormalHole, Address.NORMAL_HOLE_GENERATOR_PREFAB},
            {EBulletType.AimingHole, Address.AIMING_HOLE_GENERATOR_PREFAB},
            {EBulletType.RandomAimingHole, Address.AIMING_HOLE_GENERATOR_PREFAB}
        };

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
        public void FireBulletGroups()
        {
            if (_coroutines == null) {
                Debug.LogError("銃弾はないか作成してない");
                return;
            }

            Initialize();
            _coroutines.ForEach(cr => StartCoroutine(cr));
        }

        /// <summary>
        /// ゲーム開始時およびリトライ時の初期化
        /// </summary>
        private void Initialize()
        {
            bulletId = short.MinValue;
            startTime = Time.time;
        }

        public async void CreateBulletGroups(ICollection<BulletGroupData> bulletGroupList)
        {
            var coroutines = new List<IEnumerator>();
            foreach (var bulletGroup in bulletGroupList) {
                var tasks = bulletGroup.bullets.Select(CreateBulletGenerator);
                var bulletList = await Task.WhenAll(tasks);
                coroutines.Add(CreateBulletGroup(
                        bulletGroup.appearTime,
                        bulletGroup.interval,
                        bulletGroup.loop,
                        bulletList.ToList()
                    ));
            }
            _coroutines = coroutines;
        }

        /// <summary>
        /// BulletGroupの生成
        /// </summary>
        /// <param name="appearanceTime"> 銃弾生成の開始時刻</param>
        /// <param name="interval"> 銃弾生成の時間間隔 </param>
        /// <param name="loop"> 銃弾生成の繰り返しの有無 </param>
        /// <param name="bulletGenerators"></param>
        private IEnumerator CreateBulletGroup(float appearanceTime, float interval, bool loop,
            List<GameObject> bulletGenerators)
        {
            var bulletGroup = Instantiate(bulletGroupControllerPrefab);
            var bulletGroupScript = bulletGroup.GetComponent<BulletGroupController>();
            bulletGroupScript.Initialize(startTime: startTime, appearanceTime: appearanceTime, interval: interval,
                loop: loop, bulletGenerators: bulletGenerators);
            yield return bulletGroupScript.CreateBullets();
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

        private async Task<GameObject> CreateBulletGenerator(BulletData bulletData)
        {
            if (!_prefabAddressableKeys.ContainsKey(bulletData.type)) {
                throw new KeyNotFoundException($"no prefab for type:{bulletData.type}");
            }

            var generatorObject = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[bulletData.type]).Task;
            generatorObject.GetComponent<BulletGenerator>().Initialize(bulletData);

            return generatorObject;
        }
    }
}

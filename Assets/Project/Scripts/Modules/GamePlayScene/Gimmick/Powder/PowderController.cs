using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick.Powder
{
    public class PowderController : GimmickControllerBase
    {
        /// <summary>
        /// 背景上部分のParticleSystem
        /// </summary>
        private ParticleSystem _upperParticleSystem;

        /// <summary>
        /// 背景下部分のParticleSystem
        /// </summary>
        private ParticleSystem _lowerParticleSystem;

        /// <summary>
        /// 各NumberBottle上の堆積Powderギミック
        /// </summary>
        private PiledUpPowderController[] _piledUpPowders;

        private void Awake()
        {
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                Destroy(gameObject);
                SoundManager.Instance.StopSE(ESEKey.Gimmick_Powder);
            }).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            // 背景をセットする
            SetBackground();

            // 堆積Powderギミックを作成する
            InstantiatePiledUpPowderAsync().Forget();
        }

        /// <summary>
        /// 背景画像、背景Particleをセットする
        /// </summary>
        private void SetBackground()
        {
            string particleAddressKey;
            string backgroundAddressKey;
            switch (GamePlayDirector.treeId.GetSeasonId()) {
                case ESeasonId.Spring:
                case ESeasonId.Summer:
                case ESeasonId.Autumn:
                case ESeasonId.Winter:
                    // TODO:季節ごとにSpriteを変更する
                    particleAddressKey = Constants.Address.SAND_POWDER_PARTICLE_MATERIAL;
                    backgroundAddressKey = Constants.Address.SAND_POWDER_BACKGROUND_SPRITE;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var backgroundSprite = AddressableAssetManager.GetAsset<Sprite>(backgroundAddressKey);
            var background = transform.Find("background").GetComponent<SpriteRenderer>();
            background.sprite = backgroundSprite;
            var originalWidth = backgroundSprite.bounds.size.x;
            var originalHeight = backgroundSprite.bounds.size.y;
            transform.localScale = new Vector3(GameWindowController.Instance.GetGameSpaceWidth() / originalWidth,
                                               Constants.WindowSize.HEIGHT / originalHeight);

            // 子オブジェクト
            var upperParticle = transform.Find("UpperParticle").gameObject;
            var lowerParticle = transform.Find("LowerParticle").gameObject;
            // 子オブジェクトのParticleSystem
            _upperParticleSystem = upperParticle.GetComponent<ParticleSystem>();
            _lowerParticleSystem = lowerParticle.GetComponent<ParticleSystem>();
            var particleMaterial = AddressableAssetManager.GetAsset<Material>(particleAddressKey);
            // 子オブジェクトのParticleSystemRenderer
            var upperParticleSystemRenderer = upperParticle.GetComponent<ParticleSystemRenderer>();
            var lowerParticleSystemRenderer = lowerParticle.GetComponent<ParticleSystemRenderer>();
            // particleの画像
            upperParticleSystemRenderer.material = lowerParticleSystemRenderer.material = particleMaterial;
            // particleの位置
            upperParticle.transform.position = new Vector3(GameWindowController.Instance.GetGameSpaceWidth() / 2f,
                                                           Constants.WindowSize.HEIGHT / 2f);
            lowerParticle.transform.position = new Vector3(-GameWindowController.Instance.GetGameSpaceWidth() / 2f,
                                                           -Constants.WindowSize.HEIGHT / 2f);
            // particleの射出角度
            var upperShape = upperParticle.GetComponent<ParticleSystem>().shape;
            var lowerShape = lowerParticle.GetComponent<ParticleSystem>().shape;
            var scaleX = upperShape.scale.x;
            var emissionRadian = Mathf.Asin(scaleX / GameWindowController.Instance.GetGameSpaceWidth());
            upperShape.rotation = new Vector3(0, 0, 90 + emissionRadian * 180f / Mathf.PI);
            lowerShape.rotation = new Vector3(0, 0, 270 + emissionRadian * 180f / Mathf.PI);
            // particleの移動距離
            var length = GameWindowController.Instance.GetGameSpaceWidth() * Mathf.Cos(emissionRadian) +
                         scaleX * 2f * Mathf.Tan(emissionRadian);
            // particleの寿命 : 最も遅い粒子でも画面端まで持続するようにする
            var upperMain = upperParticle.GetComponent<ParticleSystem>().main;
            var lowerMain = lowerParticle.GetComponent<ParticleSystem>().main;
            upperMain.startLifetime = length / upperMain.startSpeed.constantMin;
            lowerMain.startLifetime = length / lowerMain.startSpeed.constantMin;
        }

        public override IEnumerator Trigger()
        {
            // 粒子を発生させる
            _upperParticleSystem.Play();
            _lowerParticleSystem.Play();
            SoundManager.Instance.PlaySE(ESEKey.Gimmick_Powder);
            // 堆積Powderギミックを開始する
            foreach (var piledUpPowder in _piledUpPowders) {
                StartCoroutine(piledUpPowder.Trigger());
            }

            yield return null;
        }

        /// <summary>
        /// 堆積Powderギミックを作成する
        /// </summary>
        /// <returns></returns>
        private UniTask InstantiatePiledUpPowderAsync()
        {
            string address;
            switch (GamePlayDirector.treeId.GetSeasonId()) {
                case ESeasonId.Spring:
                case ESeasonId.Summer:
                case ESeasonId.Autumn:
                case ESeasonId.Winter:
                    // TODO:季節ごとにprefabを変更する
                    address = Constants.Address.SAND_PILED_UP_POWDER_PREFAB;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var bottles = FindObjectsOfType<GoalBottleController>();
            _piledUpPowders = new PiledUpPowderController[bottles.Length];

            var tasks = Enumerable.Range(0, bottles.Length).ToList()
                .Select(i => AddressableAssetManager.Instantiate(address).ToUniTask()
                             .ContinueWith(powderObj => {
                                 var powderController = powderObj.GetComponent<PiledUpPowderController>();
                                 _piledUpPowders[i] = powderController;
                                 powderController.Initialize(bottles[i]);
                             }));

            return UniTask.WhenAll(tasks);
        }
    }
}

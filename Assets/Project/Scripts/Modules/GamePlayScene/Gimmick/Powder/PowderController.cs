using System.Collections;
using System.Collections.Generic;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick.Powder
{
    public class PowderController : AbstractGimmickController
    {
        private ParticleSystem _upperParticleSystem;
        private ParticleSystem _lowerParticleSystem;

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            // 季節ごとのSpriteをセットする
            LoadSprites();
        }

        private void LoadSprites()
        {
            string particleAddressKey;
            string backgroundAddressKey;
            switch (GamePlayDirector.treeId.GetSeasonId()) {
                case ESeasonId.Spring:
                case ESeasonId.Summer:
                case ESeasonId.Autumn:
                case ESeasonId.Winter:
                    // TODO:季節ごとにSpriteを変更する
                    particleAddressKey = Constants.Address.POWDER_SAND_PARTICLE_MATERIAL;
                    backgroundAddressKey = Constants.Address.POWDER_SAND_BACKGROUND_SPRITE;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            var backgroundSprite = AddressableAssetManager.GetAsset<Sprite>(backgroundAddressKey);
            var background = transform.Find("background").GetComponent<SpriteRenderer>();
            background.sprite = backgroundSprite;
            var originalWidth = backgroundSprite.bounds.size.x;
            var originalHeight = backgroundSprite.bounds.size.y;
            transform.localScale = new Vector3(GameWindowController.Instance.GetGameSpaceWidth() / originalWidth, Constants.WindowSize.HEIGHT / originalHeight);

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
            upperParticle.transform.position = new Vector3(GameWindowController.Instance.GetGameSpaceWidth() / 2f, Constants.WindowSize.HEIGHT / 2f);
            lowerParticle.transform.position = new Vector3(-GameWindowController.Instance.GetGameSpaceWidth() / 2f, -Constants.WindowSize.HEIGHT / 2f);
            // particleの射出角度
            var upperShape = upperParticle.GetComponent<ParticleSystem>().shape;
            var lowerShape = lowerParticle.GetComponent<ParticleSystem>().shape;
            var scaleX = upperShape.scale.x;
            var emissionRadian = Mathf.Asin(scaleX / GameWindowController.Instance.GetGameSpaceWidth());
            upperShape.rotation = new Vector3(0, 0, 90 + emissionRadian * 180f / Mathf.PI);
            lowerShape.rotation = new Vector3(0, 0, 270 + emissionRadian * 180f / Mathf.PI);
            // particleの移動距離
            var length = GameWindowController.Instance.GetGameSpaceWidth() * Mathf.Cos(emissionRadian) + scaleX * 2f * Mathf.Tan(emissionRadian);
            // particleの寿命 : 最も遅い粒子でも画面端まで持続するようにする
            var upperMain = upperParticle.GetComponent<ParticleSystem>().main;
            var lowerMain = lowerParticle.GetComponent<ParticleSystem>().main;
            upperMain.startLifetime = length / upperMain.startSpeed.constantMin;
            lowerMain.startLifetime = length / lowerMain.startSpeed.constantMin;
        }

        public override IEnumerator Trigger()
        {
            _upperParticleSystem.Play();
            _lowerParticleSystem.Play();
            yield return null;
        }

        protected override void OnEndGame()
        {
            Destroy(gameObject);
        }
    }
}

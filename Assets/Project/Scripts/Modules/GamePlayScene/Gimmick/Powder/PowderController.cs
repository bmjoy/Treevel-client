using System.Collections;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Modules.GamePlayScene.Gimmick.Powder
{
    public class PowderController : AbstractGimmickController
    {
        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);
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
                    // 季節ごとにSpriteを変更する
                    particleAddressKey = Constants.Address.POWDER_SAND_PARTICLE_SPRITE;
                    backgroundAddressKey = Constants.Address.POWDER_SAND_BACKGROUND_SPRITE;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            var particleSprite = AddressableAssetManager.GetAsset<Sprite>(particleAddressKey);
            var backgroundSprite = AddressableAssetManager.GetAsset<Sprite>(backgroundAddressKey);

            var background = transform.Find("background").GetComponent<SpriteRenderer>();
            background.sprite = backgroundSprite;
            var originalWidth = backgroundSprite.bounds.size.x;
            var originalHeight = backgroundSprite.bounds.size.y;
            transform.localScale = new Vector3(GameWindowController.Instance.GetGameSpaceWidth() / originalWidth, Constants.WindowSize.HEIGHT / originalHeight);
        }

        public override IEnumerator Trigger()
        {
            yield return null;
        }
    }
}

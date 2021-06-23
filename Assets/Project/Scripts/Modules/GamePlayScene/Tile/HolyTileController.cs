using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class HolyTileController : TileControllerBase
    {
        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new HolyTileBottleHandler(this);
        }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileData.number));
            #if UNITY_EDITOR
            name = Constants.TileName.HOLY_TILE;
            #endif
        }

        private sealed class HolyTileBottleHandler : DefaultBottleHandler
        {
            private readonly Animator _animator;

            private static readonly int _ANIMATOR_PARAM_TRIGGER_BOTTLE_ENTER = Animator.StringToHash("BottleEnter");
            private static readonly int _ANIMATOR_PARAM_TRIGGER_BOTTLE_EXIT = Animator.StringToHash("BottleExit");

            internal HolyTileBottleHandler(HolyTileController parent)
            {
                _animator = parent.GetComponent<Animator>();
            }

            public override void OnGameStart(GameObject bottle)
            {
                if (bottle == null) return;
                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<BottleControllerBase>() == null) return;

                // 親ボトルを無敵状態にする
                bottle.GetComponent<BottleControllerBase>().isInvincibleByHoly.Value = true;

                // ボトルが入る演出再生
                _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLE_ENTER);
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // 親ボトルを無敵状態から元に戻す
                bottle.GetComponent<BottleControllerBase>().isInvincibleByHoly.Value = false;

                // ボトルが出る演出再生
                _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BOTTLE_EXIT);
            }
        }
    }
}

using Treevel.Common.Entities.GameDatas;
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
            bottleHandler = new HolyTileBottleHandler();
        }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);

            #if UNITY_EDITOR
            name = Constants.TileName.HOLY_TILE;
            #endif
        }

        private sealed class HolyTileBottleHandler : DefaultBottleHandler
        {
            public override void OnGameStart(GameObject bottle)
            {
                if (bottle == null) return;
                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<BottleControllerBase>() == null) return;

                // 親ボトルを無敵状態にする
                bottle.GetComponent<BottleControllerBase>().isInvincible.Value = true;
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // 親ボトルを無敵状態から元に戻す
                bottle.GetComponent<BottleControllerBase>().isInvincible.Value = false;
            }
        }
    }
}

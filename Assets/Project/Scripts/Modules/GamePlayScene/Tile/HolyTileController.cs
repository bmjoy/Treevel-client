using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class HolyTileController : AbstractTileController
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
                OnBottleEnter(bottle, null);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<AbstractBottleController>() == null) return;

                // 親ボトルを無敵状態にする
                bottle.GetComponent<AbstractBottleController>().isInvincible.Value = true;
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // 親ボトルを無敵状態から元に戻す
                bottle.GetComponent<AbstractBottleController>().isInvincible.Value = false;
            }
        }
    }
}

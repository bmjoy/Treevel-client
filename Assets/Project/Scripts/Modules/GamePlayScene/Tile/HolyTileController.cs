﻿using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class HolyTileController : AbstractTileController
    {
        public override bool RunOnBottleEnterAtInit => true;

        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new HolyTileBottleHandler();
        }

        public override void Initialize(int tileNum)
        {
            base.Initialize(tileNum);

            #if UNITY_EDITOR
            name = Constants.TileName.HOLY_TILE;
            #endif
        }

        private sealed class HolyTileBottleHandler : DefaultBottleHandler
        {
            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<AbstractBottleController>() == null) return;

                // 親ボトルを無敵状態にする
                bottle.GetComponent<AbstractBottleController>().Invincible = true;
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // 親ボトルを無敵状態から元に戻す
                bottle.GetComponent<AbstractBottleController>().Invincible = false;
            }
        }
    }
}

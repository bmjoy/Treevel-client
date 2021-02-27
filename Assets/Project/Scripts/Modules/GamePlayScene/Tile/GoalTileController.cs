using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class GoalTileController : NormalTileController
    {
        /// <summary>
        /// タイルの色
        /// </summary>
        public EGoalColor color { get; private set; }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);
            color = tileData.color;
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(tileData.color.GetTileAddress()));
            #if UNITY_EDITOR
            name = Constants.TileName.GOAL_TILE + tileData.number;
            #endif
        }
    }
}

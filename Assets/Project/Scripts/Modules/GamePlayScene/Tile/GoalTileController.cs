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
        public EGoalColor GoalColor { get; private set; }

        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);
            GoalColor = tileData.goalColor;
            // colorがNoneではないことを保証する
            if (GoalColor == EGoalColor.None) UIManager.Instance.ShowErrorMessage(EErrorCode.InvalidTileColor);
            GetComponent<SpriteRendererUnifier>().SetSprite(AddressableAssetManager.GetAsset<Sprite>(tileData.goalColor.GetTileAddress()));
            #if UNITY_EDITOR
            name = Constants.TileName.GOAL_TILE + tileData.number;
            #endif
        }
    }
}

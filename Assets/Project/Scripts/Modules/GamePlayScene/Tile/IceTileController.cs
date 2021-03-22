using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class IceTileController : TileControllerBase
    {
        public override void Initialize(TileData tileData)
        {
            base.Initialize(tileData);

            #if UNITY_EDITOR
            name = Constants.TileName.ICE_TILE;
            #endif

            bottleHandler = new IceTileBottleHandler(TileNumber);
        }

        private sealed class IceTileBottleHandler : DefaultBottleHandler
        {
            private readonly int _tileNumber;

            public IceTileBottleHandler(int tileNumber)
            {
                _tileNumber = tileNumber;
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                // 方向を持たない場合は何もしない
                if (direction == null) return;

                var dynamicBottleController = bottle.GetComponent<DynamicBottleController>();

                // 移動できるボトルではなかったら何もしない
                if (dynamicBottleController == null) return;

                var x = direction.Value.x;
                var y = direction.Value.y;

                int targetTileNum;

                if (x == 0) {
                    // 縦方向の移動の場合
                    targetTileNum = _tileNumber + 3 * y;
                } else if (y == 0) {
                    // 横方向の移動の場合
                    targetTileNum = _tileNumber + x;
                } else {
                    Debug.LogError("invalid direction");
                    return;
                }

                BoardManager.Instance.Move(dynamicBottleController, targetTileNum, direction.Value);
            }
        }
    }
}

using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class NumberTileController : NormalTileController
    {
        /// <inheritdoc />
        public override void Initialize(Vector2 position, int tileNum)
        {
            base.Initialize(position, tileNum);
            name = TileName.NUMBER_TILE;
        }
    }
}

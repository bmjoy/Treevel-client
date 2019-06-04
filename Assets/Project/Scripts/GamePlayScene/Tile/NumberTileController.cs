using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
	public class NumberTileController : NormalTileController {

		public override void Initialize(Vector2 position, int tileNum)
		{
			base.Initialize(position, tileNum);
			name = TileName.NUMBER_TILE + tileNum;
		}
	}
}

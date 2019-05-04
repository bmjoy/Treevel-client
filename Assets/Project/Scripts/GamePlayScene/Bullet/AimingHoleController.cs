using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class AimingHoleController : NormalHoleController
	{
		public override void Initialize(int row, int column, Vector2 holeWarningPosition)
		{
			transform.position = holeWarningPosition;
			GetRowAndColumn(out row, out column, holeWarningPosition);
			this.row = row;
			this.column = column;
		}
	}
}

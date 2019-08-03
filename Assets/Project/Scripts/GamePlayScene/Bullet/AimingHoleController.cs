using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class AimingHoleController : NormalHoleController
    {
        public void Initialize(Vector2 holeWarningPosition)
        {
            transform.position = holeWarningPosition;
            int row;
            int column;

            // 座標から行と列を取得する
            GetRowAndColumn(out row, out column, holeWarningPosition);
            this.row = row;
            this.column = column;
        }
    }
}

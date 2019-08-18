using UnityEngine;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class AimingHoleController : NormalHoleController
    {
        public void Initialize(Vector2 holeWarningPosition)
        {
            transform.position = holeWarningPosition;
            // 座標から行と列を取得する
            (row, column) = BulletLibrary.GetRowAndColumn(holeWarningPosition);
        }
    }
}

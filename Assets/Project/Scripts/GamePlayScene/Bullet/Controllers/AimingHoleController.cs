using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Controllers
{
    public class AimingHoleController : NormalHoleController
    {
        /// <summary>
        /// 銃弾の座標を警告の座標に設定する
        /// </summary>
        /// <param name="holeWarningPosition"> 警告の座標 </param>
        public void Initialize(Vector2 holeWarningPosition)
        {
            transform.position = new Vector3(holeWarningPosition.x, holeWarningPosition.y, speed);
            // 座標から行と列を取得する
            (row, column) = BulletLibrary.GetRowAndColumn(holeWarningPosition);

            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

﻿using UnityEngine;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class AimingHoleController : NormalHoleController
    {
        /// <summary>
        /// 銃弾の座標を警告の座標に設定する
        /// </summary>
        /// <param name="holeWarningPosition"> 警告の座標 </param>
        public void Initialize(Vector2 holeWarningPosition)
        {
            transform.position = holeWarningPosition;
            // 座標から行と列を取得する
            (row, column) = BulletLibrary.GetRowAndColumn(holeWarningPosition);
        }
    }
}

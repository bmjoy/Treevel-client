using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public class AimingHoleWarningController : NormalHoleWarningController
    {
        /// <summary>
        /// 座標を設定する
        /// </summary>
        /// <param name="aimingBottles"> 撃ち抜くAttackableBottleの番号配列 </param>
        /// <param name="aimingHoleCount"> aimingBottles配列の何番目を参照するか</param>
        public void Initialize(int[] aimingBottles, ref int aimingHoleCount)
        {
            var bottleNum = aimingBottles[((aimingHoleCount - 1) % aimingBottles.Length)];
            // 次の表示位置を求める
            aimingHoleCount += 1;

            var bottle = BottleLibrary.GetBottle(bottleNum);
            // 警告の表示位置をBottleと同じ位置にする
            transform.position = bottle.transform.position;

            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

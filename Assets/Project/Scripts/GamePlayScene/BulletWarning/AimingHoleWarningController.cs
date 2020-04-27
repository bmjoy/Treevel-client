using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public class AimingHoleWarningController : NormalHoleWarningController
    {
        /// <summary>
        /// 座標を設定する
        /// </summary>
        /// <param name="aimingPanels"> 撃ち抜くNumberPanelの番号配列 </param>
        /// <param name="aimingHoleCount"> aimingPanels配列の何番目を参照するか</param>
        public void Initialize(int[] aimingPanels, ref int aimingHoleCount)
        {
            var panelNum = aimingPanels[((aimingHoleCount - 1) % aimingPanels.Length)];
            // 次の表示位置を求める
            aimingHoleCount += 1;

            var panel = BottleLibrary.GetPanel(panelNum);
            // 警告の表示位置をPanelと同じ位置にする
            transform.position = panel.transform.position;

            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}

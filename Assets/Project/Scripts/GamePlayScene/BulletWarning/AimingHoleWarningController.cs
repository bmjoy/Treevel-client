﻿using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public class AimingHoleWarningController : NormalHoleWarningController
    {
        /// <summary>
        /// 座標を設定する
        /// </summary>
        /// <param name="aimingPanel"> 撃ち抜くNumberPanelの番号配列 </param>
        /// <param name="aimingHoleCount"> aimingPanel配列の何番目を参照するか</param>
        public void Initialize(int[] aimingPanel, ref int aimingHoleCount)
        {
            var panelNum = aimingPanel[((aimingHoleCount - 1) % aimingPanel.Length)];
            // 次の表示位置を求める
            aimingHoleCount += 1;

            var panel = PanelLibrary.GetPanel(panelNum);
            // 警告の表示位置をPanelと同じ位置にする
            transform.position = panel.transform.position;
        }
    }
}

using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class AimingHoleWarningController : NormalHoleWarningController
	{
		public void Initialize(BulletInfo bulletInfo)
		{
			// 撃ち抜くPanelを取得する
			var aimingPanel = bulletInfo.GetAimingPanel();
			var count = bulletInfo.GetCount();
			var panelNum = aimingPanel[((count - 1) % aimingPanel.Length)];
			var panel = PanelLibrary.GetPanel(panelNum);
			// 警告の表示位置をPanelと同じ位置にする
			transform.position = panel.transform.position;
			// 次の表示位置を求める
			bulletInfo.SetCount(count + 1);
		}
	}
}

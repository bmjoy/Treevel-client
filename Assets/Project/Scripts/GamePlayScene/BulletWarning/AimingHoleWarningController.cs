using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class AimingHoleWarningController : HoleWarningController
	{
		public void Initialize(Dictionary<string, int[]> additionalInfo)
		{
			// 撃ち抜くPanelを取得する
			var aimingPanel = additionalInfo["AimingPanel"];
			var panelNumber = aimingPanel[0];
			var panel = GameObject.Find("NumberPanel" + panelNumber);
			// 警告の表示位置をPanelと同じ位置にする
			transform.position = panel.transform.position;
			// 次の表示位置を求めるために、配列を数字の若い方へシフトする
			if (aimingPanel.Length != 1)
			{
				int[] newAimingPanel = new int[aimingPanel.Length];
				aimingPanel = aimingPanel.Skip(1).Take(aimingPanel.Length - 1).ToArray();
				System.Array.Copy(aimingPanel, newAimingPanel, aimingPanel.Length - 1);
				newAimingPanel[aimingPanel.Length] = aimingPanel[0];
				additionalInfo.Remove("AimingPanel");
				additionalInfo.Add("AimingPanel", newAimingPanel);
			}
		}
	}
}

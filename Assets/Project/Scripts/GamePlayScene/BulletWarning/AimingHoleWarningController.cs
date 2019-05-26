using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class AimingHoleWarningController : NormalHoleWarningController
	{
		public void Initialize(BulletInfo bulletInfo)
		{
			// 撃ち抜くPanelを取得する
			var aimingPanel = bulletInfo.GetAimingPanel();
			int panelNum;
			if (aimingPanel != null)
			{
				var count = bulletInfo.GetAimingHoleCount();
				panelNum = aimingPanel[((count - 1) % aimingPanel.Length)];
				// 次の表示位置を求める
				bulletInfo.SetAimingHoleCount(count + 1);
			}
			else
			{
				panelNum = bulletInfo.GetNumberPanel();
			}

			var panel = PanelLibrary.GetPanel(panelNum);
			// 警告の表示位置をPanelと同じ位置にする
			transform.position = panel.transform.position;
		}
	}
}

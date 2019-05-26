using System.Reflection;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletInfo
	{
		// TurnCartridgeの曲がる方向
		private int[] turnDirection = null;

		public int[] GetTurnDirection()
		{
			return turnDirection;
		}

		private void SetTurnDirection(int[] turnDirection)
		{
			this.turnDirection = turnDirection;
		}

		// Turncartridgeの曲がる場所
		private int[] turnLine = null;

		public int[] GetTurnLine()
		{
			return turnLine;
		}

		private void SetTurnLine(int[] turnLine)
		{
			this.turnLine = turnLine;
		}

		// AimingHoleの撃ち抜くパネルの番号の配列
		private int[] aimingPanel = null;

		public int[] GetAimingPanel()
		{
			return aimingPanel;
		}

		private void SetAimingPanel(int[] aimingPanel)
		{
			this.aimingPanel = aimingPanel;
		}

		// AimingHoleの撃ち抜くパネルの番号の配列のindex
		private int aimingHoleCount = 0;

		public int GetAimingHoleCount()
		{
			return aimingHoleCount;
		}

		public void SetAimingHoleCount(int aimingHoleCount)
		{
			this.aimingHoleCount = aimingHoleCount;
		}

		{
			return count;
		}

		public void SetCount(int count)
		{
			this.count = count;
		}
	}
}

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

		public void SetTurnDirection(int[] turnDirection)
		{
			this.turnDirection = turnDirection;
		}

		// Turncartridgeの曲がる場所
		private int[] turnLine = null;

		public int[] GetTurnLine()
		{
			return turnLine;
		}

		public void SetTurnLine(int[] urnLine)
		{
			this.turnLine = turnLine;
		}

		// AimingHoleの撃ち抜くパネルの番号の配列
		private int[] aimingPanel = null;

		public int[] GetAimingPanel()
		{
			return aimingPanel;
		}

		public void SetAimingPanel(int[] aimingPanel)
		{
			this.aimingPanel = aimingPanel;
		}

		// AimingHoleの撃ち抜くパネルの番号の配列のindex
		// 銃弾毎に、用途を変える可能性あり
		private int count = 0;

		public int GetCount()
		{
			return count;
		}

		public void SetCount(int count)
		{
			this.count = count;
		}
	}
}

using System.Reflection;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletInfo
	{
		// 配列の初期化を行うメソッド
		private static int[] SetInitialRate(int arrayLength)
		{
			var returnArray = new int[arrayLength];
			for (var index = 0; index < arrayLength; index++)
			{
				returnArray[index] = INITIAL_RATE;
			}

			return returnArray;
		}

		// 重みに基づき配列の何番目を選択するかを決定する(配列の最初は1番目)
		private int GetRandomParameter(int[] randomParameters, int sumOfRandomParameters)
		{
			var randomValue = random.Next(sumOfRandomParameters) + 1;
			var index = 0;
			while (randomValue > 0)
			{
				randomValue -= randomParameters[index];
				index += 1;
			}

			return index;
		}

		// int[]配列を更新し、その配列の要素和を求める
		private void SetRandomParameter(out int[] randomParameters, out int sumOfRandomParameters,
			int[] newRandomParameters)
		{
			randomParameters = newRandomParameters;
			sumOfRandomParameters = 0;
			foreach (var randomParameter in randomParameters)
			{
				sumOfRandomParameters += randomParameter;
			}
		}
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

using System;
using Project.Scripts.Utils.Definitions;

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

		/* 配列の初期化を行うメソッド */
		private static int[] SetInitialRate(int arrayLength)
		{
			var returnArray = new int[arrayLength];
			for (var index = 0; index < arrayLength; index++)
			{
				returnArray[index] = INITIAL_RATE;
			}

			return returnArray;
		}

		/* 重みに基づき配列の何番目を選択するかをランダムに決定する(配列の最初であるならば1を返す) */
		private int GetRandomParameter(int[] randomParameters, int sumOfRandomParameters)
		{
			// 1以上重みの総和以下の値をランダムに取得する
			var randomValue = random.Next(sumOfRandomParameters) + 1;
			var index = 0;
			// 重み配列の最初の要素から順に、ランダムな値から値を引く
			while (randomValue > 0)
			{
				randomValue -= randomParameters[index];
				index += 1;
			}

			return index;
		}

		/* int[]配列を更新し、その配列の要素和を求める */
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

		// 乱数生成ための変数
		private readonly System.Random random = new System.Random();

		// 各パラメータの重みの初期値
		private const int INITIAL_RATE = 100;

		// Bullet(CartridgeとHole)の出現率の重み
		private int[] randomBulletTypes = SetInitialRate(Enum.GetNames(typeof(BulletType)).Length - 1);
		private int sumOfRandomBulletTypes = (Enum.GetNames(typeof(BulletType)).Length - 1) * INITIAL_RATE;

		// どのBulletを出現させるかランダムに決定する
		public BulletType GetBulletType()
		{
			var index = GetRandomParameter(randomBulletTypes, sumOfRandomBulletTypes);
			return (BulletType) Enum.ToObject(typeof(BulletType), index);
		}

		// 各Bulletの出現率の重みを設定する
		private void SetRandomBulletTypes(int[] randomBulletTypes)
		{
			if (randomBulletTypes == null) return;
			SetRandomParameter(out this.randomBulletTypes, out sumOfRandomBulletTypes, randomBulletTypes);
		}

		// 各Cartridgeの出現率の重み
		private int[] randomCartridgeTypes = SetInitialRate(Enum.GetNames(typeof(CartridgeType)).Length - 1);
		private int sumOfRandomCartridgeTypes = (Enum.GetNames(typeof(CartridgeType)).Length - 1) * INITIAL_RATE;

		// どのCartridgeを出現させるかランダムに決定する
		public CartridgeType GetCartridgeType()
		{
			var index = GetRandomParameter(randomCartridgeTypes, sumOfRandomCartridgeTypes);
			return (CartridgeType) Enum.ToObject(typeof(CartridgeType), index);
		}

		// 各Cartridgeの出現率の重みを設定する
		private void SetRandomCartridgeTypes(int[] randomCartridgeTypes)
		{
			if (randomCartridgeTypes == null) return;
			SetRandomParameter(out this.randomCartridgeTypes, out sumOfRandomCartridgeTypes, randomCartridgeTypes);
		}

		// 各Holeの出現率の重み
		private int[] randomHoleTypes = SetInitialRate(Enum.GetNames(typeof(HoleType)).Length - 1);
		private int sumOfRandomHoleTypes = (Enum.GetNames(typeof(HoleType)).Length - 1) * INITIAL_RATE;

		// どのHoleを出現させるかランダムに決定する
		public HoleType GetHoleType()
		{
			var index = GetRandomParameter(randomHoleTypes, sumOfRandomHoleTypes);
			return (HoleType) Enum.ToObject(typeof(HoleType), index);
		}

		// 各Holeの出現率の重みを設定する
		private void SetRandomHoleTypes(int[] randomHoleTypes)
		{
			if (randomHoleTypes == null) return;
			SetRandomParameter(out this.randomHoleTypes, out sumOfRandomHoleTypes, randomHoleTypes);
		}

		// Cartridgeの移動方向(上下左右)の出現率の重み
		private int[] randomCartridgeDirections = SetInitialRate(Enum.GetNames(typeof(CartridgeDirection)).Length - 1);

		private int sumOfRandomCartridgeDirections =
			(Enum.GetNames(typeof(CartridgeDirection)).Length - 1) * INITIAL_RATE;

		// どの方向にCartridgeを移動させるかランダムに決定する
		public CartridgeDirection GetCartridgeDirection()
		{
			var index = GetRandomParameter(randomCartridgeDirections, sumOfRandomCartridgeDirections);
			return (CartridgeDirection) Enum.ToObject(typeof(CartridgeDirection), index);
		}

		// Cartridgeの移動方向(上下左右)の出現率の重みを設定する
		private void SetRandomCartridgeDirections(int[] randomCartridgeDirections)
		{
			if (randomCartridgeDirections == null) return;
			SetRandomParameter(out this.randomCartridgeDirections, out sumOfRandomCartridgeDirections,
				randomCartridgeDirections);
		}

		// Cartridgeの出現する行の出現率の重み
		private int[] randomCartridgeRows = SetInitialRate(Enum.GetNames(typeof(Row)).Length - 1);
		private int sumOfRandomCartridgeRows = (Enum.GetNames(typeof(Row)).Length - 1) * INITIAL_RATE;

		// どの行にCartridgeを出現させるかランダムに決定する
		public int GetCartridgeRow()
		{
			var index = GetRandomParameter(randomCartridgeRows, sumOfRandomCartridgeRows);
			return (int) Enum.ToObject(typeof(Row), index);
		}

		// Cartridgeの出現する行の出現率の重みを設定する
		private void SetRandomCartridgeRows(int[] randomCartridgeRows)
		{
			if (randomCartridgeRows == null) return;
			SetRandomParameter(out this.randomCartridgeRows, out sumOfRandomCartridgeRows, randomCartridgeRows);
		}

		// Cartridgeの出現する列の出現率の重み
		private int[] randomCartridgeColumns = SetInitialRate(Enum.GetNames(typeof(Column)).Length - 1);
		private int sumOfRandomCartridgeColumns = (Enum.GetNames(typeof(Column)).Length - 1) * INITIAL_RATE;

		// どの列にCartridgeを出現させるかランダムに決定する
		public int GetCartridgeColumn()
		{
			var index = GetRandomParameter(randomCartridgeColumns, sumOfRandomCartridgeColumns);
			return (int) Enum.ToObject(typeof(Column), index);
		}

		// Cartridgeの出現する列の出現率の重みを設定する
		private void SetRandomCartridgeColumns(int[] randomCartridgeColumns)
		{
			if (randomCartridgeColumns == null) return;
			SetRandomParameter(out this.randomCartridgeColumns, out sumOfRandomCartridgeColumns,
				randomCartridgeColumns);
		}

		// Holeが出現するTileの出現率の重み
		private int[] randomTileNums = SetInitialRate(StageSize.TILE_NUM);
		private int sumOfRandomTileNums = StageSize.TILE_NUM * INITIAL_RATE;

		// どのTileにHoleを出現させるかランダムに決定する
		public int GetTileNum()
		{
			var index = GetRandomParameter(randomTileNums, sumOfRandomTileNums);
			return index;
		}

		// Holeの出現するTileの出現率の重みを設定する
		private void SetRandomTileNums(int[] randomTileNums)
		{
			if (randomTileNums == null) return;
			SetRandomParameter(out this.randomTileNums, out sumOfRandomTileNums, randomTileNums);
		}

		// Turncartridgeの曲がる方向の出現率の重み
		private int[] randomTurnDirections = SetInitialRate(Enum.GetNames(typeof(CartridgeDirection)).Length - 1);
		private int sumOfRandomTurnDirections = Enum.GetNames(typeof(CartridgeDirection)).Length - 1 * INITIAL_RATE;

		// どこでTurnCartridgeが曲がるかランダムに決定する
		public int GetRandomTurnDirection(CartridgeDirection direction, int line)
		{
			var randomTurnDirection = 0;
			// 最上行または最下行を移動している場合
			if ((direction == CartridgeDirection.ToLeft || direction == CartridgeDirection.ToRight) &&
			    (line == (int) Row.First || line == (int) Row.Fifth))
			{
				if (line == (int) Row.First)
				{
					randomTurnDirection = (int) CartridgeDirection.ToBottom;
				}
				else if (line == (int) Row.Fifth)
				{
					randomTurnDirection = (int) CartridgeDirection.ToUp;
				}
			}
			// 最左列または最も最右列を移動している場合
			else if ((direction == CartridgeDirection.ToUp || direction == CartridgeDirection.ToBottom) &&
			         (line == (int) Column.Left || line == (int) Column.Right))
			{
				if (line == (int) Column.Left)
				{
					randomTurnDirection = (int) CartridgeDirection.ToRight;
				}
				else if (line == (int) Column.Right)
				{
					randomTurnDirection = (int) CartridgeDirection.ToLeft;
				}
			}
			// 上記以外の場合
			else
			{
				// Cartridgeの移動方向に応じてCartridgeから見た右方向、左方向を取得する
				int cartridgeLocalLeft;
				int cartridgeLocalRight;
				switch (direction)
				{
					case CartridgeDirection.ToLeft:
						cartridgeLocalLeft = (int) CartridgeDirection.ToBottom;
						cartridgeLocalRight = (int) CartridgeDirection.ToUp;
						break;
					case CartridgeDirection.ToRight:
						cartridgeLocalLeft = (int) CartridgeDirection.ToUp;
						cartridgeLocalRight = (int) CartridgeDirection.ToBottom;
						break;
					case CartridgeDirection.ToUp:
						cartridgeLocalLeft = (int) CartridgeDirection.ToLeft;
						cartridgeLocalRight = (int) CartridgeDirection.ToRight;
						break;
					case CartridgeDirection.ToBottom:
						cartridgeLocalLeft = (int) CartridgeDirection.ToRight;
						cartridgeLocalRight = (int) CartridgeDirection.ToLeft;
						break;
					default:
						throw new NotImplementedException();
				}

				// 乱数を取得する
				var randomValue = random.Next(randomTurnDirections[cartridgeLocalLeft - 1] +
				                              randomTurnDirections[cartridgeLocalRight - 1]) + 1;
				// 乱数に基づいてCartridgeから見て右または左のどちらかの方向を選択する
				randomTurnDirection = randomValue <= randomTurnDirections[cartridgeLocalLeft - 1]
					? (int) Enum.ToObject(typeof(CartridgeDirection), cartridgeLocalLeft)
					: (int) Enum.ToObject(typeof(CartridgeDirection), cartridgeLocalRight);
			}

			return randomTurnDirection;
		}

		// Turncartridgeの曲がる方向の出現率の重みを設定する
		private void SetRandomTurnDirections(int[] randomTurnDirections)
		{
			if (randomTurnDirections == null) return;
			SetRandomParameter(out this.randomTurnDirections, out sumOfRandomTurnDirections, randomTurnDirections);
		}

		// 曲がる位置の選択率の重み
		private int[] randomTurnRows = SetInitialRate(StageSize.ROW);
		private int[] randomTurnColumns = SetInitialRate(StageSize.COLUMN);
		private int sumOfRandomTurnRows = StageSize.ROW * INITIAL_RATE;
		private int sumOfRandomTurnColumns = StageSize.COLUMN * INITIAL_RATE;

		// どこで曲がるのかランダムに決定する
		public int GetRandomTurnLine(CartridgeDirection direction = 0)
		{
			int randomValue;
			// 乱数に基づいて方向を選択する
			var index = 0;
			var randomTurnLine = 0;
			switch (direction)
			{
				case CartridgeDirection.ToLeft:
				case CartridgeDirection.ToRight:
					randomValue = random.Next(sumOfRandomTurnColumns) + 1;
					while (randomValue > 0)
					{
						randomValue -= randomTurnColumns[index];
						index += 1;
					}

					randomTurnLine = (int) Enum.ToObject(typeof(Column), index);
					break;
				case CartridgeDirection.ToUp:
				case CartridgeDirection.ToBottom:
					randomValue = random.Next(sumOfRandomTurnRows) + 1;
					while (randomValue > 0)
					{
						randomValue -= randomTurnRows[index];
						index += 1;
					}

					randomTurnLine = (int) Enum.ToObject(typeof(Row), index);
					break;
				default:
					throw new NotImplementedException();
			}

			return randomTurnLine;
		}

		// AimingHoleが出現するNumberPanelの出現率の重み
		private int[] randomNumberPanels = SetInitialRate(StageSize.NUMBER_PANEL_NUM);
		private int sumOfRandomNumberPanels = StageSize.NUMBER_PANEL_NUM * INITIAL_RATE;

		// どのNumberPanelにAimingHoleを出現させるかランダムに決定する
		public int GetNumberPanel()
		{
			var index = GetRandomParameter(randomNumberPanels, sumOfRandomNumberPanels);
			return index;
		}

		// AimingHoleの出現するNumberPanelの出現率の重みを設定する
		private void SetRandomNumberPanels(int[] randomNumberPanels)
		{
			if (randomNumberPanels == null) return;
			SetRandomParameter(out this.randomNumberPanels, out sumOfRandomNumberPanels, randomNumberPanels);
		}

		/* Bulletに関する引数を設定するメソッド */
		public static BulletInfo SetBulletInfo(int[] randomBulletType = null, int[] randomCartridgeType = null,
			int[] randomHoleType = null, int[] turnDirection = null, int[] turnLine = null,
			int[] aimingPanel = null, int[] randomDirections = null, int[] randomRows = null,
			int[] randomColumns = null, int[] randomTileNums = null, int[] randomNumberPanels = null)
		{
			var bulletInfo = new BulletInfo();
			bulletInfo.SetRandomBulletTypes(randomBulletType);
			bulletInfo.SetRandomCartridgeTypes(randomCartridgeType);
			bulletInfo.SetRandomHoleTypes(randomHoleType);
			bulletInfo.SetTurnDirection(turnDirection);
			bulletInfo.SetTurnLine(turnLine);
			bulletInfo.SetAimingPanel(aimingPanel);
			bulletInfo.SetAimingHoleCount(1);
			bulletInfo.SetRandomCartridgeDirections(randomDirections);
			bulletInfo.SetRandomCartridgeRows(randomRows);
			bulletInfo.SetRandomCartridgeColumns(randomColumns);
			bulletInfo.SetRandomTileNums(randomTileNums);
			bulletInfo.SetRandomNumberPanels(randomNumberPanels);
			return bulletInfo;
		}

		/* Cartridgeに関する引数を設定するメソッド */
		public static BulletInfo SetCartridgeInfo(int[] randomCartridgeType = null, int[] randomDirections = null,
			int[] randomRows = null,
			int[] randomColumns = null)
		{
			var bulletInfo = new BulletInfo();
			bulletInfo.SetRandomCartridgeTypes(randomCartridgeType);
			bulletInfo.SetRandomCartridgeDirections(randomDirections);
			bulletInfo.SetRandomCartridgeRows(randomRows);
			bulletInfo.SetRandomCartridgeColumns(randomColumns);
			return bulletInfo;
		}

		/* Holeに関する引数を設定するメソッド */
		public static BulletInfo SetHoleInfo(int[] randomHoleType = null, int[] randomTileNums = null,
			int[] randomNumberPanels = null)
		{
			var bulletInfo = new BulletInfo();
			bulletInfo.SetRandomHoleTypes(randomHoleType);
			bulletInfo.SetRandomTileNums(randomTileNums);
			bulletInfo.SetRandomNumberPanels(randomNumberPanels);
			return bulletInfo;
		}

		/* TurnCartridgeに関する引数を設定するメソッド */
		public static BulletInfo SetTurnCartridgeInfo(int[] turnDirection = null, int[] turnLine = null,
			int[] randomDirections = null, int[] randomRows = null,
			int[] randomColumns = null)
		{
			var bulletInfo = new BulletInfo();
			bulletInfo.SetTurnDirection(turnDirection);
			bulletInfo.SetTurnLine(turnLine);
			bulletInfo.SetRandomCartridgeDirections(randomDirections);
			bulletInfo.SetRandomCartridgeRows(randomRows);
			bulletInfo.SetRandomCartridgeColumns(randomColumns);
			return bulletInfo;
		}

		/* AimingHoleに関する引数を設定するメソッド */
		public static BulletInfo SetAimingHoleInfo(int[] aimingPanel = null, int[] randomNumberPanels = null)
		{
			var bulletInfo = new BulletInfo();
			bulletInfo.SetAimingPanel(aimingPanel);
			bulletInfo.SetAimingHoleCount(1);
			bulletInfo.SetRandomNumberPanels(randomNumberPanels);
			return bulletInfo;
		}
	}
}

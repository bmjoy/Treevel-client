using System.Collections.Generic;

namespace Project.Scripts.Utils.Definitions
{
	public enum StageLevel
	{
		Easy = 0,
		Normal,
		Hard,
		VeryHard
	}

	public static class StageInfo
	{
		public static readonly Dictionary<StageLevel, int> Num = new Dictionary<StageLevel, int>()
		{
			{StageLevel.Easy, 10},
			{StageLevel.Normal, 10},
			{StageLevel.Hard, 10},
			{StageLevel.VeryHard, 10}
		};

		public static readonly Dictionary<StageLevel, int> StageStartId = new Dictionary<StageLevel, int>()
		{
			{StageLevel.Easy, 1},
			{StageLevel.Normal, 1001},
			{StageLevel.Hard, 2001},
			{StageLevel.VeryHard, 3001}
		};
	}

	public static class StageSize
	{
		public const int ROW = 5;
		public const int COLUMN = 3;
		public const int NUM = ROW * COLUMN;
	}
}

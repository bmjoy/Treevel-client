using System.Collections.Generic;

namespace Project.Scripts.Utils.Definitions
{
	public enum EStageLevel
	{
		Easy = 0,
		Normal,
		Hard,
		VeryHard
	}

	public static class StageInfo
	{
		public static readonly Dictionary<EStageLevel, int> Num = new Dictionary<EStageLevel, int>()
		{
			{EStageLevel.Easy, 10},
			{EStageLevel.Normal, 10},
			{EStageLevel.Hard, 10},
			{EStageLevel.VeryHard, 10}
		};

		public static readonly Dictionary<EStageLevel, int> StageStartId = new Dictionary<EStageLevel, int>()
		{
			{EStageLevel.Easy, 1},
			{EStageLevel.Normal, 1001},
			{EStageLevel.Hard, 2001},
			{EStageLevel.VeryHard, 3001}
		};
	}

	public static class StageSize
	{
		public const int ROW = 5;
		public const int COLUMN = 3;
		public const int TILE_NUM = ROW * COLUMN;
		public const int NUMBER_PANEL_NUM = 8;
	}
}

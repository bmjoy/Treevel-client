namespace Project.Scripts.Utils.Definitions
{
	public static class StageNum
	{
		public const int EASY = 10;
		public const int NORMAL = 10;
		public const int HARD = 10;
		public const int VERY_HARD = 10;
	}

	public static class StageStartId
	{
		public const int EASY = 0;
		public const int NORMAL = EASY + StageNum.EASY;
		public const int HARD = NORMAL + StageNum.NORMAL;
		public const int VERY_HARD = HARD + StageNum.HARD;
	}
}

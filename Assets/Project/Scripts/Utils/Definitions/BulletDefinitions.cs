namespace Project.Scripts.Utils.Definitions
{
	public enum ECartridgeType
	{
		Normal = 1,
		Turn,
		Random = -1
	}

	public enum HoleType
	{
		Normal = 1,
		Aiming,
		Random = -1
	}

	public enum CartridgeDirection
	{
		ToLeft = 1,
		ToRight,
		ToUp,
		ToBottom,
		Random = -1
	}

	public enum Row
	{
		First = 1,
		Second,
		Third,
		Fourth,
		Fifth,
		Random = -1
	}

	public enum Column
	{
		Left = 1,
		Center,
		Right,
		Random = -1
	}
}

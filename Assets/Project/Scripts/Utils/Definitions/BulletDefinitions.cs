namespace Project.Scripts.Utils.Definitions
{
	public enum BulletType
	{
		Cartridge = 1,
		Hole,
		Random = 0
	}

	public enum CartridgeType
	{
		Normal = 1,
		Turn,
		Random = 0
	}

	public enum HoleType
	{
		Normal = 1,
		Aiming,
		Random = 0
	}

	public enum CartridgeDirection
	{
		ToLeft = 1,
		ToRight,
		ToUp,
		ToBottom,
		Random = 0
	}

	public enum Row
	{
		First = 1,
		Second,
		Third,
		Fourth,
		Fifth,
		Random = 0
	}

	public enum Column
	{
		Left = 1,
		Center,
		Right,
		Random = 0
	}
}

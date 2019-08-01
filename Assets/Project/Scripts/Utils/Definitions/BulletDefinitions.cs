namespace Project.Scripts.Utils.Definitions
{
    public enum ECartridgeType {
        Normal = 1,
        Turn,
        Random = -1
    }

    public enum EHoleType {
        Normal = 1,
        Aiming,
        Random = -1
    }

    public enum ECartridgeDirection {
        ToLeft = 1,
        ToRight,
        ToUp,
        ToBottom,
        Random = -1
    }

    public enum ERow {
        First = 1,
        Second,
        Third,
        Fourth,
        Fifth,
        Random = -1
    }

    public enum EColumn {
        Left = 1,
        Center,
        Right,
        Random = -1
    }
}

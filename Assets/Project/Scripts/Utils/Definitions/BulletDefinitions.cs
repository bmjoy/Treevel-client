namespace Project.Scripts.Utils.Definitions
{
    public static class BulletGeneratorParameter
    {
        /// <summary>
        /// ランダムな値を決めるときの各要素の重みの初期値
        /// </summary>
        public const int INITIAL_RATIO = 100;
    }
    
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

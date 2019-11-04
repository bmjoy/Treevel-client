namespace Project.Scripts.Utils.Definitions
{
    public static class BulletGeneratorParameter
    {
        /// <summary>
        /// ランダムな値を決めるときの各要素の重みの初期値
        /// </summary>
        public const int INITIAL_RATIO = 100;
    }

    /// <summary>
    /// 弾丸型の銃弾の名前
    /// </summary>
    public enum ECartridgeType {
        Normal = 1,
        Turn,
        Random = -1
    }

    /// <summary>
    /// 銃痕型の銃弾の名前
    /// </summary>
    public enum EHoleType {
        Normal = 1,
        Aiming,
        Random = -1
    }

    /// <summary>
    /// 銃弾の移動方向
    /// </summary>
    public enum ECartridgeDirection {
        ToLeft = 1,
        ToRight,
        ToUp,
        ToBottom,
        Random = -1
    }

    /// <summary>
    /// 盤面の行番号
    /// </summary>
    public enum ERow {
        First = 1,
        Second,
        Third,
        Fourth,
        Fifth,
        Random = -1
    }

    /// <summary>
    /// 盤面の列番号
    /// </summary>
    public enum EColumn {
        Left = 1,
        Center,
        Right,
        Random = -1
    }

    /// <summary>
    /// 弾丸の種類
    /// </summary>
    public enum EBulletType {
        NormalCartridge,
        TurnCartridge,
        RandomNormalCartridge,
        RandomTurnCartridge,
        NormalHole,
        AimingHole,
        RandomHole,
    }
}

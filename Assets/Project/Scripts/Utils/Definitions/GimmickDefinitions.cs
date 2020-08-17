namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 太陽光ギミックの攻撃方向
    /// </summary>
    public enum ESolarBeamDirection {
        Vertical,
        Horizontal,
    }

    /// <summary>
    /// ギミックの移動方向
    /// </summary>
    public enum EGimmickDirection {
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
    /// ギミックの種類
    /// </summary>
    public enum EGimmickType {
        Tornado,
        Meteorite,
        AimingMeteorite,
        Thunder,
        SolarBeam,
    }
}

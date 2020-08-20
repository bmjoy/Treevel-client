namespace Project.Scripts.Utils.Definitions
{
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
}

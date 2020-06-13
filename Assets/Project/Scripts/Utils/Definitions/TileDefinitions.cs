namespace Project.Scripts.Utils.Definitions
{
    #if UNITY_EDITOR
    /// <summary>
    /// タイルの名前
    /// </summary>
    public static class TileName
    {
        public const string NORMAL_TILE = "NormalTile";
        public const string WARP_TILE = "WarpTile";
        public const string HOLY_TILE = "HolyTile";
    }
    #endif

    public enum ETileType {
        Normal,
        Warp,
        Holy,
    }
}

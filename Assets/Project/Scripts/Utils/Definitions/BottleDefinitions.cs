namespace Project.Scripts.Utils.Definitions
{
    #if UNITY_EDITOR
    /// <summary>
    /// ボトルの名前
    /// </summary>
    public static class BottleName
    {
        public const string DYNAMIC_DUMMY_BOTTLE = "DynamicDummyBottle";
        public const string STATIC_DUMMY_BOTTLE = "StaticDummyBottle";
        public const string NORMAL_BOTTLE = "NormalBottle";
    }
    #endif

    public enum EBottleType {
        Dynamic,
        Static,
        Normal,
        Life,
    }
}

namespace Project.Scripts.Utils.Definitions
{
    #if UNITY_EDITOR
    /// <summary>
    /// パネルの名前
    /// </summary>
    public static class BottleName
    {
        public const string DYNAMIC_DUMMY_BOTTLE = "DynamicDummyBottle";
        public const string STATIC_DUMMY_BOTTLE = "StaticDummyBottle";
        public const string NUMBER_BOTTLE = "NumberBottle";
    }
    #endif

    public enum EBottleType {
        Dynamic,
        Static,
        Number,
        LifeNumber,
    }
}

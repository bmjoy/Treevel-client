namespace Project.Scripts.Utils.Definitions
{
    #if UNITY_EDITOR
    /// <summary>
    /// パネルの名前
    /// </summary>
    public static class PanelName
    {
        public const string DYNAMIC_DUMMY_PANEL = "DynamicDummyPanel";
        public const string STATIC_DUMMY_PANEL = "StaticDummyPanel";
        public const string NUMBER_PANEL = "NumberPanel";
    }
    #endif

    public enum EPanelType {
        Dynamic,
        Static,
        Number,
        LifeNumber,
    }
}

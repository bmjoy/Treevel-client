namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// パネルの名前
    /// </summary>
    public static class PanelName
    {
        public const string DYNAMIC_DUMMY_PANEL = "DynamicDummyPanel";
        public const string STATIC_DUMMY_PANEL = "StaticDummyPanel";
        public const string NUMBER_PANEL = "NumberPanel";
    }

    public enum EPanelType
    {
        Dynamic,
        Static,
        Number,
        LifeNumber,
    }
}

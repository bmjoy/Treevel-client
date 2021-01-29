namespace Treevel.Common.Entities
{
    /// <summary>
    /// ステージの状態
    /// </summary>
    public enum EStageState
    {
        /// <summary>
        /// 非解放状態
        /// </summary>
        Unreleased,

        /// <summary>
        /// 解放状態
        /// </summary>
        Released,

        /// <summary>
        /// クリア状態( ⊆ 解放状態)
        /// </summary>
        Cleared,
    }
}

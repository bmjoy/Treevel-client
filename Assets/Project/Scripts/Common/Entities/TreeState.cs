namespace Treevel.Common.Entities
{
    /// <summary>
    /// 木の状態
    /// </summary>
    public enum ETreeState {
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
        /// <summary>
        /// 全ステージクリア状態( ⊆ クリア状態 ⊆ 解放状態)
        /// </summary>
        AllCleared
    }
}

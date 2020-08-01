namespace Project.Scripts.Utils.Definitions
{
    public static class StageSize
    {
        /// <summary>
        /// ステージのタイル行数
        /// </summary>
        public const int ROW = 5;

        /// <summary>
        /// ステージのタイル列数
        /// </summary>
        public const int COLUMN = 3;

        /// <summary>
        /// タイルの合計数
        /// </summary>
        public const int TILE_NUM = ROW * COLUMN;

        /// <summary>
        /// ナンバーボトルの数
        /// </summary>
        public const int NUMBER_BOTTLE_NUM = 8;
    }

    /// <summary>
    /// ステージの状態
    /// </summary>
    public enum EStageState {
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
        Cleared
    }
}

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 多言語対応するテキスト
    /// </summary>
    public enum ETextIndex {
        Config,          // 設定
        ConfigReset,     // リセット
        ConfigVibration, // 振動
        ConfigVolume,    // 音量
        GameFailure,     // 失敗
        GameSuccess,     // 成功
        GameResult,      // 結果
        GameRetry,       // リトライ
        GameWarning,     // ゲーム中にアプリを閉じた時の注意
        LanguageJapanese,   // 日本語
        LanguageEnglish,    // 英語
        LevelFirst,      // レベル1
        LevelSecond,     // レベル2
        LevelThird,      // レベル3
        LevelFourth,     // レベル4
        Tutorial,        // チュートリアル
        GameTweet,       // ゲーム結果のツイート
        GamePause,       // ゲーム一時停止
        GamePauseBack,   // ゲームの再開
        GamePauseQuit,   // ゲームを諦める
        GameBack,        // ステージ選択画面へ戻る
        ConfigHideOverview,    // ステージの概要の表示を隠すかどうか
    }
}

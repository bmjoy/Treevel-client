namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// 多言語対応するテキスト
    /// </summary>
    public enum ETextIndex {
        Error,           // ERROR
        Config,          // 設定
        ConfigReset,     // リセット
        ConfigVibration, // 振動
        ConfigVolume,    // 音量
        ConfigHideOverview,    // ステージの概要の表示を隠すかどうか
        GameFailure,     // 失敗
        GameSuccess,     // 成功
        GameResult,      // 結果
        GameRetry,       // リトライ
        GameWarning,     // ゲーム中にアプリを閉じた時の注意
        GameTweet,       // ゲーム結果のツイート
        GamePause,       // ゲーム一時停止
        GamePauseBack,   // ゲームの再開
        GamePauseQuit,   // ゲームを諦める
        GameBack,        // ステージ選択画面へ戻る
        LanguageJapanese,   // 日本語
        LanguageEnglish,    // 英語
        LevelFirst,      // レベル1
        LevelSecond,     // レベル2
        LevelThird,      // レベル3
        LevelFourth,     // レベル4
        Tutorial,        // チュートリアル
        Downloading,     // ダウンロード中...
        ReturnToTitle,   // タイトルに戻る

        ErrorTextStart = 10000, // ここからはエラーメッセージ
        ErrorUnknown = ErrorTextStart, // 不明なエラー
    }
}

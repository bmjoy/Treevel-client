namespace Treevel.Common.Entities
{
    /// <summary>
    /// 多言語対応するテキスト
    /// </summary>
    public enum ETextIndex
    {
        Error,                                // ERROR
        SettingsTitle,                        // 設定
        SettingsReset,                        // リセット
        SettingsVibration,                    // 振動
        SettingsBGM,                          // BGM
        SettingsSE,                           // SE
        SettingsStageDetails,                 // ステージの概要の表示を隠すかどうか
        SettingsToDefault,                    // デフォルト設定にする
        GameFailure,                          // 失敗
        GameSuccess,                          // 成功
        GameResult,                           // 結果
        GameRetry,                            // リトライ
        GameWarning,                          // ゲーム中にアプリを閉じた時の注意
        GameTweet,                            // ゲーム結果のツイート
        GamePause,                            // ゲーム一時停止
        GamePauseBack,                        // ゲームの再開
        GamePauseQuit,                        // ゲームを諦める
        GameBack,                             // ステージ選択画面へ戻る
        GameNext,                             // 次に進む
        LanguageJapanese,                     // 日本語
        LanguageEnglish,                      // 英語
        StageSelectOverviewStartGame,         // すすむ
        StageSelectOverviewCloseWindow,       // とじる
        StageSelectOverviewSuccessPercentage, // 成功割合
        StageSelectOverviewShortestClearTime, // 最速クリアタイム
        StageSelectOverviewAppearGimmick,     // 登場ギミック
        CommonUnitSecond,                     // 秒
        LevelFirst,                           // レベル1
        LevelSecond,                          // レベル2
        LevelThird,                           // レベル3
        LevelFourth,                          // レベル4
        Tutorial,                             // チュートリアル
        Downloading,                          // ダウンロード中...
        ReturnToTitle,                        // タイトルに戻る
        StartGame,                            // スタート

        // 記録画面
        RecordShareButton,               // 共有ボタン
        RecordToIndividualButton,        // 個別記録へボタン
        RecordToGeneralButton,           // 総合記録へボタン
        RecordClearStageNum,             // ステージクリア数
        RecordPlayNum,                   // プレイ回数
        RecordPlayDays,                  // 起動日数
        RecordFlickNum,                  // フリック回数
        RecordFailureNum,                // 失敗回数
        RecordClearRate,                 // 成功率
        RecordResetConfirmDialogMessage, // リセット確認メッセージダイアログ
        ReSendStageRecordDialogMessage,  // ステージ記録の再送信ダイアログ
        SpringShort,                     // 春の略記
        SummerShort,                     // 夏の略記
        AutumnShort,                     // 秋の略記
        WinterShort,                     // 冬の略記
        MessageDlgOkBtnText,             // メッセージダイアログーOKボタン
        MessageDlgCancelBtnText,         // メッセージダイアログーCancelボタン
        MessageDlgOkBtnRetryText,        // メッセージダイアログーリトライボタン
        MessageDlgOkBtnGiveUpText,       // メッセージダイアログー諦めるボタン
        ErrorTextStart = 10000,          // ここからはエラーメッセージ
        ErrorUnknown = ErrorTextStart,   // 不明なエラー
        ErrorLoadDataFailed,             // データの読み込みが失敗しました
        ErrorInvalidBottleAccess,        // ボトルの不正アクセスが発生しました。
        ErrorInvalidBottleColor,         // ボトルの色が正しく設定されていません。
        ErrorInvalidLifeValue,           // ライフの値が正しく設定されていません。
        ErrorInvalidTileColor,           // タイルの色が正しく設定されていません。
        ErrorInvalidGimmickData,         // ギミックデータが正しく設定されていません。
        SaveStageRecordError,            // ステージの記録を保存することに失敗しました。
    }
}

namespace Treevel.Common.Entities
{
    /// <summary>
    /// エラー発生時に追跡し易くするため、各種エラーコードを定義し、<see cref="ErrorMessageBox">からユーザに提示する。
    /// エラーメッセージは<see cref="ETextIndex.ErrorTextStart">からエラーコードと同じ順序で追加する。
    /// Enumのインデックスを3桁の数字としてErrorCode=000のようにエラーメッセージの後ろに付けてユーザに提示する
    /// </summary>
    public enum EErrorCode
    {
        UnknownError,
        LoadDataError,      // データの読み込みに失敗
        InvalidBottleID,    // ボトルIDが不正
        InvalidBottleColor, // ボトルの色が不適当
        InvalidLifeValue,   // lifeの値が不適当
        InvalidTileColor,   // タイルの色が不適当
        InvalidGimmickData, // 不正のギミックデータ
    }
}

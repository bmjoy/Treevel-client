namespace Project.Scripts.GamePlayScene.Panel
{
    /// <summary>
    /// 成功判定を処理するインターフェイス
    /// 成功判定させたいパネルクラスに実装させて、タイルに移動した際成功判定を発火する
    /// </summary>
    internal interface IPanelSuccessHandler
    {
        /// <summary>
        /// タイルに載せた時の挙動
        /// </summary>
        /// <returns>載せた後に該当パネルが成功したかどうか</returns>
        bool DoWhenSuccess();

        /// <summary>
        /// 該当パネルが成功したかどうか
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();
    }
}

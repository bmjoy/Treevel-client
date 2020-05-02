namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// 成功判定を処理するインターフェイス
    /// 成功判定させたいボトルクラスに実装させて、タイルに移動した際成功判定を発火する
    /// </summary>
    internal interface IBottleSuccessHandler
    {
        /// <summary>
        /// タイルに載せた時の挙動
        /// </summary>
        void DoWhenSuccess();

        /// <summary>
        /// 該当ボトルが成功したかどうか
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();
    }
}

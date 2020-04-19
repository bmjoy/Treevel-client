
namespace Project.Scripts.GamePlayScene.Panel
{
    internal interface IJudgementHandler
    {
        /// <summary>
        /// タイルに載せた時の挙動
        /// </summary>
        /// <returns>載せた後に該当パネルが成功したかどうか</returns>
        bool Adapt();

        /// <summary>
        /// 該当パネルが成功したかどうか
        /// </summary>
        /// <returns></returns>
        bool Adapted();
    }
}

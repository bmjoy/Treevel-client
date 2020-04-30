using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// 成功判定を処理するインターフェイス
    /// 成功判定させたいボトルクラスに実装させて、タイルに移動した際成功判定を発火する
    /// </summary>
    public interface IBottleSuccessHandler
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

    internal class NormalBottleSuccessHandler : IBottleSuccessHandler
    {
        /// <summary>
        /// 目標位置
        /// </summary>
        private readonly int _targetPos;

        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        internal NormalBottleSuccessHandler(AbstractBottleController bottle, int targetPos)
        {
            _bottle = bottle;
            _targetPos = targetPos;
        }

        public void DoWhenSuccess()
        {
            // ステージの成功判定
            GameObject.FindObjectOfType<GamePlayDirector>().CheckClear();
        }

        public bool IsSuccess()
        {
            var currPos = BoardManager.GetBottlePos(_bottle);
            return currPos == _targetPos;
        }
    }
}

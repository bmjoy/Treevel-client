using Treevel.Common.Entities;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    public class PauseWindow : MonoBehaviour
    {
        /// <summary>
        /// ゲーム再開ボタン押下時の処理
        /// </summary>
        public void PauseBackButtonDown()
        {
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Playing);
        }

        /// <summary>
        /// ゲーム終了ボタン押下時の処理
        /// </summary>
        public void PauseQuitButtonDown()
        {
            GamePlayDirector.Instance.failureReason = EFailureReasonType.Others;
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }
    }
}

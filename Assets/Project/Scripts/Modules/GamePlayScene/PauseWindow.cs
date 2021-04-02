using Treevel.Common.Entities;
using Treevel.Common.Managers;
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
            SoundManager.Instance.PlaySE(ESEKey.UI_Dropdown_close);
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Playing);
        }

        /// <summary>
        /// ゲーム終了ボタン押下時の処理
        /// </summary>
        public void PauseQuitButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Dropdown_close);
            GamePlayDirector.Instance.failureReason = EFailureReasonType.Others;
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }
    }
}

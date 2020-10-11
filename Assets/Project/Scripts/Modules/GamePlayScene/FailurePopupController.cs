using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public class FailurePopupController : MonoBehaviour
    {
        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        public void RetryButtonDown()
        {
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Playing);
        }

        /// <summary>
        /// 戻るボタン押下時の処理
        /// </summary>
        public void BackButtonDown()
        {
            // StageSelectSceneに戻る
            TreeLibrary.LoadStageSelectScene(GamePlayDirector.levelName);
        }
    }
}

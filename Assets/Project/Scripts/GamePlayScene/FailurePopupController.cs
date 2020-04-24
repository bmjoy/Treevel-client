using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public class FailurePopupController : MonoBehaviour
    {
        private GamePlayDirector _gamePlayDirector;

        private void Start()
        {
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        public void RetryButtonDown()
        {
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Playing);
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

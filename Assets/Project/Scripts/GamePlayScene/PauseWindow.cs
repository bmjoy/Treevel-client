using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public class PauseWindow : MonoBehaviour
    {
        private GamePlayDirector _gamePlayDirector;

        private void Start()
        {
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        /// <summary>
        /// ゲーム再開ボタン押下時の処理
        /// </summary>
        public void PauseBackButtonDown()
        {
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Playing);
        }

        /// <summary>
        /// ゲーム終了ボタン押下時の処理
        /// </summary>
        public void PauseQuitButtonDown()
        {
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
        }
    }
}

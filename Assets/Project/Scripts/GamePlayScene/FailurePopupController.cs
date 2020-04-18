using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class FailurePopupController : MonoBehaviour
    {
        private GamePlayDirector _gamePlayDirector;

        /// <summary>
        /// リトライボタン
        /// </summary>
        private GameObject _retryButton;

        /// <summary>
        /// 戻るボタン
        /// </summary>
        private GameObject _backButton;

        private void Awake()
        {
            GamePlayDirector.OnFail += OnFail;
            gameObject.SetActive(false);
        }

        private void Start()
        {
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        private void OnDestroy()
        {
            GamePlayDirector.OnFail -= OnFail;
        }

        private void OnFail()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        public void RetryButtonDown()
        {
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(GamePlayDirector.stageId);
            ss.IncChallengeNum(GamePlayDirector.stageId);
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Opening);
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

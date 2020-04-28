using System;
using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
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

﻿using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class PauseWindow : MonoBehaviour
    {
        private GamePlayDirector _gamePlayDirector;

        /// <summary>
        /// 一時停止ボタン
        /// </summary>
        private GameObject _pauseButton;

        /// <summary>
        /// 一時停止中の背景
        /// </summary>
        private GameObject _pauseBackground;

        /// <summary>
        /// ゲーム再開ボタン
        /// </summary>
        private GameObject _backButton;

        /// <summary>
        /// ゲーム終了ボタン
        /// </summary>
        private GameObject _quitButton;

        void Awake()
        {
            _pauseButton = GameObject.Find("PauseButton").gameObject;
            _pauseBackground = transform.Find("PauseBackground").gameObject;
            _backButton = transform.Find("PausePopup/PauseBackButton").gameObject;
            _quitButton = transform.Find("PausePopup/PauseQuitButton").gameObject;
            _backButton.GetComponent<Button>().onClick.AddListener(PauseBackButtonDown);
            _quitButton.GetComponent<Button>().onClick.AddListener(PauseQuitButtonDown);
        }

        void Start()
        {
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        /// <summary>
        /// ゲーム再開ボタン押下時の処理
        /// </summary>
        public void PauseBackButtonDown()
        {
            // 一時停止ボタンを有効にする
            _pauseButton.SetActive(true);
            // 一時停止ウィンドウを非表示にする
            gameObject.SetActive(false);
            // ゲーム内の時間を元に戻す
            Time.timeScale = 1.0f;
            // ゲームプレイ状態に遷移する
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Playing);
        }

        /// <summary>
        /// ゲーム終了ボタン押下時の処理
        /// </summary>
        public void PauseQuitButtonDown()
        {
            // 失敗回数をインクリメント
            var ss = StageStatus.Get(GamePlayDirector.stageId);
            ss.IncFailureNum(GamePlayDirector.stageId);
            // ゲーム内の時間を元に戻す
            Time.timeScale = 1.0f;
            // StageSelectSceneに戻る
            SceneManager.LoadScene(SceneName.MENU_SELECT_SCENE);
        }
    }
}
using System;
using System.Collections;
using System.IO;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class ResultPopup : MonoBehaviour
    {
        private GamePlayDirector _gamePlayDirector;

        /// <summary>
        /// 結果表示用のテキスト
        /// </summary>
        private GameObject _title;

        /// <summary>
        /// リトライボタン
        /// </summary>
        private GameObject _retryButton;

        /// <summary>
        /// 戻るボタン
        /// </summary>
        private GameObject _backButton;

        /// <summary>
        /// 次に進むボタン
        /// </summary>
        private GameObject _nextButton;

        /// <summary>
        /// 投稿ボタン
        /// </summary>
        private GameObject _shareButton;

        private void Awake()
        {
            _title = transform.Find("Title").gameObject;
            _retryButton = transform.Find("RetryButton").gameObject;
            _backButton = transform.Find("BackButton").gameObject;
            _nextButton = transform.Find("NextButton").gameObject;
            _shareButton = transform.Find("ShareButton").gameObject;

            _retryButton.GetComponent<Button>().onClick.AddListener(RetryButtonDown);
            _backButton.GetComponent<Button>().onClick.AddListener(BackButtonDown);
            _nextButton.GetComponent<Button>().onClick.AddListener(NextButtonDown);
            _shareButton.GetComponent<Button>().onClick.AddListener(ShareButtonDown);
        }

        private void Start()
        {
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        private void OnEnable()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        private void OnSucceed()
        {
            _title.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameSuccess;

            // 成功時は，リトライボタン，戻るボタン，は表示しない
            _retryButton.SetActive(false);
            _backButton.SetActive(false);
        }

        private void OnFail()
        {
            _title.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameFailure;

            // 失敗時は，次に進むボタン，投稿ボタン，は表示しない
            _nextButton.SetActive(false);
            _shareButton.SetActive(false);
        }

        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        private void RetryButtonDown()
        {
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(GamePlayDirector.stageId);
            ss.IncChallengeNum(GamePlayDirector.stageId);
            _gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Opening);
        }

        /// <summary>
        /// 戻るボタン押下時の処理
        /// </summary>
        private static void BackButtonDown()
        {
            // StageSelectSceneに戻る
            TreeLibrary.LoadStageSelectScene(GamePlayDirector.levelName);
        }

        /// <summary>
        /// 次に進むボタン押下時の処理
        /// </summary>
        private static void NextButtonDown()
        {
            // StageSelectSceneに戻る
            TreeLibrary.LoadStageSelectScene(GamePlayDirector.levelName);
        }

        /// <summary>
        /// 投稿ボタン押下時の処理
        /// </summary>
        private static void ShareButtonDown()
        {
            // Unity エディタ上では実行しない
            #if !UNITY_EDITOR
            StartCoroutine(Share());
            #endif
        }

        private IEnumerator Share()
        {
            // 投稿用のテキスト
            var text = "ステージ" + GamePlayDirector.stageId + "番を" + _title.GetComponent<Text>().text;
            // URL 用に加工
            text = UnityWebRequest.EscapeURL(text);

            // 投稿用のハッシュタグ
            var hashTags = "NumberBullet,ナンバレ";
            hashTags = UnityWebRequest.EscapeURL(hashTags);

            // スクリーンショットを撮る
            const string imgPath = "Assets/StreamingAssets/SuccessScreenShot.png";
            ScreenCapture.CaptureScreenshot(imgPath);

            // スクリーンショットが保存されるのを待つ
            while (!File.Exists(imgPath)) {
                yield return null;
            }

            // シェア画面へ
            SocialConnector.SocialConnector.Share(text, "", imgPath);
        }
    }
}

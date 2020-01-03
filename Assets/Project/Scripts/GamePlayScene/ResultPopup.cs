using System.Collections;
using System.IO;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Definitions;
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
        private GameObject _resultTitle;

        /// <summary>
        /// リトライボタン
        /// </summary>
        private GameObject _resultRetryButton;

        /// <summary>
        /// 戻るボタン
        /// </summary>
        private GameObject _resultBackButton;

        /// <summary>
        /// 投稿ボタン
        /// </summary>
        private GameObject _resultShareButton;

        private void Awake()
        {
            _resultTitle = transform.Find("ResultTitle").gameObject;
            _resultRetryButton = transform.Find("ResultRetryButton").gameObject;
            _resultBackButton = transform.Find("ResultBackButton").gameObject;
            _resultShareButton = transform.Find("ResultShareButton").gameObject;

            _resultRetryButton.GetComponent<Button>().onClick.AddListener(RetryButtonDown);
            _resultBackButton.GetComponent<Button>().onClick.AddListener(BackButtonDown);
            _resultShareButton.GetComponent<Button>().onClick.AddListener(ShareButtonDown);
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
            _resultTitle.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameSuccess;
        }

        private void OnFail()
        {
            _resultTitle.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameFailure;
            // 失敗時は投稿ボタンを表示しない
            _resultShareButton.SetActive(false);
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
            SceneManager.LoadScene(SceneName.MENU_SELECT_SCENE);
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
            var text = "ステージ" + GamePlayDirector.stageId + "番を" + _resultTitle.GetComponent<Text>().text;
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

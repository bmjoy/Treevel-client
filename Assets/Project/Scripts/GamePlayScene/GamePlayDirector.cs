using System;
using System.Collections;
using System.Linq;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class GamePlayDirector : MonoBehaviour
    {
        private const string RESULT_WINDOW_NAME = "ResultWindow";
        private const string RESULT_NAME = "Result";
        private const string WARNING_NAME = "Warning";
        private const string STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string PAUSE_WINDOW_NAME = "PauseWindow";
        private const string PAUSE_BACKGROUND = "PauseBackground";
        private const string PAUSE_BUTTON_NAME = "PauseButton";

        public delegate void ChangeAction();

        /// <summary>
        /// 成功時のイベント
        /// </summary>
        public static event ChangeAction OnSucceed;

        /// <summary>
        /// 失敗時のイベント
        /// </summary>
        public static event ChangeAction OnFail;

        /// <summary>
        /// ゲームの状態一覧
        /// </summary>
        public enum EGameState {
            Opening,
            Playing,
            Success,
            Failure,
            Pausing
        }

        /// <summary>
        /// ステージ id
        /// </summary>
        public static int stageId;

        /// <summary>
        /// ゲームの現状態
        /// </summary>
        public EGameState state = EGameState.Opening;

        /// <summary>
        /// 結果ウィンドウ
        /// </summary>
        private GameObject _resultWindow;

        /// <summary>
        /// 結果ウィンドウ上の結果用テキスト
        /// </summary>
        private GameObject _resultText;

        /// <summary>
        /// 結果ウィンドウ上の警告用テキスト
        /// </summary>
        private GameObject _warningText;

        /// <summary>
        /// 一時停止ウィンドウ
        /// </summary>
        private GameObject _pauseWindow;

        /// <summary>
        /// 一時停止中の背景
        /// </summary>
        private GameObject _pauseBackground;

        /// <summary>
        /// 一時停止ボタン
        /// </summary>
        private GameObject _pauseButton;

        /// <summary>
        /// ステージ id 表示用のテキスト
        /// </summary>
        private GameObject _stageNumberText;

        /// <summary>
        /// プレイ中の BGM
        /// </summary>
        private AudioSource _playingAudioSource;

        /// <summary>
        /// 成功時の音
        /// </summary>
        private AudioSource _successAudioSource;

        /// <summary>
        /// 失敗時の音
        /// </summary>
        private AudioSource _failureAudioSource;

        private void Awake()
        {
            _resultWindow = GameObject.Find(RESULT_WINDOW_NAME);

            _resultText = _resultWindow.transform.Find(RESULT_NAME).gameObject;
            _warningText = _resultWindow.transform.Find(WARNING_NAME).gameObject;

            _stageNumberText = GameObject.Find(STAGE_NUMBER_TEXT_NAME);

            _pauseWindow = GameObject.Find(PAUSE_WINDOW_NAME);
            _pauseBackground = GameObject.Find(PAUSE_BACKGROUND);
            _pauseButton = GameObject.Find(PAUSE_BUTTON_NAME);

            UnifyDisplay(_resultWindow);

            SetAudioSources();
        }

        private void Start()
        {
            GameOpening();
        }

        /// <summary>
        /// アプリがバックグラウンドに移動する、または
        /// バックグラウンドから戻ってくる時に呼ばれる
        /// </summary>
        /// <param name="pauseStatus"></param>
        private void OnApplicationPause(bool pauseStatus)
        {
            // アプリがバックグラウンドに移動した時
            if (pauseStatus) {
                // 一時停止扱いとする
                Dispatch(EGameState.Pausing);
            }
        }

        /// <summary>
        /// アプリが終了した時に呼ばれる
        /// </summary>
        private void OnApplicationQuit()
        {
            // ゲーム失敗扱いとする
            Dispatch(EGameState.Failure);
        }

        /// <summary>
        /// ゲームがクリアしているかをチェックする
        /// </summary>
        public void CheckClear()
        {
            GameObject[] panels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_PANEL);
            if (panels.Any(panel => panel.GetComponent<NumberPanelController>().Adapted == false)) return;
            // 全ての数字パネルが最終位置にいたら，成功状態に遷移
            Dispatch(EGameState.Success);
        }

        /// <summary>
        /// ゲームの状態を変更する
        /// </summary>
        /// <param name="nextState"> 変更したい状態 </param>
        /// <returns> 変更に成功したかどうか </returns>
        public bool Dispatch(EGameState nextState)
        {
            switch (nextState) {
                case EGameState.Opening:
                    // `Success`と`Failure`からの遷移のみを許す
                    if (state == EGameState.Success || state == EGameState.Failure) {
                        state = nextState;
                        GameOpening();
                        return true;
                    }

                    break;
                case EGameState.Playing:
                    // `Opening`と`Pausing`からの遷移のみを許す
                    if (state == EGameState.Opening || state == EGameState.Pausing) {
                        state = nextState;
                        GamePlaying();
                        return true;
                    }

                    break;
                case EGameState.Success:
                    // `Playing`からの遷移のみ許す
                    if (state == EGameState.Playing) {
                        state = nextState;
                        GameSucceed();
                        return true;
                    }

                    break;
                case EGameState.Failure:
                    // `Playing`と`Pausing`からの遷移のみ許す
                    if (state == EGameState.Playing || state == EGameState.Pausing) {
                        state = nextState;
                        GameFail();
                        return true;
                    }

                    break;
                case EGameState.Pausing:
                    // `Playing`からの遷移のみ許す
                    if (state == EGameState.Playing) {
                        state = nextState;
                        GamePausing();
                        return true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
            }

            return false;
        }

        /// <summary>
        /// 起動状態の処理
        /// </summary>
        private void GameOpening()
        {
            CleanObject();

            // StageStatusのデバッグ用
            var stageStatus = StageStatus.Get(stageId);
            var tmp = stageStatus.passed ? "クリア済み" : "未クリア";
            print("-------------------------------");
            print("ステージ" + stageId + "番は" + tmp + "です");
            print("ステージ" + stageId + "番の挑戦回数は" + stageStatus.challengeNum + "回です");
            print("ステージ" + stageId + "番の成功回数は" + stageStatus.successNum + "回です");
            print("ステージ" + stageId + "番の失敗回数は" + stageStatus.failureNum + "回です");
            print("-------------------------------");

            // 現在のステージ番号を格納
            _stageNumberText.GetComponent<Text>().text = stageId.ToString();

            // 結果ウィンドウを非表示
            _resultWindow.SetActive(false);

            // 一時停止ウィンドウを非表示
            _pauseWindow.SetActive(false);
            // 一時停止背景を非表示
            _pauseBackground.SetActive(false);
            // 一時停止ボタンを有効にする
            _pauseButton.SetActive(true);

            // 警告ウィンドウを非表示
            _warningText.SetActive(false);

            // 番号に合わせたステージの作成
            StageGenerator.CreateStages(stageId);

            // 状態を変更する
            Dispatch(EGameState.Playing);
        }

        /// <summary>
        /// 開始状態の処理
        /// </summary>
        private void GamePlaying()
        {
            _playingAudioSource.Play();
        }

        /// <summary>
        /// 成功状態の処理
        /// </summary>
        private void GameSucceed()
        {
            if (OnSucceed != null) OnSucceed();
            EndProcess();
            _successAudioSource.Play();
            _resultText.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameSuccess;
            var ss = StageStatus.Get(stageId);
            // クリア済みにする
            ss.ClearStage(stageId);
            // 成功回数をインクリメント
            ss.IncSuccessNum(stageId);
        }

        /// <summary>
        /// 失敗状態の処理
        /// </summary>
        private void GameFail()
        {
            if (OnFail != null) OnFail();
            EndProcess();
            _resultText.GetComponent<MultiLanguageText>().TextIndex = ETextIndex.GameFailure;
            _failureAudioSource.Play();
            // 失敗回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncFailureNum(stageId);
        }

        /// <summary>
        /// 一時停止状態の処理
        /// </summary>
        private void GamePausing()
        {
            // ゲーム内の時間を一時停止する
            Time.timeScale = 0.0f;
            // 一時停止ボタンを無効にする
            _pauseButton.SetActive(false);
            // 一時停止ウィンドウを表示する
            _pauseWindow.SetActive(true);
            // 一時停止背景を表示する
            _pauseBackground.SetActive(true);
        }

        /// <summary>
        /// ゲーム終了時の共通処理
        /// </summary>
        private void EndProcess()
        {
            _playingAudioSource.Stop();
            _resultWindow.SetActive(true);
            // 一時停止ボタンを無効にする
            _pauseButton.SetActive(false);
        }

        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        public void RetryButtonDown()
        {
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncChallengeNum(stageId);
            Dispatch(EGameState.Opening);
        }

        /// <summary>
        /// 戻るボタン押下時の処理
        /// </summary>
        public void BackButtonDown()
        {
            // StageSelectSceneに戻る
            SceneManager.LoadScene(SceneName.MENU_SELECT_SCENE);
        }

        /// <summary>
        /// Twitter 投稿ボタン押下時の処理．
        /// </summary>
        public void ShareButtonDown()
        {
            print("For Debug");
            // 投稿用のテキスト
            var text = "ステージ" + stageId + "番を" + _resultText.GetComponent<Text>().text;
            // URL 用に加工
            text = UnityWebRequest.EscapeURL(text);

            // 投稿用のハッシュタグ
            var hashTags = "NumberBullet,ナンバレ";
            hashTags = UnityWebRequest.EscapeURL(hashTags);

            // Twitter 投稿画面へ
            Application.OpenURL("https://twitter.com/intent/tweet?text=" + text + "&hashtags=" + hashTags);
        }

        /// <summary>
        /// 一時停止ボタン押下時の処理
        /// </summary>
        public void PauseButtonDown()
        {
            Dispatch(EGameState.Pausing);
        }

        /// <summary>
        /// ゲーム再開ボタン押下時の処理
        /// </summary>
        public void PauseBackButtonDown()
        {
            // 一時停止ボタンを有効にする
            _pauseButton.SetActive(true);
            // 一時停止ウィンドウを非表示にする
            _pauseWindow.SetActive(false);
            // 一時停止背景を非表示にする
            _pauseBackground.SetActive(false);
            // ゲーム内の時間を元に戻す
            Time.timeScale = 1.0f;
            // ゲームプレイ状態に遷移する
            Dispatch(EGameState.Playing);
        }

        /// <summary>
        /// ゲームを諦めるボタン押下時の処理
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

        /// <summary>
        /// タイル・パネル・銃弾オブジェクトの削除
        /// </summary>
        private static void CleanObject()
        {
            GameObject[] tiles = GameObject.FindGameObjectsWithTag(TagName.TILE);
            foreach (var tile in tiles) {
                // タイルの削除 (に伴いパネルも削除される)
                DestroyImmediate(tile);
            }

            GameObject[] bullets = GameObject.FindGameObjectsWithTag(TagName.BULLET);
            foreach (var bullet in bullets) {
                // 銃弾の削除
                DestroyImmediate(bullet);
            }
        }

        /// <summary>
        /// ゲーム画面のアスペクト比を統一する
        /// </summary>
        /// <param name="resultWindow"> 結果ウィンドウ </param>
        /// Bug: ゲーム画面遷移時にカメラ範囲が狭くなることがある
        private static void UnifyDisplay(GameObject resultWindow)
        {
            // 想定するデバイスのアスペクト比
            const float targetRatio = WindowSize.WIDTH / WindowSize.HEIGHT;
            // 実際のデバイスのアスペクト比
            var currentRatio = (float) Screen.width / Screen.height;
            // 許容するアスペクト比の誤差
            const float aspectRatioError = 0.001f;
            if (currentRatio > targetRatio + aspectRatioError) {
                // 横長のデバイスの場合
                var ratio = targetRatio / currentRatio;
                var rectX = (1 - ratio) / 2f;
                Camera.main.rect = new Rect(rectX, 0f, ratio, 1f);
                // 結果ウィンドウも変える
                resultWindow.transform.localScale = new Vector2(ratio, ratio);
            } else if (currentRatio < targetRatio - aspectRatioError) {
                // 縦長のデバイスの場合
                var ratio = currentRatio / targetRatio;
                var rectY = (1 - ratio) / 2f;
                Camera.main.rect = new Rect(0f, rectY, 1f, ratio);
                // 結果ウィンドウも変える
                resultWindow.transform.localScale = new Vector2(ratio, ratio);
            }
        }

        /// <summary>
        /// 音源のセットアップ
        /// </summary>
        private void SetAudioSources()
        {
            // 各音源の設定
            // Playing
            gameObject.AddComponent<AudioSource>();
            _playingAudioSource = gameObject.GetComponents<AudioSource>()[0];
            SetAudioSource(clipName: ClipName.PLAYING, audioSource: _playingAudioSource, time: 2.0f, loop: true,
                volumeRate: 0.25f);
            // Success
            gameObject.AddComponent<AudioSource>();
            _successAudioSource = gameObject.GetComponents<AudioSource>()[1];
            SetAudioSource(clipName: ClipName.SUCCESS, audioSource: _successAudioSource);
            // Failure
            gameObject.AddComponent<AudioSource>();
            _failureAudioSource = gameObject.GetComponents<AudioSource>()[2];
            SetAudioSource(clipName: ClipName.FAILURE, audioSource: _failureAudioSource);
        }

        /// <summary>
        /// 個々の音源のセットアップ
        /// </summary>
        /// <param name="clipName"> 音源名 </param>
        /// <param name="audioSource"> 音源 </param>
        /// <param name="time"> 開始時間 </param>
        /// <param name="loop"> 繰り返しの有無 </param>
        /// <param name="volumeRate"> 設定音量に対する比率 </param>
        private static void SetAudioSource(string clipName, AudioSource audioSource, float time = 0.0f,
            bool loop = false, float volumeRate = 1.0f)
        {
            var clip = Resources.Load<AudioClip>("Clips/GamePlayScene/" + clipName);
            audioSource.clip = clip;
            audioSource.time = time;
            audioSource.loop = loop;
            audioSource.volume = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME, Audio.DEFAULT_VOLUME) * volumeRate;
        }
    }
}

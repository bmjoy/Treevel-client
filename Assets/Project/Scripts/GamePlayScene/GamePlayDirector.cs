using System;
using System.Collections;
using System.Linq;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.MenuSelectScene.Settings;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.GamePlayScene.Bullet.Generators;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class GamePlayDirector : MonoBehaviour
    {
        private const string _STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string _TIMER_TEXT_NAME = "TimerText";
        private const string _PAUSE_WINDOW_NAME = "PauseWindow";
        private const string _PAUSE_BUTTON_NAME = "PauseButton";
        private const string _SUCCESS_POPUP_NAME = "SuccessPopup";
        private const string _FAILURE_POPUP_NAME = "FailurePopup";

        /// <summary>
        /// 成功時のイベント
        /// </summary>
        public static event Action OnSucceed;

        /// <summary>
        /// 失敗時のイベント
        /// </summary>
        public static event Action OnFail;

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
        /// 木のレベル
        /// </summary>
        public static ELevelName levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        public static ETreeId treeId;

        /// <summary>
        /// ステージ id
        /// </summary>
        public static int stageId;

        /// <summary>
        /// ゲームの現状態
        /// </summary>
        public EGameState state = EGameState.Opening;

        /// <summary>
        /// 一時停止ウィンドウ
        /// </summary>
        private GameObject _pauseWindow;

        /// <summary>
        /// 一時停止ボタン
        /// </summary>
        private GameObject _pauseButton;

        /// <summary>
        /// ステージ id 表示用のテキスト
        /// </summary>
        private GameObject _stageNumberText;

        /// <summary>
        /// タイマー表示用のテキスト
        /// </summary>
        private GameObject _timerText;

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

        /// <summary>
        /// タイマー
        /// </summary>
        private CustomTimer _customTimer;

        /// <summary>
        /// 成功ポップアップ
        /// </summary>
        private GameObject _successPopup;

        /// <summary>
        /// 失敗ポップアップ
        /// </summary>
        private GameObject _failurePopup;

        private void Awake()
        {
            _stageNumberText = GameObject.Find(_STAGE_NUMBER_TEXT_NAME);
            _timerText = GameObject.Find(_TIMER_TEXT_NAME);

            _pauseWindow = GameObject.Find(_PAUSE_WINDOW_NAME);
            _pauseButton = GameObject.Find(_PAUSE_BUTTON_NAME);

            _successPopup = GameObject.Find(_SUCCESS_POPUP_NAME);
            _failurePopup = GameObject.Find(_FAILURE_POPUP_NAME);

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
            var panels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_PANEL);
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

            _successPopup.SetActive(false);
            _failurePopup.SetActive(false);

            // 現在のステージ番号を格納
            _stageNumberText.GetComponent<Text>().text = levelName.ToString() + "_" + treeId.ToString() + "_" + stageId.ToString();

            // 一時停止ウィンドウを非表示
            _pauseWindow.SetActive(false);
            // 一時停止ボタンを有効にする
            _pauseButton.SetActive(true);

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

            // 時間の計測
            if (GetComponent<CustomTimer>() == null) {
                _customTimer = gameObject.AddComponent<CustomTimer>();
                _customTimer.Initialize(_timerText.GetComponent<Text>());
            }
            _customTimer.StartTimer();

            // 銃弾の生成
            BulletGroupGenerator.Instance.FireBulletGroups();
        }

        /// <summary>
        /// 成功状態の処理
        /// </summary>
        private void GameSucceed()
        {
            EndProcess();
            _successPopup.SetActive(true);
            _successAudioSource.Play();
            var ss = StageStatus.Get(stageId);
            // クリア済みにする
            ss.ClearStage(stageId);
            // 成功回数をインクリメント
            ss.IncSuccessNum(stageId);
            OnSucceed?.Invoke();
        }

        /// <summary>
        /// 失敗状態の処理
        /// </summary>
        private void GameFail()
        {
            EndProcess();
            _failurePopup.SetActive(true);
            _failureAudioSource.Play();
            // 失敗回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncFailureNum(stageId);
            OnFail?.Invoke();
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
        }

        /// <summary>
        /// ゲーム終了時の共通処理
        /// </summary>
        private void EndProcess()
        {
            _playingAudioSource.Stop();
            // 一時停止ボタンを無効にする
            _pauseButton.SetActive(false);
            // タイマーを止める
            _customTimer.StopTimer();
        }

        /// <summary>
        /// 一時停止ボタン押下時の処理
        /// </summary>
        public void PauseButtonDown()
        {
            Dispatch(EGameState.Pausing);
        }

        /// <summary>
        /// タイル・パネル・銃弾オブジェクトの削除
        /// </summary>
        private static void CleanObject()
        {
            // パネルを破壊
            var panels = GameObject.FindObjectsOfType<PanelController>();
            foreach (var panel in panels) {
                // パネルの削除
                DestroyImmediate(panel.gameObject);
            }

            var bullets = GameObject.FindGameObjectsWithTag(TagName.BULLET);
            foreach (var bullet in bullets) {
                // 銃弾の削除
                DestroyImmediate(bullet);
            }
        }

        /// <summary>
        /// 音源のセットアップ
        /// </summary>
        private void SetAudioSources()
        {
            // 各音源の設定
            // loop,volumeは予めInspectorで設定
            var audioSources = gameObject.GetComponents<AudioSource>();
            foreach (var audioSource in audioSources) {
                switch (audioSource.clip.name) {
                    case AudioClipName.PLAYING:
                        _playingAudioSource = audioSource;
                        _playingAudioSource.time = 2.0f; // 再生ポイントを2秒からにする
                        break;

                    case AudioClipName.SUCCESS:
                        _successAudioSource = audioSource;
                        break;

                    case AudioClipName.FAILURE:
                        _failureAudioSource = audioSource;
                        break;

                    default:
                        throw new Exception("Clip name invalid");
                }

                // ユーザの音量設定
                audioSource.volume *= SettingsManager.BGMVolume;
            }
        }
    }
}

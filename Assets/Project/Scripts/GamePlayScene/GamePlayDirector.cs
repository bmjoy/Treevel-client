using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.MenuSelectScene.Settings;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.Patterns.StateMachine;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.Utils.Library;
using Project.Scripts.GamePlayScene.Bullet.Generators;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace Project.Scripts.GamePlayScene
{
    public class GamePlayDirector : SingletonObject<GamePlayDirector>
    {
        private const string _STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string _TIMER_TEXT_NAME = "TimerText";
        private const string _PAUSE_WINDOW_NAME = "PauseWindow";
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
            Playing, // 初期状態
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
        /// 各状態に対応するステートのインスタンス
        /// </summary>
        private readonly Dictionary<EGameState, State> _stateList = new Dictionary<EGameState, State>();

        /// <summary>
        /// ゲームの現状態
        /// </summary>
        public EGameState State
        {
            get {
                return _stateList.SingleOrDefault(pair => pair.Value == _stateMachine.CurrentState).Key;
            }
        }

        /// <summary>
        /// 状態遷移を管理するステートマシン
        /// </summary>
        private StateMachine _stateMachine;

        private void Awake()
        {
            // ステートマシン初期化
            foreach (var state in Enum.GetValues(typeof(EGameState))) {
                AddState((EGameState)state);
            }

            _stateMachine = new StateMachine(_stateList[EGameState.Playing], _stateList.Values.ToArray());

            // 可能の状態遷移を設定
            foreach (var state in Enum.GetValues(typeof(EGameState))) {
                AddTransition((EGameState)state);
            }

            BoardManager.Initialize();
        }

        private void Start()
        {
            _stateMachine.Start();
        }

        /// <summary>
        /// ステートの列挙からインスタンスを作成、辞書に保存する
        /// <see cref="https://stackoverflow.com/questions/3200875/how-to-instantiate-privatetype-of-inner-private-class/22700890">参考リンク</see>
        /// </summary>
        /// <param name="state">ステート</param>
        private void AddState(EGameState state)
        {
            // 親クラスを取得
            var parentType = typeof(GamePlayDirector);
            // ステートのタイプを取得
            var stateType = parentType.GetNestedType($"{state.ToString()}State", BindingFlags.NonPublic);

            // ステートのインスタンス生成
            var stateInstance = (State)Activator.CreateInstance(stateType, new object[] {this});

            _stateList.Add(state, stateInstance);
        }

        /// <summary>
        /// 可能な状態遷移をステートマシンに追加
        /// </summary>
        /// <param name="state">ステート</param>
        private void AddTransition(EGameState state)
        {
            switch (state) {
                case EGameState.Playing:
                    _stateMachine.AddTransition(_stateList[EGameState.Playing], _stateList[EGameState.Pausing]); // playing -> pausing
                    _stateMachine.AddTransition(_stateList[EGameState.Playing], _stateList[EGameState.Failure]); // playing -> faliure
                    _stateMachine.AddTransition(_stateList[EGameState.Playing], _stateList[EGameState.Success]); // playing -> success
                    break;
                case EGameState.Pausing:
                    _stateMachine.AddTransition(_stateList[EGameState.Pausing], _stateList[EGameState.Playing]); // pausing -> playing
                    _stateMachine.AddTransition(_stateList[EGameState.Pausing], _stateList[EGameState.Failure]); // pausing -> faliure
                    break;
                case EGameState.Failure:
                    _stateMachine.AddTransition(_stateList[EGameState.Failure], _stateList[EGameState.Playing]); // failure -> playing
                    break;
                case EGameState.Success:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid GameState: {state}");
            }
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
            if (!StageGenerator.CreatedFinished)
                return;

            var bottles = GameObject.FindObjectsOfType<AbstractBottleController>().OfType<IBottleSuccessHandler>();
            if (bottles.Any(bottle => bottle.IsSuccess() == false)) return;
            // 全ての数字ボトルが最終位置にいたら，成功状態に遷移
            Dispatch(EGameState.Success);
        }

        /// <summary>
        /// ゲームの状態を変更する
        /// </summary>
        /// <param name="nextState"> 変更したい状態 </param>
        public void Dispatch(EGameState nextState)
        {
            if (!_stateList.ContainsKey(nextState)) {
                throw new ArgumentOutOfRangeException($"Invalid State [{nextState}]");
            }

            _stateMachine.SetState(_stateList[nextState]);
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
            var bottles = GameObject.FindObjectsOfType<AbstractBottleController>();
            foreach (var bottle in bottles) {
                // パネルの削除
                DestroyImmediate(bottle.gameObject);
            }

            var bullets = GameObject.FindGameObjectsWithTag(TagName.BULLET);
            foreach (var bullet in bullets) {
                // 銃弾の削除
                DestroyImmediate(bullet);
            }
        }

        private class PlayingState: State
        {
            /// <summary>
            /// プレイ中の BGM
            /// </summary>
            private readonly AudioSource _playingBGM;

            /// <summary>
            /// ゲーム時間を計測するタイマー
            /// </summary>
            private readonly CustomTimer _customTimer;

            /// <summary>
            /// タイマー用テキスト
            /// </summary>
            private readonly Text _timerText;

            /// <summary>
            /// ステージID表示用テキスト
            /// </summary>
            private readonly Text _stageNumberText;

            public PlayingState(GamePlayDirector caller)
            {
                // BGM設定
                _playingBGM = caller.GetComponents<AudioSource>().SingleOrDefault(audioSource => audioSource.clip.name == AudioClipName.PLAYING);
                if (_playingBGM != null) {
                    _playingBGM.time = 2.0f;
                    _playingBGM.volume *= SettingsManager.BGMVolume;
                }

                // タイマー設定
                _timerText = GameObject.Find(_TIMER_TEXT_NAME).GetComponent<Text>();
                _customTimer = caller.gameObject.AddComponent<CustomTimer>();
                _customTimer.Initialize(_timerText);

                // ステージID表示
                _stageNumberText = GameObject.Find(_STAGE_NUMBER_TEXT_NAME).GetComponent<Text>();
                _stageNumberText.text = levelName.ToString() + "_" + treeId.ToString() + "_" + stageId.ToString();
            }

            public override void OnEnter(State from = null)
            {
                // 一時停止から戻る時はステージ再作成しない
                if (!(from is PausingState)) {
                    CleanObject();
                    StageInitialize();
                }

                _playingBGM.Play();
            }

            public override void OnExit(State to)
            {
                // 一時停止だったらそのまま処理終わる
                if (to is PausingState)
                    return;

                // その他の状態に遷移する時ゲーム終了
                EndProcess();
            }

            /// <summary>
            /// ゲーム始まる前の前処理
            /// </summary>
            private void StageInitialize()
            {
                // 番号に合わせたステージの作成
                StageGenerator.CreateStages(stageId);

                // 時間の計測
                _customTimer.StartTimer();

                // 銃弾の生成
                BulletGroupGenerator.Instance.FireBulletGroups();
            }

            /// <summary>
            /// ゲーム終了時の共通処理
            /// </summary>
            private void EndProcess()
            {
                _customTimer.StopTimer();
                _playingBGM.Stop();
            }
        }

        private class PausingState: State
        {
            /// <summary>
            /// 一時停止ウィンドウ
            /// </summary>
            private readonly GameObject _pauseWindow;

            public PausingState(GamePlayDirector caller)
            {
                _pauseWindow = GameObject.Find(_PAUSE_WINDOW_NAME);
                _pauseWindow.SetActive(false);
            }

            public override void OnEnter(State from = null)
            {
                // ゲーム内の時間を一時停止する
                Time.timeScale = 0.0f;
                // 一時停止ポップアップ表示
                _pauseWindow.SetActive(true);
            }

            public override void OnExit(State to)
            {
                // ゲーム内の時間を元に戻す
                Time.timeScale = 1.0f;

                if (!(to is FailureState)) {
                    // 一時停止ウィンドウを非表示にする
                    _pauseWindow.SetActive(false);
                }
            }
        }

        private class SuccessState: State
        {
            /// <summary>
            /// 成功ポップアップ
            /// </summary>
            private readonly GameObject _successPopup;

            /// <summary>
            /// 成功時の音
            /// </summary>
            private readonly AudioSource _successSE;

            public SuccessState(GamePlayDirector caller)
            {
                // 成功ポップアップ設定
                _successPopup = GameObject.Find(_SUCCESS_POPUP_NAME);
                _successPopup.SetActive(false);

                // 成功効果音設定
                _successSE = caller.GetComponents<AudioSource>().SingleOrDefault(se => se.clip.name == AudioClipName.SUCCESS);
                if (_successSE != null) _successSE.volume *= SettingsManager.SEVolume;
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                StageStatus.Get(stageId).Update(success: true);

                _successSE.Play();

                // 成功ポップアップ表示
                _successPopup.SetActive(true);

                // 成功イベント
                OnSucceed?.Invoke();
            }

            public override void OnExit(State to)
            {
                _successPopup.SetActive(false);
            }
        }

        private class FailureState: State
        {
            /// <summary>
            /// 失敗ポップアップ
            /// </summary>
            private readonly GameObject _failurePopup;

            /// <summary>
            /// 失敗時の音
            /// </summary>
            private readonly AudioSource _failureSE;

            public FailureState(GamePlayDirector caller)
            {
                // 失敗ポップアップ設定
                _failurePopup = GameObject.Find(_FAILURE_POPUP_NAME);
                _failurePopup.SetActive(false);

                // 失敗効果音設定
                _failureSE = caller.GetComponents<AudioSource>().SingleOrDefault(se => se.clip.name == AudioClipName.FAILURE);
                if (_failureSE != null) _failureSE.volume *= SettingsManager.SEVolume;
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                StageStatus.Get(stageId).Update(success: false);

                // Pausingから来たらステージ選択画面へ
                if (from is PausingState) {
                    // StageSelectSceneに戻る
                    TreeLibrary.LoadStageSelectScene(levelName);
                } else {
                    // 失敗SE
                    _failureSE.Play();

                    // 失敗ポップアップを表示
                    _failurePopup.SetActive(true);

                    // 失敗イベント
                    OnFail?.Invoke();
                }
            }

            public override void OnExit(State to)
            {
                _failurePopup.SetActive(false);
            }
        }
    }
}

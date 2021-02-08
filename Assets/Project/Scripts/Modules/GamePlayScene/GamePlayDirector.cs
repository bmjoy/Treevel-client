using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Treevel.Common.Attributes;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Patterns.StateMachine;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using Treevel.Modules.GamePlayScene.Gimmick;
using Treevel.Modules.GamePlayScene.Tile;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Treevel.Modules.GamePlayScene
{
    public class GamePlayDirector : SingletonObject<GamePlayDirector>
    {
        /// <summary>
        /// FPS
        /// </summary>
        public static readonly int FRAME_RATE = (int)Mathf.Round(1.0f / Time.fixedDeltaTime);

        private const float _BGM_VOLUME_RATIO_ON_PAUSE = 0.5f;

        private const string _STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string _TIMER_TEXT_NAME = "TimerText";
        private const string _PAUSE_WINDOW_NAME = "PauseWindow";
        private const string _PAUSE_BACKGROUND_NAME = "PauseBackground";
        private const string _SUCCESS_POPUP_NAME = "SuccessPopup";
        private const string _FAILURE_POPUP_NAME = "FailurePopup";

        [SerializeField] private GameObject _tutorialWindow;

        /// <summary>
        /// ゲーム開始時のイベント
        /// </summary>
        public IObservable<Unit> GameStart => _gameStartSubject;

        private readonly Subject<Unit> _gameStartSubject = new Subject<Unit>();

        /// <summary>
        /// ゲーム終了時のイベント
        /// </summary>
        public IObservable<Unit> GameEnd => _gameSucceededSubject.Merge(_gameFailedSubject);

        /// <summary>
        /// 成功時のイベント
        /// </summary>
        public IObservable<Unit> GameSucceeded => _gameSucceededSubject;

        private readonly Subject<Unit> _gameSucceededSubject = new Subject<Unit>();

        /// <summary>
        /// 失敗時のイベント
        /// </summary>
        public IObservable<Unit> GameFailed => _gameFailedSubject;

        private readonly Subject<Unit> _gameFailedSubject = new Subject<Unit>();

        /// <summary>
        /// ゲームの状態一覧
        /// </summary>
        public enum EGameState
        {
            Tutorial,
            Opening,
            Playing,
            Success,
            Failure,
            Pausing,
        }

        /// <summary>
        /// 木の季節
        /// </summary>
        public static ESeasonId seasonId;

        /// <summary>
        /// 木のId
        /// </summary>
        public static ETreeId treeId;

        /// <summary>
        /// ステージ id
        /// </summary>
        public static int stageNumber;

        /// <summary>
        /// 失敗原因を保持
        /// </summary>
        public EFailureReasonType failureReason = EFailureReasonType.Others;

        /// <summary>
        /// 各状態に対応するステートのインスタンス
        /// </summary>
        private readonly Dictionary<EGameState, State> _stateList = new Dictionary<EGameState, State>();

        /// <summary>
        /// ゲームの現状態
        /// </summary>
        public EGameState State {
            get { return _stateList.SingleOrDefault(pair => pair.Value == _stateMachine.CurrentState).Key; }
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

            var startState = ShouldShowTutorial() ? _stateList[EGameState.Tutorial] : _stateList[EGameState.Opening];

            _stateMachine = new StateMachine(startState, _stateList.Values.ToArray());

            // 可能の状態遷移を設定
            foreach (var state in Enum.GetValues(typeof(EGameState))) {
                AddTransition((EGameState)state);
            }
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
            var stateInstance = (State)Activator.CreateInstance(stateType, new object[] { this });

            _stateList.Add(state, stateInstance);
        }

        /// <summary>
        /// 可能な状態遷移をステートマシンに追加
        /// </summary>
        /// <param name="state">ステート</param>
        private void AddTransition(EGameState state)
        {
            switch (state) {
                case EGameState.Tutorial:
                    _stateMachine.AddTransition(_stateList[EGameState.Tutorial],
                                                _stateList[EGameState.Opening]); // tutorial -> opening
                    break;
                case EGameState.Opening:
                    _stateMachine.AddTransition(_stateList[EGameState.Opening],
                                                _stateList[EGameState.Playing]); // opening -> playing
                    break;
                case EGameState.Playing:
                    _stateMachine.AddTransition(_stateList[EGameState.Playing],
                                                _stateList[EGameState.Pausing]); // playing -> pausing
                    _stateMachine.AddTransition(_stateList[EGameState.Playing],
                                                _stateList[EGameState.Failure]); // playing -> faliure
                    _stateMachine.AddTransition(_stateList[EGameState.Playing],
                                                _stateList[EGameState.Success]); // playing -> success
                    break;
                case EGameState.Pausing:
                    _stateMachine.AddTransition(_stateList[EGameState.Pausing],
                                                _stateList[EGameState.Playing]); // pausing -> playing
                    _stateMachine.AddTransition(_stateList[EGameState.Pausing],
                                                _stateList[EGameState.Failure]); // pausing -> faliure
                    break;
                case EGameState.Failure:
                    _stateMachine.AddTransition(_stateList[EGameState.Failure],
                                                _stateList[EGameState.Opening]); // failure -> opening
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
            failureReason = EFailureReasonType.Others;
            Dispatch(EGameState.Failure);
        }

        /// <summary>
        /// ゲームがクリアしているかをチェックする
        /// </summary>
        public void CheckClear()
        {
            if (!StageGenerator.CreatedFinished) return;

            var bottles = FindObjectsOfType<NormalBottleController>();
            if (bottles.Any(bottle => bottle.IsSuccess() == false)) return;

            // 全ての成功判定が付くボトルが成功の場合，成功状態に遷移
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
        /// Button OnClickに設定するためのラッパー
        /// </summary>
        [EnumAction(typeof(EGameState))]
        public void Dispatch(int nextState)
        {
            Dispatch((EGameState)nextState);
        }

        /// <summary>
        /// 一時停止ボタン押下時の処理
        /// </summary>
        public void PauseButtonDown()
        {
            Dispatch(EGameState.Pausing);
        }

        /// <summary>
        /// タイル・ボトル・銃弾オブジェクトの削除
        /// </summary>
        private static void CleanObject()
        {
            // ボトルを破壊
            var bottles = FindObjectsOfType<AbstractBottleController>();
            foreach (var bottle in bottles) {
                // ボトルの削除
                DestroyImmediate(bottle.gameObject);
            }

            var gimmicks = GameObject.FindGameObjectsWithTag(Constants.TagName.GIMMICK);
            foreach (var gimmick in gimmicks) {
                // 銃弾の削除
                DestroyImmediate(gimmick);
            }

            // ノーマルタイル以外のタイルを削除
            var specialTiles = FindObjectsOfType<AbstractTileController>().Where(t => !(t is NormalTileController));
            foreach (var tile in specialTiles) {
                DestroyImmediate(tile.gameObject);
            }
        }

        /// <summary>
        /// チュートリアルを表示するかどうか
        /// </summary>
        /// <returns>
        /// チュートリアルがない -> `false`
        /// チュートリアルがある
        ///     -> 見たことある -> `false`
        ///     -> 見たことない -> `true`
        /// </returns>
        private bool ShouldShowTutorial()
        {
            var stageData = GameDataManager.GetStage(treeId, stageNumber);
            if (stageData.Tutorial.type == ETutorialType.None) return false;

            var stageStatus = StageStatus.Get(treeId, stageNumber);
            return !stageStatus.tutorialChecked;
        }

        private class OpeningState : State
        {
            /// <summary>
            /// ステージID表示用テキスト
            /// </summary>
            private readonly Text _stageNumberText;

            /// <summary>
            /// ゲーム時間を計測するタイマー
            /// </summary>
            private readonly CustomTimer _customTimer;

            /// <summary>
            /// タイマー用テキスト
            /// </summary>
            private readonly Text _timerText;

            public OpeningState(GamePlayDirector caller)
            {
                // TODO: ステージTextを適切に配置する
                // ステージID表示
                _stageNumberText = GameObject.Find(_STAGE_NUMBER_TEXT_NAME).GetComponent<Text>();
                _stageNumberText.text = seasonId + "_" + treeId + "_" + stageNumber;

                // タイマー設定
                _timerText = GameObject.Find(_TIMER_TEXT_NAME).GetComponent<Text>();
                _customTimer = caller.gameObject.AddComponent<CustomTimer>();
                _customTimer.Initialize(_timerText);
            }

            public override void OnEnter(State from = null)
            {
                // TODO: ステージ準備中のアニメーションを用意する
                CleanObject();
                StageInitialize();
            }

            public override void OnExit(State to)
            {
                // TODO: ステージ準備中のアニメーションを停止する
                // 時間の計測
                _customTimer.StartTimer();

                // ゲーム開始時のイベント
                Instance._gameStartSubject.OnNext(Unit.Default);

                // ギミックの発火
                GimmickGenerator.Instance.FireGimmick();

                // BGMの再生
                SoundManager.Instance.PlayBGM(EBGMKey.GamePlay_Spring, 2.0f);
            }

            /// <summary>
            /// ゲーム始まる前の前処理
            /// </summary>
            private void StageInitialize()
            {
                // BoardManagerの初期化
                BoardManager.Instance.Initialize();
                // 番号に合わせたステージの作成
                StageGenerator.CreateStages(treeId, stageNumber);
            }
        }

        private class PlayingState : State
        {
            /// <summary>
            /// ゲーム時間を計測するタイマー
            /// </summary>
            private readonly CustomTimer _customTimer;

            public PlayingState(GamePlayDirector caller)
            {
                _customTimer = caller.gameObject.GetComponent<CustomTimer>();
            }

            public override void OnEnter(State from = null)
            {
                // todo: 暫定で10が難しいステージのBGMを流す
                if (stageNumber == 10)
                    SoundManager.Instance.PlayBGM(EBGMKey.GamePlay_Difficult);
                else
                    SoundManager.Instance.PlayBGM(seasonId.GetGamePlayBGM());
            }

            public override void OnExit(State to)
            {
                // 一時停止だったらそのまま処理終わる
                if (to is PausingState) return;

                // その他の状態に遷移する時ゲーム終了
                EndProcess();
            }

            /// <summary>
            /// ゲーム終了時の共通処理
            /// </summary>
            private void EndProcess()
            {
                _customTimer.StopTimer();
                SoundManager.Instance.StopBGM();

                // フリック回数の取得
                var bottles = FindObjectsOfType<DynamicBottleController>();
                var flickNum = bottles.Select(bottle => bottle.flickNum).Sum();

                // フリック回数の保存
                var stageStatus = StageStatus.Get(treeId, stageNumber);
                stageStatus.AddFlickNum(treeId, stageNumber, flickNum);
            }
        }

        private class PausingState : State
        {
            /// <summary>
            /// 一時停止ウィンドウ
            /// </summary>
            private readonly GameObject _pauseWindow;

            /// <summary>
            /// 一時停止背景
            /// </summary>
            private readonly GameObject _pauseBackground;

            public PausingState(GamePlayDirector caller)
            {
                _pauseWindow = GameObject.Find(_PAUSE_WINDOW_NAME);
                _pauseWindow.SetActive(false);
                _pauseBackground = GameObject.Find(_PAUSE_BACKGROUND_NAME);
                _pauseBackground.SetActive(false);
            }

            public override void OnEnter(State from = null)
            {
                // ゲーム内の時間を一時停止する
                Time.timeScale = 0.0f;
                // ポーズ中は BGM を少し下げる
                SoundManager.Instance.ChangeBGMVolume(_BGM_VOLUME_RATIO_ON_PAUSE);
                // 一時停止ポップアップ表示
                _pauseWindow.SetActive(true);
                _pauseBackground.SetActive(true);
            }

            public override void OnExit(State to)
            {
                // ゲーム内の時間を元に戻す
                Time.timeScale = 1.0f;
                // BGM を元に戻す
                SoundManager.Instance.ChangeBGMVolume(1 / _BGM_VOLUME_RATIO_ON_PAUSE);
                if (!(to is FailureState)) {
                    // 一時停止ウィンドウを非表示にする
                    _pauseWindow.SetActive(false);
                    _pauseBackground.SetActive(false);
                }
            }
        }

        private class SuccessState : State
        {
            /// <summary>
            /// 成功ポップアップ
            /// </summary>
            private readonly GameObject _successPopup;

            public SuccessState(GamePlayDirector caller)
            {
                // 成功ポップアップ設定
                _successPopup = GameObject.Find(_SUCCESS_POPUP_NAME);
                _successPopup.SetActive(false);
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                StageStatus.Get(treeId, stageNumber).Update(success: true);

                SoundManager.Instance.PlaySE(ESEKey.SE_Success);

                // 成功ポップアップ表示
                _successPopup.SetActive(true);

                // 成功イベント
                Instance._gameSucceededSubject.OnNext(Unit.Default);
            }

            public override void OnExit(State to)
            {
                _successPopup.SetActive(false);
            }
        }

        private class FailureState : State
        {
            /// <summary>
            /// 失敗ポップアップ
            /// </summary>
            private readonly GameObject _failurePopup;

            public FailureState(GamePlayDirector caller)
            {
                // 失敗ポップアップ設定
                _failurePopup = GameObject.Find(_FAILURE_POPUP_NAME);
                _failurePopup.SetActive(false);
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                StageStatus.Get(treeId, stageNumber).Update(success: false);

                // 失敗原因を保存
                var dic = RecordData.Instance.FailureReasonCount;
                if (dic.ContainsKey(Instance.failureReason)) {
                    dic[Instance.failureReason]++;
                } else {
                    dic[Instance.failureReason] = 1;
                }

                RecordData.Instance.FailureReasonCount = dic;

                // Pausingから来たらステージ選択画面へ
                if (from is PausingState) {
                    // StageSelectSceneに戻る
                    AddressableAssetManager.LoadScene(seasonId.GetSceneName());
                } else {
                    // 失敗SE
                    SoundManager.Instance.PlaySE(ESEKey.SE_Failure);

                    // 失敗ポップアップを表示
                    _failurePopup.SetActive(true);

                    // 失敗イベント
                    Instance._gameFailedSubject.OnNext(Unit.Default);
                }
            }

            public override void OnExit(State to)
            {
                _failurePopup.SetActive(false);
            }
        }

        private class TutorialState : State
        {
            private readonly GameObject _tutorialWindow;

            public TutorialState(GamePlayDirector caller)
            {
                _tutorialWindow = caller._tutorialWindow;
            }

            public override void OnEnter(State from = null)
            {
                SoundManager.Instance.PlayBGM(EBGMKey.GamePlay_Tutorial);

                var stageData = GameDataManager.GetStage(treeId, stageNumber);
                var tutorialData = stageData.Tutorial;
                if (tutorialData.type == ETutorialType.None) return;

                var content = _tutorialWindow.transform.Find("Content");
                if (tutorialData.type == ETutorialType.Image) {
                    var imageAssetReference = tutorialData.image;
                    imageAssetReference.LoadAssetAsync<Texture2D>().Completed += handle => {
                        var image = content.GetComponent<RawImage>();
                        image.texture = handle.Result;
                        _tutorialWindow.SetActive(true);
                    };
                } else if (tutorialData.type == ETutorialType.Video) {
                    var videoAssetReference = tutorialData.video;
                    videoAssetReference.LoadAssetAsync<VideoClip>().Completed += handle => {
                        var videoPlayer = content.GetComponent<VideoPlayer>();

                        videoPlayer.clip = handle.Result;
                        videoPlayer.enabled = true;
                        _tutorialWindow.SetActive(true);
                        videoPlayer.Play();
                    };
                }
            }

            public override void OnExit(State to)
            {
                var stageStatus = StageStatus.Get(treeId, stageNumber);
                stageStatus.SetTutorialChecked(true);
                _tutorialWindow.SetActive(false);

                // OpeningState はBGMを流さないため止めとく
                SoundManager.Instance.StopBGM();
            }
        }
    }
}

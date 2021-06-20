using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
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
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Treevel.Modules.GamePlayScene
{
    public class GamePlayDirector : SingletonObjectBase<GamePlayDirector>
    {
        private const float _BGM_VOLUME_RATIO_ON_PAUSE = 0.5f;

        private const string _STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string _TIMER_TEXT_NAME = "TimerText";
        private const string _SUCCESS_PANEL_NAME = "SuccessPanel";
        private const string _FAILURE_POPUP_NAME = "FailurePopup";

        [SerializeField] private GameObject _tutorialWindow;

        /// <summary>
        /// ゲーム画面の手前のマスクオブジェクト
        /// </summary>
        [SerializeField] private GameObject _gameMask;

        /// <summary>
        /// 一時停止中の背景
        /// </summary>
        [SerializeField] private GameObject _pauseBackground;

        /// <summary>
        /// 一時停止ボタン
        /// </summary>
        [SerializeField] private GameObject _pauseButton;

        /// <summary>
        /// 一時停止ポップアップ
        /// </summary>
        [SerializeField] private GameObject _pauseWindow;

        /// <summary>
        /// カウントダウン演出用オブジェクト
        /// </summary>
        [SerializeField] private GameObject _countDownObject;

        /// <summary>
        /// ステージ準備完了時のイベント
        /// </summary>
        public IObservable<Unit> StagePrepared => _stagePreparedSubject;

        private readonly Subject<Unit> _stagePreparedSubject = new Subject<Unit>();

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
            Preparing,
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
        private readonly Dictionary<EGameState, StateBase> _stateList = new Dictionary<EGameState, StateBase>();

        /// <summary>
        /// ステージの記録を保持
        /// </summary>
        private StageRecord _stageRecord;

        /// <summary>
        /// ステージの情報を保持
        /// </summary>
        private StageData _stageData;

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

        /// <summary>
        /// リトライしているか
        /// </summary>
        public bool IsRetry { get; private set; }

        private void Awake()
        {
            _stageRecord = StageRecordService.Instance.Get(treeId, stageNumber);
            _stageData = GameDataManager.GetStage(treeId, stageNumber);

            // ステートマシン初期化
            foreach (var state in Enum.GetValues(typeof(EGameState))) {
                AddState((EGameState)state);
            }

            var shouldShowTutorial = _stageData.Tutorial.type != ETutorialType.None && !_stageRecord.tutorialChecked;
            var startState = shouldShowTutorial ? _stateList[EGameState.Tutorial] : _stateList[EGameState.Preparing];

            _stateMachine = new StateMachine(startState, _stateList.Values);

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
            var stateInstance = (StateBase)Activator.CreateInstance(stateType, new object[] { this });

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
                                                _stateList[EGameState.Preparing]); // tutorial -> preparing
                    break;
                case EGameState.Preparing:
                    _stateMachine.AddTransition(_stateList[EGameState.Preparing],
                                                _stateList[EGameState.Playing]); // preparing -> playing
                    _stateMachine.AddTransition(_stateList[EGameState.Preparing],
                                                _stateList[EGameState.Opening]); // preparing -> opening
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
                                                _stateList[EGameState.Preparing]); // failure -> preparing
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
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            Dispatch(EGameState.Pausing);
        }

        /// <summary>
        /// タイル・ボトル・銃弾オブジェクトの削除
        /// </summary>
        private static void CleanObject()
        {
            // ボトルを削除
            var bottles = FindObjectsOfType<BottleControllerBase>();
            foreach (var bottle in bottles) {
                DestroyImmediate(bottle.gameObject);
            }

            // ギミックを削除
            var gimmicks = GameObject.FindGameObjectsWithTag(Constants.TagName.GIMMICK);
            foreach (var gimmick in gimmicks) {
                DestroyImmediate(gimmick);
            }

            // ギミックの警告を削除
            var gimmickWarnings = GameObject.FindGameObjectsWithTag(Constants.TagName.GIMMICK_WARNING);
            foreach (var gimmickWarning in gimmickWarnings) {
                DestroyImmediate(gimmickWarning);
            }

            // タイルを削除
            var specialTiles = FindObjectsOfType<TileControllerBase>();
            foreach (var tile in specialTiles) {
                DestroyImmediate(tile.gameObject);
            }
        }

        private class PreparingState : StateBase
        {
            /// <summary>
            /// ステージID表示用テキスト
            /// </summary>
            private readonly Text _stageNumberText;

            public PreparingState(GamePlayDirector caller)
            {
                // TODO: ステージTextを適切に配置する
                // ステージID表示
                _stageNumberText = GameObject.Find(_STAGE_NUMBER_TEXT_NAME).GetComponent<Text>();
                _stageNumberText.text = seasonId + "_" + treeId + "_" + stageNumber;
            }

            public override void OnEnter(StateBase from = null)
            {
                Instance.IsRetry = from is FailureState;

                CleanObject();
                StageInitialize();
            }

            public override void OnExit(StateBase to)
            {
                // ステージ準備完了時のイベント
                Instance._stagePreparedSubject.OnNext(Unit.Default);
            }

            /// <summary>
            /// ゲーム始まる前の前処理
            /// </summary>
            private void StageInitialize()
            {
                // BoardManagerの初期化
                BoardManager.Instance.Initialize();
                // 番号に合わせたステージの作成
                StageGenerator.CreateStagesAsync(treeId, stageNumber).Forget();
            }
        }

        private class PlayingState : StateBase
        {
            /// <summary>
            /// ゲーム時間を計測するタイマー
            /// </summary>
            private readonly CustomTimer _customTimer;

            public PlayingState(GamePlayDirector caller)
            {
                _customTimer = caller.gameObject.GetComponent<CustomTimer>();

                // タイマー設定
                var timerText = GameObject.Find(_TIMER_TEXT_NAME).GetComponent<Text>();
                _customTimer = caller.gameObject.AddComponent<CustomTimer>();
                _customTimer.Initialize(timerText);
            }

            public override void OnEnter(StateBase from = null)
            {
                // 一時停止だったらそのまま処理終わる
                if (from is PausingState) return;

                // ゲーム画面のマスクを非表示にする
                Instance._gameMask.SetActive(false);

                // 時間の計測
                _customTimer.StartTimer();

                // ゲーム開始時のイベント
                Instance._gameStartSubject.OnNext(Unit.Default);

                // ギミックの発火
                GimmickGenerator.Instance.FireGimmick();

                Instance._stageRecord.challengeNum++;

                // todo: 暫定で10が難しいステージのBGMを流す
                if (stageNumber == 10)
                    SoundManager.Instance.PlayBGM(EBGMKey.GamePlay_Difficult);
                else
                    SoundManager.Instance.PlayBGM(seasonId.GetGamePlayBGM());

                // 一時停止ボタンを表示する
                Instance._pauseButton.SetActive(true);
            }

            public override void OnExit(StateBase to)
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
                SoundManager.Instance.StopBGMAsync();
                // 一時停止ボタンを非表示にする
                Instance._pauseButton.SetActive(false);

                // フリック回数の取得
                var bottles = FindObjectsOfType<DynamicBottleController>();
                var flickNum = bottles.Select(bottle => bottle.flickNum).Sum();

                Instance._stageRecord.flickNum += flickNum;
            }
        }

        private class PausingState : StateBase
        {
            /// <summary>
            /// バナー広告のビュー
            /// </summary>
            private BannerView _banner;

            public PausingState(GamePlayDirector caller) { }

            public override void OnEnter(StateBase from = null)
            {
                // 広告を表示
                _banner = AdvertiseService.ShowBanner(Constants.MobileAds.BANNER_UNIT_ID_GAME_PAUSED, AdPosition.Top);
                // ゲーム内の時間を一時停止する
                Time.timeScale = 0.0f;
                // ポーズ中は BGM を少し下げる
                SoundManager.Instance.SetMasterBGMVolume(_BGM_VOLUME_RATIO_ON_PAUSE);
                // 一時停止ポップアップ表示
                Instance._pauseWindow.SetActive(true);
                Instance._pauseBackground.SetActive(true);
                // 一時停止ボタン非表示
                Instance._pauseButton.SetActive(false);
            }

            public override void OnExit(StateBase to)
            {
                // 広告を削除
                _banner.Destroy();
                // ゲーム内の時間を元に戻す
                Time.timeScale = 1.0f;
                // BGM を元に戻す
                SoundManager.Instance.SetMasterBGMVolume(1.0f);
                if (!(to is FailureState)) {
                    // 一時停止ウィンドウを非表示にする
                    Instance._pauseWindow.SetActive(false);
                    Instance._pauseBackground.SetActive(false);
                    // 一時停止ボタンを表示する
                    Instance._pauseButton.SetActive(true);
                }
            }
        }

        private class SuccessState : StateBase
        {
            /// <summary>
            /// 成功ポップアップ
            /// </summary>
            private readonly GameObject _successPopup;

            public SuccessState(GamePlayDirector caller)
            {
                // 成功ポップアップ設定
                _successPopup = GameObject.Find(_SUCCESS_PANEL_NAME);
                _successPopup.SetActive(false);
            }

            public override async void OnEnter(StateBase from = null)
            {
                Instance._stageRecord.Succeed();

                SoundManager.Instance.PlaySE(ESEKey.GamePlay_Success);

                // 成功イベント
                Instance._gameSucceededSubject.OnNext(Unit.Default);

                try {
                    await StageRecordService.Instance.SaveAsync(treeId, stageNumber, Instance._stageRecord);
                } catch {
                    UIManager.Instance.CreateOkCancelMessageDialog(
                        ETextIndex.ReSendStageRecordDialogMessage,
                        ETextIndex.MessageDlgOkBtnRetryText,
                        ETextIndex.MessageDlgOkBtnGiveUpText,
                        async () => {
                            try {
                                await StageRecordService.Instance.SaveAsync(treeId, stageNumber, Instance._stageRecord);
                            } catch {
                                UIManager.Instance.ShowErrorMessageAsync(EErrorCode.SaveStageRecordError).Forget();
                            }
                        },
                        false);
                }

                // 成功盤面を一定時間表示
                // TODO: アニメーションの終了を待つ
                await Task.Delay(500);
                // 成功ポップアップ表示
                _successPopup.SetActive(true);
            }

            public override void OnExit(StateBase to)
            {
                _successPopup.SetActive(false);
            }
        }

        private class FailureState : StateBase
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

            public override async void OnEnter(StateBase from = null)
            {
                Instance._stageRecord.Fail();
                Instance._stageRecord.failureReasonNum.Increment(Instance.failureReason);
                
                // 通常の失敗時
                if (from is PlayingState) {
                    // 失敗SE
                    SoundManager.Instance.PlaySERandom(new[] { ESEKey.GamePlay_Failed_1, ESEKey.GamePlay_Failed_2 });

                    // 失敗イベント
                    Instance._gameFailedSubject.OnNext(Unit.Default);
                }

                await StageRecordService.Instance.SaveAsync(treeId, stageNumber, Instance._stageRecord);

                if (from is PlayingState) {
                    // 失敗盤面を一定時間表示
                    // TODO: アニメーションの終了を待つ
                    await Task.Delay(500);
                    // 失敗ポップアップを表示
                    _failurePopup.SetActive(true);
                }

                // Pausingから来たらステージ選択画面へ
                if (from is PausingState) {
                    // StageSelectSceneに戻る
                    AddressableAssetManager.LoadScene(seasonId.GetSceneName());
                }
            }

            public override void OnExit(StateBase to)
            {
                _failurePopup.SetActive(false);
            }
        }

        private class TutorialState : StateBase
        {
            private readonly GameObject _tutorialWindow;

            public TutorialState(GamePlayDirector caller)
            {
                _tutorialWindow = caller._tutorialWindow;
            }

            public override void OnEnter(StateBase from = null)
            {
                SoundManager.Instance.PlayBGM(EBGMKey.GamePlay_Tutorial);

                var stageData = GameDataManager.GetStage(treeId, stageNumber);
                var tutorialData = stageData.Tutorial;
                if (tutorialData.type == ETutorialType.None) return;

                var content = _tutorialWindow.transform.Find("Content");
                if (tutorialData.type == ETutorialType.Image) {
                    var imageAssetReference = tutorialData.image;
                    var image = content.GetComponent<RawImage>();
                    if (imageAssetReference.OperationHandle.IsValid()) {
                        image.texture = imageAssetReference.OperationHandle.Convert<Texture2D>().Result;
                        _tutorialWindow.SetActive(true);
                    } else {
                        imageAssetReference.LoadAssetAsync<Texture2D>().Completed += handle => {
                            image.texture = handle.Result;
                            _tutorialWindow.SetActive(true);
                        };
                    }
                } else if (tutorialData.type == ETutorialType.Video) {
                    var videoAssetReference = tutorialData.video;
                    var videoPlayer = content.GetComponent<VideoPlayer>();
                    if (videoAssetReference.IsValid()) {
                        videoPlayer.clip = videoAssetReference.OperationHandle.Convert<VideoClip>().Result;
                        videoPlayer.enabled = true;
                        _tutorialWindow.SetActive(true);
                        videoPlayer.Play();
                    } else {
                        videoAssetReference.LoadAssetAsync<VideoClip>().Completed += handle => {
                            videoPlayer.clip = handle.Result;
                            videoPlayer.enabled = true;
                            _tutorialWindow.SetActive(true);
                            videoPlayer.Play();
                        };
                    }
                }
            }

            public override void OnExit(StateBase to)
            {
                Instance._stageRecord.tutorialChecked = true;
                SoundManager.Instance.PlaySE(ESEKey.UI_Dropdown_Close);
                _tutorialWindow.SetActive(false);

                // PreparingState はBGMを流さないため止めとく
                SoundManager.Instance.StopBGMAsync();
            }
        }

        private class OpeningState : StateBase
        {
            private readonly Animator _animator;
            private static readonly int _ANIMATOR_PARAM_PLAY_COUNT_DOWN = Animator.StringToHash("PlayCountDown");
            private static readonly int _ANIMATOR_STATE_OPENING = Animator.StringToHash("Opening");
            public OpeningState(GamePlayDirector caller)
            {
                _animator = caller._countDownObject.GetComponent<Animator>();
                _animator.GetBehaviour<ObservableStateMachineTrigger>()
                    .OnStateExitAsObservable()
                    .Where(state => state.StateInfo.shortNameHash == _ANIMATOR_STATE_OPENING)
                    .Subscribe(_ => {
                        Instance.Dispatch(EGameState.Playing);
                    }) // アニメーション再生終了後プレイステートに移行
                    .AddTo(caller);
            }

            public override void OnEnter(StateBase from = null)
            {
                _animator.SetTrigger(_ANIMATOR_PARAM_PLAY_COUNT_DOWN);
            }
        }
    }
}

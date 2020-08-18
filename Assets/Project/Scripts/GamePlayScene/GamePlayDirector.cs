using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.Patterns.StateMachine;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using Project.Scripts.GameDatas;
using UnityEngine.Video;
using Project.Scripts.Utils.Attributes;
using Project.Scripts.GamePlayScene.Gimmick;

namespace Project.Scripts.GamePlayScene
{
    public class GamePlayDirector : SingletonObject<GamePlayDirector>
    {
        private const string _STAGE_NUMBER_TEXT_NAME = "StageNumberText";
        private const string _TIMER_TEXT_NAME = "TimerText";
        private const string _PAUSE_WINDOW_NAME = "PauseWindow";
        private const string _PAUSE_BACKGROUND_NAME = "PauseBackground";
        private const string _SUCCESS_POPUP_NAME = "SuccessPopup";
        private const string _FAILURE_POPUP_NAME = "FailurePopup";

        [SerializeField] private GameObject _tutorialWindow;

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
            Tutorial,
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

            var startState = ShouldShowTutorial() ? _stateList[EGameState.Tutorial] : _stateList[EGameState.Playing];

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
                case EGameState.Tutorial:
                    _stateMachine.AddTransition(_stateList[EGameState.Tutorial], _stateList[EGameState.Playing]); // tutorial -> playing
                    break;
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
            failureReason = EFailureReasonType.Others;
            Dispatch(EGameState.Failure);
        }

        /// <summary>
        /// ゲームがクリアしているかをチェックする
        /// </summary>
        public void CheckClear()
        {
            if (!StageGenerator.CreatedFinished)
                return;

            var bottles = GameObject.FindObjectsOfType<AbstractBottleController>();
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
            var bottles = GameObject.FindObjectsOfType<AbstractBottleController>();
            foreach (var bottle in bottles) {
                // ボトルの削除
                DestroyImmediate(bottle.gameObject);
            }

            var gimmicks = GameObject.FindGameObjectsWithTag(TagName.GIMMICK);
            foreach (var gimmick in gimmicks) {
                // 銃弾の削除
                DestroyImmediate(gimmick);
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
            var stageData = GameDataBase.GetStage(treeId, stageNumber);
            if (stageData.Tutorial.type == ETutorialType.None)
                return false;

            var stageStatus = StageStatus.Get(treeId, stageNumber);
            return !stageStatus.tutorialChecked;
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
                    _playingBGM.volume *= UserSettings.BGMVolume;
                }

                // タイマー設定
                _timerText = GameObject.Find(_TIMER_TEXT_NAME).GetComponent<Text>();
                _customTimer = caller.gameObject.AddComponent<CustomTimer>();
                _customTimer.Initialize(_timerText);

                // TODO: ステージTextを適切に配置する
                // ステージID表示
                _stageNumberText = GameObject.Find(_STAGE_NUMBER_TEXT_NAME).GetComponent<Text>();
                _stageNumberText.text = levelName.ToString() + "_" + treeId.ToString() + "_" + stageNumber.ToString();
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
                StageGenerator.CreateStages(treeId, stageNumber);

                // 時間の計測
                _customTimer.StartTimer();

                // ギミックの生成
                GimmickGenerator.Instance.FireGimmick();
            }

            /// <summary>
            /// ゲーム終了時の共通処理
            /// </summary>
            private void EndProcess()
            {
                _customTimer.StopTimer();
                _playingBGM.Stop();

                // フリック回数の取得
                var bottles = FindObjectsOfType<DynamicBottleController>();
                var flickNum = bottles.Select(bottle => bottle.FlickNum).Sum();

                // フリック回数の保存
                var stageStatus = StageStatus.Get(treeId, stageNumber);
                stageStatus.AddFlickNum(treeId, stageNumber, flickNum);

                // FIXME: マージ前に消す
                Debug.Log($"フリック回数：{flickNum}");
                Debug.Log($"合計フリック回数：{stageStatus.flickNum}");
            }
        }

        private class PausingState: State
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
                // 一時停止ポップアップ表示
                _pauseWindow.SetActive(true);
                _pauseBackground.SetActive(true);
            }

            public override void OnExit(State to)
            {
                // ゲーム内の時間を元に戻す
                Time.timeScale = 1.0f;

                if (!(to is FailureState)) {
                    // 一時停止ウィンドウを非表示にする
                    _pauseWindow.SetActive(false);
                    _pauseBackground.SetActive(false);
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
                if (_successSE != null) _successSE.volume *= UserSettings.SEVolume;
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                var stageStatus = StageStatus.Get(treeId, stageNumber);
                stageStatus.Update(success: true);

                Debug.Log($"初成功した日付：{stageStatus.firstSuccessAt}");

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
                if (_failureSE != null) _failureSE.volume *= UserSettings.SEVolume;
            }

            public override void OnEnter(State from = null)
            {
                // 記録更新
                StageStatus.Get(treeId, stageNumber).Update(success: false);

                // 失敗原因を保存
                var dic = RecordData.Instance.FailureReasonCount;
                if (dic.ContainsKey(Instance.failureReason)) {
                    dic[Instance.failureReason]++;
                }
                else {
                    dic[Instance.failureReason] = 1;
                }
                RecordData.Instance.FailureReasonCount = dic;

                // FIXME: マージ前に消す
                Debug.Log($"{Instance.failureReason}：{dic[Instance.failureReason]}");

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

        private class TutorialState : State
        {
            private readonly GameObject _tutorialWindow;

            public TutorialState(GamePlayDirector caller)
            {
                _tutorialWindow = caller._tutorialWindow;
            }

            public override void OnEnter(State from = null)
            {
                var stageData = GameDataBase.GetStage(treeId, stageNumber);
                var tutorialData = stageData.Tutorial;
                if (tutorialData.type == ETutorialType.None)
                    return;

                var content = _tutorialWindow.transform.Find("Content");
                if (tutorialData.type == ETutorialType.Image) {
                    var imageAssetReference = tutorialData.image;
                    imageAssetReference.LoadAssetAsync<Texture2D>().Completed += (handle) => {
                        var image = content.GetComponent<RawImage>();
                        image.texture = handle.Result;
                        _tutorialWindow.SetActive(true);
                    };
                } else if (tutorialData.type == ETutorialType.Video) {
                    var videoAssetReference = tutorialData.video;
                    videoAssetReference.LoadAssetAsync<VideoClip>().Completed += (handle) => {
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
            }
        }
    }
}

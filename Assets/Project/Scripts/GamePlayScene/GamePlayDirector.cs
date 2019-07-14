using System;
using System.Linq;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
	// TODO Singleton
	public class GamePlayDirector : MonoBehaviour
	{
		private const string STAGE_GENERATOR_NAME = "StageGenerator";
		private const string RESULT_WINDOW_NAME = "ResultWindow";
		private const string RESULT_NAME = "Result";
		private const string SUCCESS_TEXT = "成功！";
		private const string FAILURE_TEXT = "失敗！";
		private const string WARNING_NAME = "Warning";
		private const string WARNING_TEXT = "アプリが\nバックグラウンドに\n移動しました";
		private const string STAGE_NUMBER_TEXT_NAME = "StageNumberText";

		public delegate void ChangeAction();

		public static event ChangeAction OnFail;
		public static event ChangeAction OnSucceed;

		public enum GameState
		{
			Opening,
			Playing,
			Success,
			Failure
		}

		public static int stageId;

		public GameState state = GameState.Opening;

		private GameObject stageGenerator;

		private GameObject resultWindow;

		private GameObject resultText;

		private GameObject warningText;

		private GameObject stageNumberText;

		private AudioSource playingAudioSource;

		private AudioSource successAudioSource;

		private AudioSource failureAudioSource;

		private void Awake()
		{
			stageGenerator = GameObject.Find(STAGE_GENERATOR_NAME);

			resultWindow = GameObject.Find(RESULT_WINDOW_NAME);

			resultText = resultWindow.transform.Find(RESULT_NAME).gameObject;
			warningText = resultWindow.transform.Find(WARNING_NAME).gameObject;
			warningText.GetComponent<Text>().text = WARNING_TEXT;

			stageNumberText = GameObject.Find(STAGE_NUMBER_TEXT_NAME);

			UnifyDisplay(resultWindow);

			SetAudioSources();
		}

		private void Start()
		{
			GameOpening();
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus) // アプリがバックグラウンドに移動した時
			{
				if (Dispatch(GameState.Failure))
				{
					// 警告ウィンドウを表示
					warningText.SetActive(true);
				}
			}
		}

		/* ゲームのクリア判定 */
		public void CheckClear()
		{
			GameObject[] panels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_PANEL);
			if (panels.Any(panel => panel.GetComponent<NumberPanelController>().adapted == false)) return;
			// 全ての数字パネルが最終位置にいたら，成功状態に遷移
			Dispatch(GameState.Success);
		}

		/* 状態遷移 */
		public bool Dispatch(GameState nextState)
		{
			switch (nextState)
			{
				case GameState.Opening:
					// `Success`と`Failure`からの遷移のみを許す
					if (state == GameState.Success || state == GameState.Failure)
					{
						state = nextState;
						GameOpening();
						return true;
					}

					break;
				case GameState.Playing:
					// `Opening`からの遷移のみを許す
					if (state == GameState.Opening)
					{
						state = nextState;
						GamePlaying();
						return true;
					}

					break;
				case GameState.Success:
					// `Playing`からの遷移のみ許す
					if (state == GameState.Playing)
					{
						state = nextState;
						GameSucceed();
						return true;
					}

					break;
				case GameState.Failure:
					// `Playing`からの遷移のみ許す
					if (state == GameState.Playing)
					{
						state = nextState;
						GameFail();
						return true;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException("nextState", nextState, null);
			}

			return false;
		}

		/* ゲーム起動時 */
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
			stageNumberText.GetComponent<Text>().text = stageId.ToString();

			// 結果ウィンドウを非表示
			resultWindow.SetActive(false);

			// 警告ウィンドウを非表示
			warningText.SetActive(false);

			// 番号に合わせたステージの作成
			stageGenerator.GetComponent<StageGenerator>().CreateStages(stageId);

			// 状態を変更する
			Dispatch(GameState.Playing);
		}

		/* ゲーム開始時 */
		private void GamePlaying()
		{
			playingAudioSource.Play();
		}

		/* ゲーム成功時 */
		private void GameSucceed()
		{
			if (OnSucceed != null) OnSucceed();
			EndProcess();
			successAudioSource.Play();
			resultText.GetComponent<Text>().text = SUCCESS_TEXT;
			var ss = StageStatus.Get(stageId);
			// クリア済みにする
			ss.ClearStage(stageId);
			// 成功回数をインクリメント
			ss.IncSuccessNum(stageId);
		}

		/* ゲーム失敗時 */
		private void GameFail()
		{
			if (OnFail != null) OnFail();
			EndProcess();
			failureAudioSource.Play();
			resultText.GetComponent<Text>().text = FAILURE_TEXT;
			// 失敗回数をインクリメント
			var ss = StageStatus.Get(stageId);
			ss.IncFailureNum(stageId);
		}

		/* ゲーム終了時の共通処理 */
		private void EndProcess()
		{
			playingAudioSource.Stop();
			resultWindow.SetActive(true);
		}

		/* リトライボタン押下時 */
		public void RetryButtonDown()
		{
			// 挑戦回数をインクリメント
			var ss = StageStatus.Get(stageId);
			ss.IncChallengeNum(stageId);
			Dispatch(GameState.Opening);
		}

		/* 戻るボタン押下時 */
		public void BackButtonDown()
		{
			// StageSelectSceneに戻る
			SceneManager.LoadScene(SceneName.MENU_SELECT_SCENE);
		}

		/* タイル・パネル・銃弾オブジェクトの削除 */
		private static void CleanObject()
		{
			GameObject[] tiles = GameObject.FindGameObjectsWithTag(TagName.TILE);
			foreach (var tile in tiles)
			{
				// タイルの削除 (に伴いパネルも削除される)
				DestroyImmediate(tile);
			}

			GameObject[] bullets = GameObject.FindGameObjectsWithTag(TagName.BULLET);
			foreach (var bullet in bullets)
			{
				// 銃弾の削除
				DestroyImmediate(bullet);
			}
		}

		/* ゲーム画面のアスペクト比を統一する */
		private static void UnifyDisplay(GameObject resultWindow)
		{
			// 想定するデバイスのアスペクト比
			const float targetRatio = WindowSize.WIDTH / WindowSize.HEIGHT;
			// 実際のデバイスのアスペクト比
			var currentRatio = (float) Screen.width / Screen.height;
			// 許容するアスペクト比の誤差
			const float aspectRatioError = 0.001f;
			if (currentRatio > targetRatio + aspectRatioError)
			{
				// 横長のデバイスの場合
				var ratio = targetRatio / currentRatio;
				var rectX = (1 - ratio) / 2f;
				Camera.main.rect = new Rect(rectX, 0f, ratio, 1f);
				// 結果ウィンドウも変える
				resultWindow.transform.localScale = new Vector2(ratio, ratio);
			}
			else if (currentRatio < targetRatio - aspectRatioError)
			{
				// 縦長のデバイスの場合
				var ratio = currentRatio / targetRatio;
				var rectY = (1 - ratio) / 2f;
				Camera.main.rect = new Rect(0f, rectY, 1f, ratio);
				// 結果ウィンドウも変える
				resultWindow.transform.localScale = new Vector2(ratio, ratio);
			}
		}

		/* 音源のセットアップ */
		private void SetAudioSources()
		{
			// 各音源の設定
			// Playing
			gameObject.AddComponent<AudioSource>();
			playingAudioSource = gameObject.GetComponents<AudioSource>()[0];
			SetAudioSource(clipName: ClipName.PLAYING, audioSource: playingAudioSource, time: 2.0f, loop: true,
				volumeRate: 0.25f);
			// Success
			gameObject.AddComponent<AudioSource>();
			successAudioSource = gameObject.GetComponents<AudioSource>()[1];
			SetAudioSource(clipName: ClipName.SUCCESS, audioSource: successAudioSource);
			// Failure
			gameObject.AddComponent<AudioSource>();
			failureAudioSource = gameObject.GetComponents<AudioSource>()[2];
			SetAudioSource(clipName: ClipName.FAILURE, audioSource: failureAudioSource);
		}

		/* 個々の音源のセットアップ (音源名 / 開始時間 / 繰り返し の設定) */
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

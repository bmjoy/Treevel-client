using System;
using System.Linq;
using Project.Scripts.Library.Data;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
	public class GamePlayDirector : MonoBehaviour
	{
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

		private GameObject tileGenerator;

		private GameObject panelGenerator;

		private GameObject bulletGenerator;

		private GameObject resultWindow;

		private GameObject resultText;

		private GameObject warningText;

		private GameObject stageNumberText;

		private void Start()
		{
			UnifyDisplay();
			tileGenerator = GameObject.Find("TileGenerator");
			panelGenerator = GameObject.Find("PanelGenerator");
			bulletGenerator = GameObject.Find("BulletGenerator");

			resultWindow = GameObject.Find("ResultWindow");
			resultWindow.SetActive(false);

			resultText = resultWindow.transform.Find("Result").gameObject;
			warningText = resultWindow.transform.Find("Warning").gameObject;
			stageNumberText = GameObject.Find("StageNumberText");
			// 現在のステージ番号を格納
			stageNumberText.GetComponent<Text>().text = stageId.ToString();

			Dispatch(GameState.Opening);
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus) // アプリがバックグラウンドに移動した時
			{
				Dispatch(GameState.Failure);
				warningText.GetComponent<Text>().text = "アプリが\nバックグラウンドに\n移動しました";
			}
		}

		public void CheckClear()
		{
			GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
			// 全てのパネルが最終位置にいたら，成功状態に遷移
			if (panels.Any(panel => panel.GetComponent<NormalPanelController>().adapted == false)) return;

			Dispatch(GameState.Success);
		}

		// 状態による振り分け処理
		public void Dispatch(GameState state)
		{
			switch (state)
			{
				case GameState.Opening:
					GameOpening();
					break;
				case GameState.Playing:
					break;
				case GameState.Success:
					GameSucceed();
					break;
				case GameState.Failure:
					GameFail();
					break;
				default:
					throw new ArgumentOutOfRangeException("state", state, null);
			}
		}

		private void GameOpening()
		{
			// タイル作成スクリプトを起動
			tileGenerator.GetComponent<TileGenerator>().CreateTiles(stageId);
			// パネル作成スクリプトを起動
			panelGenerator.GetComponent<PanelGenerator>().CreatePanels(stageId);
			// 銃弾作成スクリプトを起動
			bulletGenerator.GetComponent<BulletGenerator>().CreateBullets(stageId);

			Destroy(tileGenerator);
			Destroy(panelGenerator);
			// 状態を変更する
			Dispatch(GameState.Playing);
		}

		private void GameSucceed()
		{
			resultWindow.SetActive(true);
			resultText.GetComponent<Text>().text = "成功！";
			Destroy(bulletGenerator);
			if (OnSucceed != null) OnSucceed();
		}

		private void GameFail()
		{
			resultWindow.SetActive(true);
			resultText.GetComponent<Text>().text = "失敗！";
			Destroy(bulletGenerator);
			if (OnFail != null) OnFail();
		}

		public void RetryButtonDown()
		{
			// 現在のScene名を取得する
			Scene loadScene = SceneManager.GetActiveScene();
			// Sceneの読み直し
			SceneManager.LoadScene(loadScene.name);
		}

		public void BackButtonDown()
		{
			// StageSelectSceneに戻る
			SceneManager.LoadScene("StageSelectScene");
		}

		private static void UnifyDisplay()
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
			}
			else if (currentRatio < targetRatio - aspectRatioError)
			{
				// 縦長のデバイスの場合
				var ratio = currentRatio / targetRatio;
				var rectY = (1 - ratio) / 2f;
				Camera.main.rect = new Rect(0f, rectY, 1f, ratio);
			}
		}
	}
}

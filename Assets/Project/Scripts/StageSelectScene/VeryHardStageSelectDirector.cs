using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
	public class VeryHardStageSelectDirector : MonoBehaviour
	{
		public GameObject stageButtonPrefab;

		private void Awake()
		{
			SetListener();
			makeButtons();
		}

		private static void SetListener()
		{
			// Buttons/*にリスナーを登録
			foreach (Transform child in GameObject.Find("Buttons").transform)
			{
				var obj = child.gameObject;
				obj.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(obj));
			}
		}

		private void makeButtons()
		{
			var content = GameObject.Find("Canvas/Scroll View/Viewport/Content/Buttons").GetComponent<RectTransform>();
			for (var i = 0; i < 10; i++)
			{
				var button = Instantiate(stageButtonPrefab);
				button.name = (i+1).ToString();
				button.transform.SetParent(content, false);
				button.GetComponentInChildren<Text>().text = "ステージ" + (i+1) + "へ";
				button.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(button));
				button.GetComponent<Image>().color = new Color(1.0f, 0.25f, 0.25f);
				var rectTransform = button.GetComponent<RectTransform>();
				var buttonPositionY = 0.05f + i * 0.10f;
				rectTransform.anchorMax = new Vector2(0.90f, buttonPositionY+0.02f);
				rectTransform.anchorMin = new Vector2(0.10f, buttonPositionY-0.02f);
				rectTransform.anchoredPosition = new Vector2(0.50f, buttonPositionY);
			}
		}

		private static void StageButtonDown(GameObject clickedButton)
		{
			// タップされたステージidを取得（暫定的にボタンの名前）
			var stageId = int.Parse(clickedButton.name);
			// 挑戦回数をインクリメント
			var ss = StageStatus.Get(stageId);
			ss.IncChallengeNum(stageId);
			// リセットのデバッグ用
			if (stageId == 1)
			{
				StageStatus.Reset(2);
			}
			else if (stageId == 2)
			{
				StageStatus.Reset(1);
			}

			// ステージ番号を渡す
			GamePlayDirector.stageId = stageId;
			// シーン遷移
			SceneManager.LoadScene("GamePlayScene");
		}
	}
}

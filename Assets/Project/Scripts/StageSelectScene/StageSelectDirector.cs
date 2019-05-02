using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
	public abstract class StageSelectDirector : MonoBehaviour
	{
		public GameObject stageButtonPrefab;

		private void Awake()
		{
			// ボタンの作成
			MakeButtons();
			// Buttonのリスナーを設定
			SetListener();
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

		// ボタンの生成
		private void MakeButtons()
		{
			var content = GameObject.Find("Canvas/Scroll View/Viewport/Content/Buttons").GetComponent<RectTransform>();
			for (var i = 0; i < 10; i++)
			{
				var button = Instantiate(stageButtonPrefab);
				button.name = (i + 1).ToString();
				button.transform.SetParent(content, false);
				button.GetComponentInChildren<Text>().text = "ステージ" + (i + 1) + "へ";
				button.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(button));
				// Buttonの色
				button.GetComponent<Image>().color = SetButtonColor();
				// Buttonの位置
				var rectTransform = button.GetComponent<RectTransform>();
				// 下部のマージン : 0.05f
				// ボタン間の間隔 : 0.10f
				var buttonPositionY = 0.05f + i * 0.10f;
				// ボタンの縦幅 : 0.04f (上に0.02f, 下に0.02fをアンカー中央から伸ばす)
				rectTransform.anchorMax = new Vector2(0.90f, buttonPositionY + 0.02f);
				rectTransform.anchorMin = new Vector2(0.10f, buttonPositionY - 0.02f);
				rectTransform.anchoredPosition = new Vector2(0.50f, buttonPositionY);
			}
		}

		protected abstract Color SetButtonColor();
	}
}

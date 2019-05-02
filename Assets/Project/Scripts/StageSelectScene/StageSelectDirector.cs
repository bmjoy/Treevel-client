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

		protected static void StageButtonDown(GameObject clickedButton)
		{
			// タップされたステージidを取得（暫定的にボタンの名前）
			var stageId = int.Parse(clickedButton.name);
			// 挑戦回数をインクリメント
			var ss = StageStatus.Get(stageId);
			ss.IncChallengeNum(stageId);
			// ステージ番号を渡す
			GamePlayDirector.stageId = stageId;
			// シーン遷移
			SceneManager.LoadScene("GamePlayScene");
		}

		/* ボタンの生成 */
		protected abstract void MakeButtons();
	}
}

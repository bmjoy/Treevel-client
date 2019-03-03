using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
	public class StageSelectDirector : MonoBehaviour
	{
		private void Start()
		{
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
	}
}

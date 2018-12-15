using Project.Scripts.GamePlayScene;
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

		private void SetListener()
		{
			// Buttons/*にリスナーを登録
			foreach (Transform child in GameObject.Find("Buttons").transform)
			{
				GameObject obj = child.gameObject;
				obj.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(obj));
			}
		}

		private void StageButtonDown(GameObject clickedButton)
		{
			// タップされたステージidを取得（暫定的にボタンの名前）
			var stageId = int.Parse(clickedButton.name);
			// ステージ番号を渡す
			GamePlayDirector.stageId = stageId;
			// シーン遷移
			SceneManager.LoadScene("GamePlayScene");
		}
	}
}

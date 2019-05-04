using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.StageSelectScene
{
	public abstract class StageSelectDirector : MonoBehaviour
	{
		public GameObject stageButtonPrefab;

		public GameObject dummuBackgroundPrefab;

		private void Awake()
		{
			// ボタンの作成
			MakeButtons();
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

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.LevelSelectScene
{
	public class LevelSelectDirector : MonoBehaviour
	{
		private string nowScene;

		private GameObject easyStageSelectToggle;

		private GameObject normalStageSelectToggle;

		private GameObject hardStageSelectToggle;

		private GameObject veryHardStageSelectToggle;

		private void Awake()
		{
			// Toggleの取得
			easyStageSelectToggle = GameObject.Find("EasyStageSelect");
			normalStageSelectToggle = GameObject.Find("NormalStageSelect");
			hardStageSelectToggle = GameObject.Find("HardStageSelect");
			veryHardStageSelectToggle = GameObject.Find("VeryHardStageSelect");
			// Toggleのリスナーを設定
			AddListeners();
			// 初期シーンのロード
			StartCoroutine(AddScene("EasyStageSelectScene"));
		}

		private IEnumerator AddScene(string sceneName)
		{
			// シーンをロード
			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			// シーンがロードされるのを待つ
			var scene = SceneManager.GetSceneByName(sceneName);
			while (!scene.isLoaded)
			{
				yield return null;
			}

			// 指定したシーン名をアクティブにする
			SceneManager.SetActiveScene(scene);
			// シーンの保存
			nowScene = sceneName;
		}

		private void AddListeners()
		{
			easyStageSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(easyStageSelectToggle);
			});

			normalStageSelectToggle.GetComponent<Toggle>().onValueChanged.
				AddListener(delegate { ToggleValueChanged(normalStageSelectToggle); });

			hardStageSelectToggle.GetComponent<Toggle>().onValueChanged.
				AddListener(delegate { ToggleValueChanged(hardStageSelectToggle); });

			veryHardStageSelectToggle.GetComponent<Toggle>().onValueChanged.
				AddListener(delegate { ToggleValueChanged(veryHardStageSelectToggle); });
		}

		private void ToggleValueChanged(GameObject toggle)
		{
			// ONになった場合のみ処理
			if (toggle.GetComponent<Toggle>().isOn)
			{
				// 今のシーンをアンロード
				SceneManager.UnloadSceneAsync(nowScene);
				// 新しいシーンをロード
				StartCoroutine(AddScene(toggle.name + "Scene"));
			}
		}

	}
}

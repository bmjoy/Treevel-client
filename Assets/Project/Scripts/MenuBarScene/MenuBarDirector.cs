using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuBarScene
{
	public class MenuBarDirector : MonoBehaviour
	{
		private string nowScene;

		private GameObject stageSelectToggle;

		private GameObject recordToggle;

		private GameObject tutorialToggle;

		private GameObject configToggle;

		private void Start()
		{
			// Toggleの取得
			stageSelectToggle = GameObject.Find("StageSelect");
			recordToggle = GameObject.Find("Record");
			tutorialToggle = GameObject.Find("Tutorial");
			configToggle = GameObject.Find("Config");
			// Toggleのリスナーを設定
			AddListeners();
			// 初期シーンのロード
			StartCoroutine(AddScene("StageSelectScene"));
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
			stageSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(stageSelectToggle);
			});

			recordToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(recordToggle);
			});

			tutorialToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(tutorialToggle);
			});

			configToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(configToggle);
			});
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

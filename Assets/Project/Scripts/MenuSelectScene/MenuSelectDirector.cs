using System.Collections;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene
{
	public class MenuSelectDirector : MonoBehaviour
	{
		private string nowScene;

		private GameObject levelSelectToggle;

		private GameObject recordToggle;

		private GameObject tutorialToggle;

		private GameObject configToggle;

		private void Awake()
		{
			// Toggleの取得
			levelSelectToggle = GameObject.Find("LevelSelect");
			recordToggle = GameObject.Find("Record");
			tutorialToggle = GameObject.Find("Tutorial");
			configToggle = GameObject.Find("Config");
			// Toggleのリスナーを設定
			AddListeners();
			// 初期シーンのロード
			StartCoroutine(AddScene(SceneName.LEVEL_SELECT_SCENE));
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
			levelSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				ToggleValueChanged(levelSelectToggle);
			});

			recordToggle.GetComponent<Toggle>().onValueChanged
				.AddListener(delegate { ToggleValueChanged(recordToggle); });

			tutorialToggle.GetComponent<Toggle>().onValueChanged
				.AddListener(delegate { ToggleValueChanged(tutorialToggle); });

			configToggle.GetComponent<Toggle>().onValueChanged
				.AddListener(delegate { ToggleValueChanged(configToggle); });
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

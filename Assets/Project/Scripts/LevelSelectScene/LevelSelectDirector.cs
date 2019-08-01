using System.Collections;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.LevelSelectScene
{
    public class LevelSelectDirector : MonoBehaviour
    {
        public const string LEVEL_TAB_TOGGLE_GROUP_NAME = "LevelTab";
        private const string EASY_STAGE_SELECT_TOGGLE_NAME = "EasyStageSelect";
        private const string NORMAL_STAGE_SELECT_TOGGLE_NAME = "NormalStageSelect";
        private const string HARD_STAGE_SELECT_TOGGLE_NAME = "HardStageSelect";
        private const string VERY_HARD_STAGE_SELECT_TOGGLE_NAME = "VeryHardStageSelect";

        private string nowScene;

        private GameObject easyStageSelectToggle;

        private GameObject normalStageSelectToggle;

        private GameObject hardStageSelectToggle;

        private GameObject veryHardStageSelectToggle;

        private void Awake()
        {
            // Toggleの取得
            easyStageSelectToggle = GameObject.Find(EASY_STAGE_SELECT_TOGGLE_NAME);
            normalStageSelectToggle = GameObject.Find(NORMAL_STAGE_SELECT_TOGGLE_NAME);
            hardStageSelectToggle = GameObject.Find(HARD_STAGE_SELECT_TOGGLE_NAME);
            veryHardStageSelectToggle = GameObject.Find(VERY_HARD_STAGE_SELECT_TOGGLE_NAME);
            // Toggleのリスナーを設定
            AddListeners();
            // 初期シーンのロード
            StartCoroutine(AddScene(SceneName.EASY_STAGE_SELECT_SCENE));
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /* このシーンがアンロードされた時に，`nowScene`もアンロードする */
        private void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == SceneName.LEVEL_SELECT_SCENE) {
                SceneManager.UnloadSceneAsync(nowScene);
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
            }
        }

        private IEnumerator AddScene(string sceneName)
        {
            // シーンをロード
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            // シーンがロードされるのを待つ
            var scene = SceneManager.GetSceneByName(sceneName);
            while (!scene.isLoaded) {
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

            normalStageSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(normalStageSelectToggle);
            });

            hardStageSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(hardStageSelectToggle);
            });

            veryHardStageSelectToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(veryHardStageSelectToggle);
            });
        }

        private void ToggleValueChanged(GameObject toggle)
        {
            // ONになった場合のみ処理
            if (toggle.GetComponent<Toggle>().isOn) {
                if (toggle.name + "Scene" != nowScene) {
                    // 今のシーンをアンロード
                    SceneManager.UnloadSceneAsync(nowScene);
                    // 新しいシーンをロード
                    StartCoroutine(AddScene(toggle.name + "Scene"));
                }
            }
        }
    }
}

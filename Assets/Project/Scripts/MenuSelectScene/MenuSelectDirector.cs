using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine.SceneManagement;

namespace Project.Scripts.MenuSelectScene
{
    public class MenuSelectDirector : SingletonObject<MenuSelectDirector>
    {
        public string nowScene;

        private void Awake()
        {
            // 初期シーンのロード
            StartCoroutine(AddScene(SceneName.STAGE_SELECT_SCENE));
        }

        public IEnumerator AddScene(string sceneName)
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
    }
}

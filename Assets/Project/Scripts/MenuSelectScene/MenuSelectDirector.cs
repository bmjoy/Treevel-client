using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine.SceneManagement;

namespace Project.Scripts.MenuSelectScene
{
    public class MenuSelectDirector : SingletonObject<MenuSelectDirector>
    {
        /// <summary>
        /// 現在表示しているシーン
        /// </summary>
        public string NowScene
        {
            get;
            private set;
        }

        private void Awake()
        {
            // 初期シーンのロード
            StartCoroutine(ChangeScene(SceneName.LEVEL_SELECT_SCENE));
        }

        /// <summary>
        /// シーンを変更する
        /// </summary>
        /// <param name="sceneName"> シーン名 </param>
        public IEnumerator ChangeScene(string sceneName)
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
            NowScene = sceneName;
        }
    }
}

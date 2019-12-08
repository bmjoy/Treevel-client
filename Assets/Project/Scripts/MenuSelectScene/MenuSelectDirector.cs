using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene
{
    public class MenuSelectDirector : SingletonObject<MenuSelectDirector>
    {
        /// <summary>
        /// ON状態になっているToggle
        /// 初期値はStageSelectToggle
        /// </summary>
        [SerializeField] private MenuSelectToggle nowToggle;
        
        private void Awake()
        {
            // 初期シーンのロード
            StartCoroutine(ChangeScene());
        }

        /// <summary>
        /// シーンを変更する
        /// </summary>
        /// <param name="sceneName"> シーン名 </param>
        public IEnumerator ChangeScene()
        {
            string nowSceneName = nowToggle.GetSceneName();
            // シーンをロード
            SceneManager.LoadScene(nowSceneName, LoadSceneMode.Additive);
            // シーンがロードされるのを待つ
            var scene = SceneManager.GetSceneByName(nowSceneName);
            while (!scene.isLoaded) {
                yield return null;
            }

            // 指定したシーン名をアクティブにする
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toggle"></param>
        public void SetNowScene(MenuSelectToggle toggle) {
            // 現在シーンに紐づいているToggleをOFFにする
            if(nowToggle != toggle) {
                nowToggle.isOn = false;
                nowToggle = toggle;
            }
            // 新しいシーンをロード
            StartCoroutine(ChangeScene());
        }
    }
}

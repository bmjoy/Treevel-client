using System.Collections;
using System.Threading.Tasks;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Networks.Commands;
using Project.Scripts.Networks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Project.Scripts.StartUpScene
{
    public class StartUpDirector : MonoBehaviour
    {
        private IEnumerator Start()
        {
            // UIManager Initialize
            yield return new WaitWhile(() => !UIManager.Instance.Initialized);

            // Network Initialization
            // TODO : ユーザー情報の初期化（IDの取得OR発行など）
            NetworkService.Execute(new HelloWorldRequest(), (data) => {
                Debug.Log(data as string);
            });

            // AAS Initialize
            AddressableAssetManager.Initalize().Completed += OnAASInitializeCompleted;

            // Database Initialize
            GameDataBase.Initialize();
        }

        /// <summary>
        /// AASが初期化された直後に行うべきこと。
        /// 1. MenuSelectSceneを読み込む
        /// 2. MenuSelectSceneを読み込んだ直後にStartUpSceneをアンロード
        /// </summary>
        private void OnAASInitializeCompleted(AsyncOperationHandle handle)
        {
            var menuSelectSceneHandler = AddressableAssetManager.LoadScene(SceneName.MENU_SELECT_SCENE, LoadSceneMode.Additive);
            menuSelectSceneHandler.Completed += async(handle2) => {
                // TODO remove before merged
                await Task.Delay(1000);

                // Unload Startup Scene
                SceneManager.UnloadSceneAsync(SceneName.START_UP_SCENE);
            };
        }
    }
}

using System.Collections;
using System.Threading.Tasks;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
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

            // AAS Initialize
            AddressableAssetManager.Initalize().Completed += OnAASInitializeCompleted;

            // Database Initialize
            GameDataBase.Initialize();

            // TODO Network Initialize
        }

        /// <summary>
        /// AASが初期化された直後に行うべきこと。
        /// 1. MenuSelectSceneを読み込む
        /// 2. MenuSelectSceneが読み込んだ直後にStartUpSceneをアンロード
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

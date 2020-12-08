using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Treevel.Modules.StartUpScene
{
    public class StartUpDirector : MonoBehaviour
    {
        [SerializeField] private GameObject _startButton;

        private async void Start()
        {
            // Don't destroy EventSystem
            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem != null)
                DontDestroyOnLoad(eventSystem.gameObject);

            // UIManager Initialize
            await UniTask.WaitUntil(() => UIManager.Instance.Initialized);

            // AppManager Initialize
            AppManager.OnApplicationStart();

            #if ENV_DEV
            Debug.Log("DEV");
            #else
            Debug.Log("PROD");
            #endif

            // AAS Initialize
            AddressableAssetManager.Initialize().Completed += OnAASInitializeCompleted;

            // Database Initialize
            GameDataManager.Initialize();
        }

        /// <summary>
        /// AASが初期化された直後に行うべきこと。
        /// 1. MenuSelectSceneを読み込む
        /// 2. MenuSelectSceneを読み込んだ直後にStartUpSceneをアンロード
        /// </summary>
        private void OnAASInitializeCompleted(AsyncOperationHandle handle)
        {
            var menuSelectSceneHandler = AddressableAssetManager.LoadScene(Constants.SceneName.MENU_SELECT_SCENE, LoadSceneMode.Additive);
            menuSelectSceneHandler.Completed += async(handle2) => {
                // TODO remove before merged
                await Task.Delay(1000);
                _startButton.SetActive(true);
            };
        }

        public void OnStartButtonClicked()
        {
            // Unload Startup Scene
            SceneManager.UnloadSceneAsync(Constants.SceneName.START_UP_SCENE);
        }
    }
}

using Cysharp.Threading.Tasks;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Treevel.Modules.StartUpScene
{
    public class StartUpDirector : MonoBehaviour
    {
        [SerializeField] private GameObject _startButton;

        private async void Start()
        {
            // Don't destroy EventSystem
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null) DontDestroyOnLoad(eventSystem.gameObject);

            // UIManager Initialize
            await UIManager.Instance.Initialize();

            // AppManager Initialize
            AppManager.OnApplicationStart();

            #if ENV_DEV
            Debug.Log("DEV");
            #else
            Debug.Log("PROD");
            #endif

            // AASを初期化
            await AddressableAssetManager.Initialize();

            // MenuSelectSceneを読み込み
            var loadSceneTask = AddressableAssetManager
                .LoadScene(Constants.SceneName.MENU_SELECT_SCENE, LoadSceneMode.Additive).ToUniTask();
            // GameDataManager初期化
            var dataManagerInitTask = GameDataManager.Initialize();

            // 全部完了したら開始ボタンを表示
            await UniTask.WhenAll(loadSceneTask, dataManagerInitTask);

            _startButton.SetActive(true);
        }

        public void OnStartButtonClicked()
        {
            // Unload Startup Scene
            SceneManager.UnloadSceneAsync(Constants.SceneName.START_UP_SCENE);
        }
    }
}

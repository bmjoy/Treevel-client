using Cysharp.Threading.Tasks;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;
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

            // AASを初期化
            // => MenuSelectSceneを読み込み
            // => 開始ボタンをアクティブにする
            AddressableAssetManager.Initialize()
                .ToObservable()
                .Subscribe(_ => {
                    AddressableAssetManager.LoadScene(Constants.SceneName.MENU_SELECT_SCENE, LoadSceneMode.Additive)
                        .ToObservable()
                        .Subscribe(__ => _startButton.SetActive(true));
                });

            // Database Initialize
            GameDataManager.Initialize();
        }

        public void OnStartButtonClicked()
        {
            // Unload Startup Scene
            SceneManager.UnloadSceneAsync(Constants.SceneName.START_UP_SCENE);
        }
    }
}

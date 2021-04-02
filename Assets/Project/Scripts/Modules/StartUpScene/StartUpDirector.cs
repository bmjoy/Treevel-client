using Cysharp.Threading.Tasks;
using PlayFab;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
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
            SoundManager.Instance.PlayBGM(EBGMKey.StartUp);

            // Don't destroy EventSystem
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null) DontDestroyOnLoad(eventSystem.gameObject);

            // UIManager Initialize
            await UIManager.Instance.InitializeAsync();

            // AppManager Initialize
            AppManager.OnApplicationStart();

            #if ENV_DEV
            Debug.Log("DEV");
            #else
            Debug.Log("PROD");
            #endif

            // AASを初期化
            await AddressableAssetManager.Initialize();

            // リモートサーバへ接続（ログイン）
            if (await NetworkService.Execute(new LoginRequest())) {
                Debug.Log($"PlayFabID[{PlayFabSettings.staticPlayer.PlayFabId}] Login Success");
            }

            // MenuSelectSceneを読み込み
            var loadSceneTask = AddressableAssetManager
                .LoadScene(Constants.SceneName.MENU_SELECT_SCENE, LoadSceneMode.Additive).ToUniTask();
            // GameDataManager初期化
            var dataManagerInitTask = GameDataManager.InitializeAsync();

            // 全部完了したら開始ボタンを表示
            await UniTask.WhenAll(loadSceneTask, dataManagerInitTask);

            _startButton.SetActive(true);
        }

        public void OnStartButtonClicked()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click1);

            // MenuSelectSceneのBGMを流す
            SoundManager.Instance.PlayBGM(EBGMKey.MenuSelect);

            // Unload Startup Scene
            SceneManager.UnloadSceneAsync(Constants.SceneName.START_UP_SCENE);
        }
    }
}
